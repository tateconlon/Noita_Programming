// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.SubAssets.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(SubAssetAttribute))]
    public class SubAssetPropertyDrawer : PropertyDrawer
    {
        private const int OPTIONAL_BUTTON_WIDTH = 60;
        private const int OPTIONAL_BUTTON_PADDING = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (SubAssetHelper.IsValidSubAssetProperty(property))
            {
                var serializedObject = property.serializedObject;
                var mainAsset = serializedObject.targetObject;
                var subAssetAttribute = (SubAssetAttribute)attribute;
                var subAssetIsOptional = subAssetAttribute.optional;
                var subAssetIsMissing = property.objectReferenceValue == null;

                var propertyFieldRect = position;

                if (!subAssetIsOptional)
                {
                    if (subAssetIsMissing)
                    {
                        var didUpdate = SubAssetHelper.UpdateAllSubAssets(serializedObject);
                        if (didUpdate)
                        {
                            SubAssetHelper.SaveAndReimportAsset(mainAsset);
                        }
                    }
                }
                else
                {
                    propertyFieldRect.width -= OPTIONAL_BUTTON_WIDTH + OPTIONAL_BUTTON_PADDING;
                }

                DrawPropertyField(propertyFieldRect, property, label);

                if (subAssetIsOptional)
                {
                    var optionalButtonRect = new Rect(position)
                    {
                        x = propertyFieldRect.xMax + OPTIONAL_BUTTON_PADDING,
                        width = OPTIONAL_BUTTON_WIDTH
                    };

                    if (GUI.Button(optionalButtonRect, subAssetIsMissing ? "Create" : "Destroy"))
                    {
                        if (subAssetIsMissing)
                        {
                            var subAsset = SubAssetHelper.CreateSubAsset(fieldInfo, mainAsset);
                            if (subAsset)
                            {
                                Undo.IncrementCurrentGroup();
                                Undo.SetCurrentGroupName("Create sub asset");

                                Undo.RegisterCreatedObjectUndo(subAsset, "Create sub asset");

                                Undo.RecordObject(mainAsset, "Assign sub asset");
                                property.objectReferenceValue = subAsset;

                                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                            }
                        }
                        else
                        {
                            var shouldDestroy = EditorUtility.DisplayDialog("Destroy Soda SubAsset", "Do you really want to destroy this sub asset?", "Yes", "Cancel");
                            if (shouldDestroy)
                            {
                                var subAsset = property.objectReferenceValue;

                                Undo.IncrementCurrentGroup();
                                Undo.SetCurrentGroupName("Destroy sub asset");

                                Undo.RecordObject(mainAsset, "Nullify sub asset");
                                property.objectReferenceValue = null;

                                Undo.DestroyObjectImmediate(subAsset);

                                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                            }
                        }

                        if (serializedObject.ApplyModifiedProperties())
                        {
                            SubAssetHelper.SaveAndReimportAsset(mainAsset);
                        }
                    }
                }
            }
            else
            {
                var helpBoxRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight
                };
                helpBoxRect = EditorGUI.PrefixLabel(helpBoxRect, GUIContent.none);
                EditorGUI.HelpBox(helpBoxRect, "The [SubAsset] attribute can only be used on fields referencing ScriptableObjects.", MessageType.Error);

                var propertyRect = new Rect(position);
                propertyRect.yMin += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(propertyRect, property, label);
            }
        }

        private static void DrawPropertyField(Rect rect, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            label.text = $"[SubAsset] {label.text}";
            EditorGUI.PropertyField(rect, property, label);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (SubAssetHelper.IsValidSubAssetProperty(property))
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                var originalHeight = EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
                return originalHeight + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            }
        }
    }
}
