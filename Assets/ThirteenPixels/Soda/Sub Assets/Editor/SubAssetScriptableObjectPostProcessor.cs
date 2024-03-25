// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.SubAssets.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    internal class SubAssetScriptableObjectPostProcessor : AssetPostprocessor
    {
        private static readonly List<ScriptableObject> importedScriptableObjects = new List<ScriptableObject>();

        private static void OnPostprocessAllAssets(string[] importedAssetPaths, string[] deletedAssetPaths, string[] movedAssetPaths, string[] movedFromAssetPaths)
        {
            if (OnlyContainsIgnoredAssetPath(importedAssetPaths)) return;

            importedScriptableObjects.Clear();

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (var path in importedAssetPaths)
                {
                    var asset = AssetDatabase.LoadMainAssetAtPath(path);
                    if (asset is ScriptableObject scriptableObject)
                    {
                        importedScriptableObjects.Add(scriptableObject);
                    }
                    else if (asset is MonoScript script)
                    {
                        var type = script.GetClass();
                        if (CouldContainSubAssets(type))
                        {
                            var guids = AssetDatabase.FindAssets("t:" + type.Name);

                            foreach (var guid in guids)
                            {
                                var instancePath = AssetDatabase.GUIDToAssetPath(guid);
                                var instance = (ScriptableObject)AssetDatabase.LoadMainAssetAtPath(instancePath);
                                importedScriptableObjects.Add(instance);
                            }
                        }
                    }
                }

                for (var i = importedScriptableObjects.Count - 1; i >= 0; i--)
                {
                    var asset = importedScriptableObjects[i];

                    var didUpdate = SubAssetHelper.UpdateAllSubAssets(asset);
                    if (!didUpdate)
                    {
                        importedScriptableObjects.RemoveAt(i);
                    }
                }

                foreach (var asset in importedScriptableObjects)
                {
                    SubAssetHelper.SaveAndReimportAsset(asset);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                importedScriptableObjects.Clear();
            }
        }

        private static bool OnlyContainsIgnoredAssetPath(string[] paths)
        {
            return paths.Length == 1 && paths[0] == SubAssetHelper.ignoredAssetPathForImport;
        }

        private static bool CouldContainSubAssets(System.Type type)
        {
            return type != null && !type.IsAbstract && type.IsSubclassOf(typeof(ScriptableObject));
        }
    }
}
