using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

public static class RunWebGLBuild
{
    [MenuItem("Tools/Run Latest WebGL Build")]
    public static void Run()
    {
        string webServerPath = EditorApplication.applicationContentsPath +
                               "/PlaybackEngines/WebGLSupport/BuildTools/SimpleWebServer.exe";

        string webGlBuildPath = EditorUserBuildSettings.GetBuildLocation(BuildTarget.WebGL);

        int port = GetFreeTcpPort();

        Process webServerProcess = new();
        webServerProcess.StartInfo.FileName = webServerPath;
        webServerProcess.StartInfo.Arguments = $"\"{webGlBuildPath}\" {port}";
        webServerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        webServerProcess.Start();
        
        Application.OpenURL($"http://localhost:{port}");
        
        // TODO: could add cleanup code to webServerProcess.Dispose() when quitting editor, etc.
        // (Can't save static reference to webServerProcess that will survive domain reloading, though)
    }
    
    // https://stackoverflow.com/a/150974
    static int GetFreeTcpPort()
    {
        TcpListener l = new(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }
}
