using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneScriptableObject", menuName = "ScriptableObject/SceneScriptableObject", order = 0)]
public class SceneScriptableObject : ScriptableObject
{
    [SerializeField] SceneReference sceneReference;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneReference.SceneName);
    }
}
