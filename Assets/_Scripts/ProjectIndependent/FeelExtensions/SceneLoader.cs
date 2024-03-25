using DevLocker.Utils;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool _runOnStart;
    
    [Header("Parameters")]
    [SerializeField, Required] private SceneReference _sceneToLoad;
    [SerializeField, Required] private SceneReference _loadingScreenScene;
    [SerializeField] private MMAdditiveSceneLoadingManagerSettings _loadSettings;
    
    private void Start()
    {
        if (_runOnStart)
        {
            Load();
        }
    }
    
    [Button]
    public void Load()
    {
        if (!Application.isPlaying) return;
        
        Debug.Assert(_sceneToLoad != null && _loadingScreenScene != null);
        
        MMAdditiveSceneLoadingManager.LoadScene(_sceneToLoad.SceneName, _loadSettings);
    }

    private void OnValidate()
    {
        if (_loadingScreenScene != null && _loadSettings != null)
        {
            _loadSettings.LoadingSceneName = _loadingScreenScene.SceneName;
        }
    }
}
