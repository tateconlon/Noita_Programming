using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Assertions;

// https://forum.unity.com/threads/burstdebuginformation_donotship-in-builds.1172273/#post-8103611
internal class DeleteBurstDebugInfoOnBuild : IPostprocessBuildWithReport
{
    public int callbackOrder => -1;

    public void OnPostprocessBuild(BuildReport report)
    {
        //if (report.summary.result != BuildResult.Succeeded) return;  // Result is "Unknown" by design, don't check
        
        string outputPath = report.summary.outputPath;
 
        try
        {
            string applicationName = Path.GetFileNameWithoutExtension(outputPath);
            string outputFolder = Path.GetDirectoryName(outputPath);
            Assert.IsNotNull(outputFolder);
 
            outputFolder = Path.GetFullPath(outputFolder);
 
            string burstDebugInformationDirectoryPath = Path.Combine(outputFolder, $"{applicationName}_BurstDebugInformation_DoNotShip");
 
            if (Directory.Exists(burstDebugInformationDirectoryPath))
            {
                Debug.Log($" > Deleting Burst debug information folder at path '{burstDebugInformationDirectoryPath}'...");
 
                Directory.Delete(burstDebugInformationDirectoryPath, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"An unexpected exception occurred while performing build cleanup: {e}");
        }
    }
}