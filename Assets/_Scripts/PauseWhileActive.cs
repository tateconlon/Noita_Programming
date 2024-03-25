using UnityEngine;

public class PauseWhileActive : MonoBehaviour
{
    private void OnEnable()
    {
        PauseManager.IsPaused.Increment();
    }
    
    private void OnDisable()
    {
        PauseManager.IsPaused.Decrement();
    }
}