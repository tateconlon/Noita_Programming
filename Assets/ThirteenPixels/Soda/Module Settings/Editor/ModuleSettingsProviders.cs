// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.ModuleSettings.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A static class that creates the <see cref="SettingsProvider"/> instances that show up in the Project Settings window.
    /// </summary>
    internal static class ModuleSettingsProviders
    {
        private const string CATEGORY = "Module Settings";

        private static readonly Dictionary<Type, Editor> cachedEditors = new Dictionary<Type, Editor>();

        [SettingsProviderGroup]
        private static SettingsProvider[] CreateProviders()
        {
            var results = new List<SettingsProvider>();
            results.Add(CreateCategory());
            foreach (var setting in ModuleSettingsEditorManager.GetAll())
            {
                var type = setting.GetType();
                results.Add(Create(type));
            }
            return results.ToArray();
        }

        private static SettingsProvider CreateCategory()
        {
            return new SettingsProvider($"Project/{CATEGORY}", SettingsScope.Project)
            {
                label = CATEGORY,
                guiHandler = searchContext =>
                {
                    BeginPadding();

                    EditorGUILayout.HelpBox($"Create a subtype of the {nameof(ModuleSettings)} class to make a single instance of it show up here.\n" +
                        $"Load the settings using {nameof(ModuleSettings)}.{nameof(ModuleSettings.Get)}<MyClassName>();", MessageType.Info);

                    EndPadding();
                },
                keywords = new HashSet<string>()
            };
        }

        private static SettingsProvider Create(Type type)
        {
            var title = GetTitle(type);

            return new SettingsProvider($"Project/{CATEGORY}/{title}", SettingsScope.Project)
            {
                label = title,
                guiHandler = searchContext =>
                {
                    BeginPadding();

                    var settings = ModuleSettingsEditorManager.CreateOrGet(type);
                    if (settings)
                    {
                        if (EditorApplication.isPlaying)
                        {
                            EditorGUILayout.HelpBox("Changes made in play mode will not be saved.", MessageType.Info);
                        }

                        cachedEditors.TryGetValue(settings.GetType(), out var editor);
                        Editor.CreateCachedEditor(settings, null, ref editor);
                        cachedEditors[settings.GetType()] = editor;

                        var hasChanges = editor.DrawDefaultInspector();

                        if (hasChanges && !EditorApplication.isPlaying)
                        {
                            ModuleSettingsEditorManager.Save(settings);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Module Settings could not be loaded.", MessageType.Error);
                    }

                    EndPadding();
                },
                keywords = FindKeywords(type)
            };
        }

        private static string GetTitle(Type type)
        {
            var title = type.Name;

            var settings = ModuleSettingsEditorManager.CreateOrGet(type);
            if (settings)
            {
                title = settings.title;
            }

            return title;
        }

        private static void DisallowReferencesToSceneObjects(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference
                && property.objectReferenceValue != null
                && !AssetDatabase.Contains(property.objectReferenceValue))
            {
                property.objectReferenceValue = null;
            }
        }

        private static void BeginPadding()
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            GUILayout.BeginVertical();
        }

        private static void EndPadding()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        
        private static HashSet<string> FindKeywords(Type type)
        {
            var keywords = new HashSet<string>();
            var settings = ModuleSettingsEditorManager.CreateOrGet(type);
            if (settings)
            {
                var serializedSettings = new SerializedObject(settings);
                var property = serializedSettings.GetIterator();
                property.NextVisible(true); // Skip the script field
                while (property.NextVisible(true))
                {
                    keywords.Add(property.name);
                }
            }

            return keywords;
        }
    }
}
