using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

internal class ZipBuildForSteam : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        //if (report.summary.result != BuildResult.Succeeded) return;  // Result is "Unknown" by design, don't check
        if (report.summary.platform != BuildTarget.StandaloneWindows64 && 
            report.summary.platform != BuildTarget.StandaloneLinux64) return;

        FileInfo outputExe = new FileInfo(report.summary.outputPath);

        StringBuilder zipFileNameSource = new StringBuilder();
        zipFileNameSource.Append(outputExe.Directory.Parent.FullName);
        zipFileNameSource.Append(Path.DirectorySeparatorChar);
        zipFileNameSource.Append(report.summary.platform);
        zipFileNameSource.Append('-');
        
        StringBuilder zipFileNameDest = new StringBuilder();
        zipFileNameDest.Append(outputExe.Directory.FullName);
        zipFileNameDest.Append(Path.DirectorySeparatorChar);
        zipFileNameDest.Append(report.summary.platform);
        zipFileNameDest.Append('-');
        
        // Delete any old zip files in the output directory BEFORE zipping
        foreach (FileInfo fileInfo in outputExe.Directory.EnumerateFiles($"{report.summary.platform}-*.zip"))
        {
            File.Delete(fileInfo.FullName);
        }

        HashSet<char> invalidChars = new HashSet<char>();
        invalidChars.UnionWith(Path.GetInvalidPathChars());
        invalidChars.UnionWith(Path.GetInvalidFileNameChars());
        
        foreach (char bundleVersionChar in PlayerSettings.bundleVersion)
        {
            if (!invalidChars.Contains(bundleVersionChar))
            {
                zipFileNameSource.Append(bundleVersionChar);
                zipFileNameDest.Append(bundleVersionChar);
            }
        }

        zipFileNameSource.Append(".zip");
        zipFileNameDest.Append(".zip");

        ZipFile.CreateFromDirectory(outputExe.Directory.FullName, zipFileNameSource.ToString());
        
        File.Move(zipFileNameSource.ToString(), zipFileNameDest.ToString());
        
        Debug.Log($"Compressed build files to {zipFileNameDest}");
    }
}