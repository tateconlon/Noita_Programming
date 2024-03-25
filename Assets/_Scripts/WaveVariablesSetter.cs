using ThirteenPixels.Soda;
using UnityEngine;

public class WaveVariablesSetter : MonoBehaviour
{
    [SerializeField] private Stage curStage;
    [SerializeField] private ScopedVariable<float> waveDuration;
    [SerializeField] private ScopedVariable<float> stageClock;
    [SerializeField] private ScopedVariable<int> curWaveIndex;

    private void OnEnable()
    {
        Wave curWave = curStage.GetWave(curWaveIndex);
        
        waveDuration.value = curWave.duration;
        
        stageClock.value = waveDuration.value;
    }
}