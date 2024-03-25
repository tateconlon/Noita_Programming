using ThirteenPixels.Soda.ModuleSettings;
using UnityEngine;

public static class GameLoopEventsManager
{
    static GameLoopEventsSettings _gameLoopEventsSettings;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        _gameLoopEventsSettings = ModuleSettings.Get<GameLoopEventsSettings>();
        
        Application.quitting += OnApplicationQuit;
    }

    static void OnApplicationQuit()
    {
        Application.quitting -= OnApplicationQuit;
        
        _gameLoopEventsSettings.isApplicationQuitting.value = true;
        _gameLoopEventsSettings.onApplicationQuit.Raise();
    }
}