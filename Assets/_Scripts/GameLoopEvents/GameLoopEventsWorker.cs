using System;
using UnityEngine;

/// <summary>
/// This worker component always runs before other components and will fire events through GameLoopEventsManager
/// when certain MonoBehaviour/application lifecycle events happen
/// </summary>
[DefaultExecutionOrder(int.MinValue)]  // Needs to run before anything else that could possibly depend on its events
public class GameLoopEventsWorker : MonoBehaviour
{
    public event Action OnApplicationQuitEvent;
    
    void OnApplicationQuit()
    {
        OnApplicationQuitEvent?.Invoke();
    }
}
