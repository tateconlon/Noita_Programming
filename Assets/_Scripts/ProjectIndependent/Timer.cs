using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float Duration = 1.0f;
    public bool Loop = false;
    public bool UseUnscaledTime = false;
    
    public bool IsTimedOut => !enabled || TimeElapsed >= Duration;
    
    public float TimeElapsed { get; private set; } = 0f;
    public float TimeElapsedNormalized => Mathf.InverseLerp(0f, Duration, TimeElapsed);
    
    [ShowInInspector, ProgressBar(0, nameof(Duration))]
    public float TimeRemaining => Duration - TimeElapsed;
    public float TimeRemainingNormalized => 1.0f - TimeElapsedNormalized;
    
    public event Action OnTimeout;
    
    public void Restart()
    {
        TimeElapsed = 0f;
        enabled = true;
    }
    
    private void OnEnable()
    {
        Restart();
    }
    
    private void Update()
    {
        TimeElapsed += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        
        if (TimeElapsed >= Duration)
        {
            Timeout();
        } 
    }
    
    private void Timeout()
    {
        if (Loop)
        {
            OnTimeout?.Invoke();  // Invoke OnTimeout first so that IsTimedOut will be true while responses run
            Restart();
        }
        else
        {
            enabled = false;
            OnTimeout?.Invoke();  // Invoke OnTimeout last so an OnTimeout response can re-enable this Timer
        }
    }
}