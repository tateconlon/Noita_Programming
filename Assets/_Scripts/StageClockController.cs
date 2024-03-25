using System;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class StageClockController : MonoBehaviour
{
    public ScopedVariable<float> stageClock;
    
    [Tooltip("Other GlobalFloats that should be increased along with the stageClock but not cleared")]
    public List<GlobalFloat> persistentClockTrackers;

    void OnEnable()
    {
        Clear();
    }

    public void Clear()
    {
        stageClock.value = 0f;
    }

    void Update()
    {
        float clockDelta = Time.deltaTime;
        
        stageClock.value += clockDelta;

        foreach (GlobalFloat persistentClockTracker in persistentClockTrackers)
        {
            persistentClockTracker.value += clockDelta;
        }
    }
}
