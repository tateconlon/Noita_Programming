using System;
using Febucci.UI;
using UnityEngine;
using UnityEngine.Events;

public class TextAnimatorEventListener : MonoBehaviour
{
    [SerializeField] TextAnimator textAnimator;
    
    [Serializable] class StringToUnityEventDict : UnitySerializedDictionary<string, UnityEvent> { }
    [SerializeField] StringToUnityEventDict eventsToResponses = new();

    void OnEnable()
    {
        textAnimator.onEvent += OnEvent;
    }
    
    void OnEvent(string message)
    {
        if (eventsToResponses.ContainsKey(message))
        {
            eventsToResponses[message].Invoke();
        }
        else
        {
            Debug.LogWarning($"Found no response for TextAnimator event '{message}'");
        }
    }
    
    void OnDisable()
    {
        textAnimator.onEvent -= OnEvent;
    }
}
