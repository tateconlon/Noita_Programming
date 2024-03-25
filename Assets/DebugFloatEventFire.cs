using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class DebugFloatEventFire : MonoBehaviour
{
    public UnityEvent<float> FloatEvent;
    
    public float value;

    [Button]
    private void InvokeFloatEvent()
    {
        FloatEvent.Invoke(value);
    }
    
    
}
