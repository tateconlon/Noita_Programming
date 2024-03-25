using MoreMountains.Feedbacks;
using UnityEngine;

public static class PauseManager
{
    public static readonly CounterBool IsPaused = new(false);
    
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        IsPaused.OnChangeValue -= OnChangeIsPaused;
        IsPaused.OnChangeValue += OnChangeIsPaused;
    }
    
    private static void OnChangeIsPaused(bool isPaused)
    {
        if (isPaused)
        {
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 0f, true);
        }
        else
        {
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
        }
    }
}
