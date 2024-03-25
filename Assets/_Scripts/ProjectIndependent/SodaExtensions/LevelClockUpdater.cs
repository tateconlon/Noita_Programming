using ThirteenPixels.Soda;
using UnityEngine;

public class LevelClockUpdater : MonoBehaviour
{
    public ScopedVariable<float> levelClock;
    
    private void OnEnable()
    {
        Clear();
    }

    public void Clear()
    {
        levelClock.value = 0f;
    }

    private void Update()
    {
        levelClock.value += Time.deltaTime;
    }
}