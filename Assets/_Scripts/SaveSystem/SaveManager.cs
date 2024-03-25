using ThirteenPixels.Soda;
using ThirteenPixels.Soda.ModuleSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    static SaveSettings _saveSettings;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        _saveSettings = ModuleSettings.Get<SaveSettings>();
        
        SavestateReaderWriterPlayerPrefs readerWriter = new();
        SavestateSettings.defaultReader = readerWriter;
        SavestateSettings.defaultWriter = readerWriter;
        
        LoadGame();

        _saveSettings.requestLoadGame.onRaise.AddResponse(LoadGame);
        _saveSettings.requestSaveGame.onRaise.AddResponse(SaveGame);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.quitting += CleanUp;
    }

    static void LoadGame()
    {
        _saveSettings.activeSavestate.Load();
    }

    static void SaveGame()
    {
        _saveSettings.activeSavestate.Save();
    }
    
    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveGame();  // Save game on scene load as a backup
    }
    
    static void CleanUp()
    {
        _saveSettings.requestLoadGame.onRaise.RemoveResponse(LoadGame);
        _saveSettings.requestSaveGame.onRaise.RemoveResponse(SaveGame);
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= CleanUp;
        
        SaveGame();  // Save game before the application quits
    }
}
