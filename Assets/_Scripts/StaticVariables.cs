// DATA class. Allows for global access to data ScriptableObjects at runtime.

using System.Collections.Generic;

public static class D
{
    // Assign these ScriptableObjects to the DataManager prefab in the editor,
    // then set them in DataManager.Awake()
    
    public static AudioVariables au;
    public static BuildVariables bv;
    public static DebugVariables dv;
    public static GameplayVariables gv;
    public static VisualVariables vv;
}

// MANAGERS class. Allows for global access to manager singletons at runtime.
public static class M
{
    // Put the code below at the start of the Awake method for every manager class:
    //
    // if (!M.managerNameHere)
    // {
    //     M.managerNameHere = this;
    //     // DontDestroyOnLoad(gameObject);  // Optionally use this to persist across scene changes
    // }
    // else
    // {
    //     Debug.LogError($"There is already an instance of {GetType().Name}");
    //     gameObject.SetActive(false);  // ReSharper disable once Unity.InefficientPropertyAccess
    //     Destroy(gameObject);
    //     return;
    // }

    public static AudioManager am;
    public static DataManager dm;
    public static DebugManager dbg;
}