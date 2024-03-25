using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModifyInputsWhileActive : MonoBehaviour
{
    [SerializeField] private List<InputActionReference> _enableWhileEnabled;
    [SerializeField] private List<InputActionReference> _disableWhileEnabled;
    
    private void OnEnable()
    {
        foreach (InputActionReference inputActionReference in _enableWhileEnabled)
        {
            inputActionReference.action.actionMap.Enable();
            inputActionReference.action.Enable();
        }
        
        foreach (InputActionReference inputActionReference in _disableWhileEnabled)
        {
            inputActionReference.action.Disable();
        }
    }
    
    private void OnDisable()
    {
        foreach (InputActionReference inputActionReference in _enableWhileEnabled)
        {
            inputActionReference.action.Disable();
        }
        
        foreach (InputActionReference inputActionReference in _disableWhileEnabled)
        {
            inputActionReference.action.Enable();
        }
    }
}