using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This wraps common static methods in a UnityEngine Object that can be referenced by UnityEvents,
/// which enables calling static methods through UnityEvents
/// </summary>
[CreateAssetMenu(fileName = "UnityEventStaticMethodWrapper", menuName = "ScriptableObject/UnityEventStaticMethodWrapper", order = 0)]
public class UnityEventStaticMethodWrapper : ScriptableObject
{
    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void ReloadActiveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DebugLog(string message)
    {
        Debug.Log(message);
    }
    
    public void DebugLogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    
    public void DebugLogError(string message)
    {
        Debug.LogError(message);
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}
