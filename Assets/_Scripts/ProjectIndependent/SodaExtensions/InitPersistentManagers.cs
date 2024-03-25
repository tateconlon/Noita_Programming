using ThirteenPixels.Soda.ModuleSettings;
using UnityEngine;

public static class InitPersistentManagers
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        PersistentManagerSettings persistentManagerSettings = ModuleSettings.Get<PersistentManagerSettings>();

        foreach (GameObject persistentManagerPrefab in persistentManagerSettings.persistentManagerPrefabs)
        {
            GameObject persistentManager = Object.Instantiate(persistentManagerPrefab);
            persistentManager.name = persistentManagerPrefab.name;  // To remove "(Clone)" in name
            
            Object.DontDestroyOnLoad(persistentManager);
        }
    }
}
