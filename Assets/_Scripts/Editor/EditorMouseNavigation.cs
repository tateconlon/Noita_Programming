using System;
using UnityEditor;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public static class EditorMouseNavigation
{
    public static event Action BackButtonPressed;
    public static event Action ForwardButtonPressed;
    
    private static bool _isBackButtonPressed = false;
    private static bool _isForwardButtonPressed = false;
    
    static EditorMouseNavigation()
    {
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;
    }
    
    private static void EditorUpdate()
    {
        if (Mouse.current.backButton.isPressed)
        {
            if (!_isBackButtonPressed)
            {
                _isBackButtonPressed = true;
                
                BackButtonPressed?.Invoke();
            }
        }
        else
        {
            _isBackButtonPressed = false;
        }
        
        if (Mouse.current.forwardButton.isPressed)
        {
            if (!_isForwardButtonPressed)
            {
                _isForwardButtonPressed = true;
                
                ForwardButtonPressed?.Invoke();
            }
        }
        else
        {
            _isForwardButtonPressed = false;
        }
    }
}