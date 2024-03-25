using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//This class should not be used for continuous firing as 
//we reset to 0 so errors accumulate
public class EventTimerSODA : MonoBehaviour
{
    [Header("Timer Logic")]
    [SerializeField] 
    ScopedVariable<float> duration;
    [SerializeField] 
    ScopedVariable<bool> runOnStart;

    [Header("View Model")]
    [SerializeField] 
    ScopedVariable<bool> isRunning;
    [SerializeField]
    ScopedVariable<float> elapsedTime;
    [SerializeField]
    ScopedVariable<float> remainingTime;
    [SerializeField] 
    ScopedVariable<float> completionPercent;
    [SerializeField] 
    ScopedVariable<float> reverseCompletionPercent;

    [Header("Timer Behaviour")]
    public ScopedVariable<float> multiple;    //Frequency of timerIntermediaetEvent
    public ScopedVariable<float> offset;      //Offset from frequency of timerIntermediateEvent
    public UnityEvent timerIntermediateEvent;
    public UnityEvent timerDoneEvent;

    void Start()
    {
        isRunning.value = runOnStart;
    }

    void Update()
    {
        
        if(isRunning.value) //Refactor: TRICKY!!! if(isRunning) checks null, not the casted bool value
        {
            float prevElapsedTime = elapsedTime;
            elapsedTime.value += Time.deltaTime;
            remainingTime.value = duration - elapsedTime;
            completionPercent.value = Mathf.Clamp01(elapsedTime / duration);
            reverseCompletionPercent.value = 1 - completionPercent;

            float prevInterval = (prevElapsedTime + offset) % multiple;
            float currInterval = (elapsedTime + offset) % multiple;
            
            if (currInterval <= prevInterval)
            {
                timerIntermediateEvent.Invoke();
            }

            if (elapsedTime >= duration)
            {
                timerDoneEvent.Invoke();
                StopTimer();
            }
        }
    }

    public void Clear()
    {
        elapsedTime.value = 0;
    }

    public void ClearAndStart()
    {
        Clear();
        Resume();
    }

    public void Resume()
    {
        isRunning.value = true;
    }
   
    public void StopTimer()
    {
        isRunning.value = false;
    }

    public void OnIsRunningChanged(bool isRunning)
    {
        //nothing
    }

    public void SetDuration(float newDur)
    {
        duration.value = newDur;
    }
}