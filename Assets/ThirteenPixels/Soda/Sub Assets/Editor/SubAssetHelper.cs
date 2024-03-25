// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.SubAssets.Editor
{
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    internal static class SubAssetHelper
    {
        internal const string SUB_ASSET_PREFIX = "→ ";
        private const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly List<Object> existingSubAssets = new List<Object>();
        private static readonly List<FieldInfo> fieldInfos = new List<FieldInfo>();

        internal static string ignoredAssetPathForImport { get; private set; } = null;
        private static readonly List<string> pathsToReimport = new List<string>();


        internal static bool UpdateAllSubAssets(SerializedObject serializedObject)
        {
            return UpdateAllSubAssets(serializedObject.targetObject, serializedObject);
        }

        internal static bool UpdateAllSubAssets(Object mainAsset)
        {
            return UpdateAllSubAssets(mainAsset, null);
        }

        /// <summary>
        /// Goes through the list of [SubAsset]s of the script of a ScriptableObject and makes sure that every [SubAsset] field has a valid sub asset assigned.
        /// Can be called with either of the parameters. At least one of them has to be non-null.
        /// </summary>
        private static bool UpdateAllSubAssets(Object mainAsset, SerializedObject serializedObject)
        {
            if (mainAsset == null) throw new System.ArgumentNullException(nameof(mainAsset));

            if (!CanAddSubAssets(mainAsset))
            {
                return false;
            }

            fieldInfos.Clear();
            existingSubAssets.Clear();

            FindSodaSubAssetsOf(mainAsset);
            FindSodaSubAssetFieldsOf(mainAsset);

            var hasSubAssetFields = fieldInfos.Count > 0;
            var hasSodaSubAssets = existingSubAssets.Count > 0;

            if (hasSubAssetFields || hasSodaSubAssets)
            {
                if (serializedObject == null)
                {
                    serializedObject = new SerializedObject(mainAsset);
                }
                serializedObject.Update();
            }

            if (hasSubAssetFields)
            {
                FindAndRenameReferencedSubAssets(serializedObject);

                FixMissingReferencesByMatchingNames(serializedObject, GetSubAssetNameFromFormerlyserializedAsAttribute, true);
                FixMissingReferencesByMatchingNames(serializedObject, GetSubAssetNameFromFieldName);

                CreateMissingSubAssets(mainAsset, serializedObject);
            }

            var hadDanglingSubAssets = DestroyDanglingSubAssets();
            var didApplyModifiedProperties = serializedObject != null && serializedObject.ApplyModifiedProperties();

            fieldInfos.Clear();
            existingSubAssets.Clear();

            return didApplyModifiedProperties || hadDanglingSubAssets;
        }

        internal static void SaveAndReimportAsset(Object asset)
        {
            AssetDatabase.SaveAssets();

            if (pathsToReimport.Count == 0)
            {
                EditorApplication.delayCall += ReimportAssetsFromList;
            }

            var path = AssetDatabase.GetAssetPath(asset);
            pathsToReimport.Add(path);
        }

        private static void ReimportAssetsFromList()
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var path in pathsToReimport)
                {
                    try
                    {
                        ignoredAssetPathForImport = path;
                        AssetImporter.GetAtPath(path).SaveAndReimport();
                    }
                    finally
                    {
                        ignoredAssetPathForImport = null;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
            AssetDatabase.SaveAssets();
            EditorApplication.RepaintProjectWindow();

            pathsToReimport.Clear();
        }

        /// <summary>
        /// Finds all sub assets of the given asset that are controlled by the Soda Sub Assets system.
        /// Uses the <see cref="SUB_ASSET_PREFIX"/> to identify the sub assets.
        /// </summary>
        private static void FindSodaSubAssetsOf(Object mainAsset)
        {
            var mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(mainAssetPath);

            foreach (var subAsset in subAssets)
            {
                if (IsSodaSubAsset(subAsset))
                {
                    existingSubAssets.Add(subAsset);
                }
            }
        }

        /// <summary>
        /// Finds all serialized fields in the given asset's class that use the SubAsset attribute.
        /// </summary>
        private static void FindSodaSubAssetFieldsOf(Object mainAsset)
        {
            var type = mainAsset.GetType();
            var fields = type.GetFields(FIELD_BINDING_FLAGS);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<SubAssetAttribute>() != null &&
                    field.GetCustomAttribute<SerializeField>() != null &&
                    IsValidSubAssetProperty(field))
                {
                    fieldInfos.Add(field);
                }
            }
        }

        private static bool CanAddSubAssets(Object mainAsset)
        {
            // This method not only prevents sub asset chaos with sub assets in sub assets,
            // it is also used to prevent an error flood that happens when trying to add sub assets
            // to a freshly created ScriptableObject that hasn't had its name submitted yet.

            return AssetDatabase.IsMainAsset(mainAsset);
        }

        /// <summary>
        /// Goes through all sub assets referenced in fields and makes sure their names match that field's.
        /// </summary>
        private static void FindAndRenameReferencedSubAssets(SerializedObject serializedObject)
        {
            for (var i = fieldInfos.Count - 1; i >= 0; i--)
            {
                var fieldInfo = fieldInfos[i];

                var property = serializedObject.FindProperty(fieldInfo.Name);
                var subAsset = property.objectReferenceValue;
                if (subAsset != null)
                {
                    var desiredName = CreateAssetName(fieldInfo.Name);
                    if (subAsset.name != desiredName)
                    {
                        subAsset.name = desiredName;
                    }

                    existingSubAssets.Remove(subAsset);
                    fieldInfos.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns the sub asset name generated from the old name specified in the FormerlySerializedAs attribute of the given field, if there is such attribute.
        /// Returns null otherwise.
        /// </summary>
        private static string GetSubAssetNameFromFormerlyserializedAsAttribute(FieldInfo fieldInfo)
        {
            var formerlySerializedAs = fieldInfo.GetCustomAttribute<FormerlySerializedAsAttribute>();
            if (formerlySerializedAs != null)
            {
                return CreateAssetName(formerlySerializedAs.oldName);
            }
            return null;
        }

        /// <summary>
        /// Returns the sub asset name generated from the given field's name.
        /// </summary>
        private static string GetSubAssetNameFromFieldName(FieldInfo fieldInfo)
        {
            return CreateAssetName(fieldInfo.Name);
        }

        /// <summary>
        /// Attempts to assign references to existing sub assets to fields with null references.
        /// Does so by matching sub asset and field names.
        /// </summary>
        /// <param name="getExpectedName">A function that takes a <see cref="FieldInfo"/> and returns the expected name of its sub asset.</param>
        /// <param name="fixName">If true, the method expects the name of a name-matched sub asset to be deprecated. The name will be updated to match the given FieldInfo.</param>
        private static void FixMissingReferencesByMatchingNames(SerializedObject serializedObject,
            System.Func<FieldInfo, string> getExpectedName,
            bool fixName = false)
        {
            for (var i = fieldInfos.Count - 1; i >= 0; i--)
            {
                var fieldInfo = fieldInfos[i];

                var subAssetName = getExpectedName(fieldInfo);
                if (subAssetName != null)
                {
                    var subAsset = existingSubAssets.Find(asset => asset.name == subAssetName);
                    if (subAsset != null)
                    {
                        var property = serializedObject.FindProperty(fieldInfo.Name);
                        property.objectReferenceValue = subAsset;

                        if (fixName)
                        {
                            var desiredName = CreateAssetName(fieldInfo.Name);
                            if (subAsset.name != desiredName)
                            {
                                subAsset.name = desiredName;
                            }
                        }

                        existingSubAssets.Remove(subAsset);
                        fieldInfos.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Creates all sub assets that fields require but don't have yet.
        /// </summary>
        private static void CreateMissingSubAssets(Object mainAsset, SerializedObject serializedObject)
        {
            for (var i = fieldInfos.Count - 1; i >= 0; i--)
            {
                var fieldInfo = fieldInfos[i];

                var subAssetAttribute = fieldInfo.GetCustomAttribute<SubAssetAttribute>();
                if (subAssetAttribute.optional)
                {
                    continue;
                }

                var subAsset = CreateSubAsset(fieldInfo, mainAsset);

                var property = serializedObject.FindProperty(fieldInfo.Name);
                property.objectReferenceValue = subAsset;
            }

            fieldInfos.Clear();
        }

        internal static ScriptableObject CreateSubAsset(FieldInfo fieldInfo, Object mainAsset)
        {
            var subAsset = ScriptableObject.CreateInstance(fieldInfo.FieldType);

            var subAssetName = CreateAssetName(fieldInfo.Name);
            subAsset.name = subAssetName;

            AssetDatabase.AddObjectToAsset(subAsset, mainAsset);

            return subAsset;
        }

        /// <summary>
        /// Destroys all Soda-controlled sub assets that no field of the main asset's class feels responsible for.
        /// </summary>
        private static bool DestroyDanglingSubAssets()
        {
            var hasDanglingSubAssets = existingSubAssets.Count > 0;
            foreach (var danglingSubAsset in existingSubAssets)
            {
                Object.DestroyImmediate(danglingSubAsset, true);
            }

            return hasDanglingSubAssets;
        }

        private static string CreateAssetName(string name)
        {
            return SUB_ASSET_PREFIX + name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
        }

        private static bool IsSodaSubAsset(Object asset)
        {
            return asset && asset.name != null && asset.name.StartsWith(SUB_ASSET_PREFIX);
        }

        internal static bool IsValidSubAssetProperty(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference &&
                IsValidSubAssetProperty(GetFieldInfoForProperty(property));
        }

        internal static bool IsValidSubAssetProperty(FieldInfo field)
        {
            return field != null && field.FieldType.IsSubclassOf(typeof(ScriptableObject));
        }

        private static FieldInfo GetFieldInfoForProperty(SerializedProperty property)
        {
            var objectType = property.serializedObject.targetObject.GetType();
            var fieldInfo = objectType.GetField(property.name, FIELD_BINDING_FLAGS);
            return fieldInfo;
        }
    }
}
