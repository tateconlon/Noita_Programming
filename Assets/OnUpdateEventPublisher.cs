using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnUpdateEventPublisher : MonoBehaviour
{
    [SerializeField] UnityEvent _Update;
    [SerializeField] UnityEvent<float> _UpdateDeltaTime;

    void Update()
    {
        _Update.Invoke();
        _UpdateDeltaTime.Invoke(Time.deltaTime);
    }
}
