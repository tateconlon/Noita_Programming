using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PointerManager : MonoBehaviour
{
    public static Vector2 PointerPosition => Mouse.current.position.ReadValue();
    public static bool IsOverUi { get; private set; } = false;
    public static event Action<bool> OnChangeIsOverUi;
    
    private readonly List<RaycastResult> _raycastResults = new();
    
    private void Update()
    {
        bool isOverUi = false;
        
        if (EventSystem.current != null)
        {
            PointerEventData pointerEventData = new(EventSystem.current)
            {
                position = PointerPosition
            };
            
            EventSystem.current.RaycastAll(pointerEventData, _raycastResults);
            
            isOverUi = _raycastResults.Count > 0;
        }
        
        if (isOverUi != IsOverUi)
        {
            IsOverUi = isOverUi;
            OnChangeIsOverUi?.Invoke(IsOverUi);
        }
    }
    
    private void OnDisable()
    {
        IsOverUi = false;
        OnChangeIsOverUi?.Invoke(IsOverUi);
    }
}