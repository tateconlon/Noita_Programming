// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.ModuleSettings.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using System;

    /// <summary>
    /// A pre- and postprocessor that makes sure that <see cref="ModuleSettings"/> instances are correctly added to builds.
    /// </summary>
    internal class ModuleSettingsBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 0;
        private const string BASE_FOLDER = "Assets/" + ModuleSettings.FOLDER_NAME + "/";
        private const string RESOURCES_PATH = BASE_FOLDER + "Resources/" + ModuleSettings.FOLDER_NAME + "/";

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            SerializeAllModuleSettingsToResources();
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            DeleteModuleSettingsResources();
        }

        private void SerializeAllModuleSettingsToResources()
        {
            if (ModuleSettingsEditorManager.settingsCount == 0) return;

            System.IO.Directory.CreateDirectory(RESOURCES_PATH);
            foreach (var setting in ModuleSettingsEditorManager.GetAll())
            {
                try
                {
                    var path = RESOURCES_PATH + setting.GetType().FullName + ".asset";
                    AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate(setting), path);
                    AssetDatabase.ImportAsset(path);
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Settings asset could not be serialized.", e));
                }
            }
        }

        private void DeleteModuleSettingsResources()
        {
            AssetDatabase.DeleteAsset(BASE_FOLDER);
            AssetDatabase.Refresh();
        }
    }
}
