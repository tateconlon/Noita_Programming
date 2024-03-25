// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.ModuleSettings.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Manages all <see cref="ModuleSettings"/> instances.
    /// Instead of keeping them in the assets, this class stores them in the Project Settings folder in Json format.
    /// </summary>
    internal static class ModuleSettingsEditorManager
    {
        private const string ASSET_PATH = "ProjectSettings/Module Settings/";
        private static readonly Regex guidRegex = new Regex("\\\"guid\\\"\\s*:\\s*\\\"([0-9a-f]{32})\\\"\\s*,\\s*\\\"type\\\"\\s*:\\s*2");

        private static Dictionary<Type, ModuleSettings> cache => ModuleSettings.cache;
        internal static int settingsCount => cache?.Count ?? 0;


        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.delayCall += InitializeAll;

            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += (oldScene, newScene) => InitializeAll();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeAll()
        {
            cache.Clear();

            if (!Directory.Exists(ASSET_PATH))
            {
                Directory.CreateDirectory(ASSET_PATH);
            }

            var allSettingsTypes = TypeCache.GetTypesDerivedFrom<ModuleSettings>();
            foreach (var type in allSettingsTypes)
            {
                if (!type.IsAbstract)
                {
                    CreateOrGet(type);
                }
            }
        }

        internal static ModuleSettings CreateOrGet(Type type)
        {
            if (!cache.TryGetValue(type, out var instance))
            {
                instance = Load(type);

                if (!instance)
                {
                    instance = (ModuleSettings)ScriptableObject.CreateInstance(type);
                    Save(instance);
                }

                cache[type] = instance;
            }

            return instance;
        }

        private static ModuleSettings Load(Type type)
        {
            try
            {
                var json = File.ReadAllText(ASSET_PATH + type.FullName + ".json");

                // Using EditorJsonUtility.FromJsonOverwrite while one or more of the ScriptableObjects referenced in the json file are not loaded causes an ugly error message in the console.
                // ("Unexpected recursive transfer of scripted class. [...]")
                // Preload all scripted assets via their guid as stored in the json file to prevent this.
                PreloadReferencedScriptedAssets(json);

                var instance = (ModuleSettings)ScriptableObject.CreateInstance(type);
                EditorJsonUtility.FromJsonOverwrite(json, instance);
                return instance;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Preloads all scripted assets that are referenced in the given json file.
        /// </summary>
        private static void PreloadReferencedScriptedAssets(string json)
        {
            var match = guidRegex.Match(json);
            if (match.Success)
            {
                for (var i = 1; i < match.Groups.Count; i++)
                {
                    var guid = match.Groups[i].Value;
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                }
            }
        }

        internal static void Save(ModuleSettings instance)
        {
            var name = instance.GetType().FullName;
            var json = EditorJsonUtility.ToJson(instance, true);
            try
            {
                File.WriteAllText(ASSET_PATH + name + ".json", json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static IEnumerable<ModuleSettings> GetAll()
        {
            InitializeAll();
            return cache.Values;
        }
    }
}
