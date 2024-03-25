using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetActiveOnInputAction : MonoBehaviour
{
    [SerializeField, Required]
    private InputActionReference _inputAction;
    
    [SerializeField, Required, ChildGameObjectsOnly(IncludeSelf = false)]
    private GameObject _target;
    
    [SerializeField] private ActivateBehavior _activateBehavior = ActivateBehavior.Toggle;
    [SerializeField] private bool _deactivateOnAwake = true;
    
    private void Awake()
    {
        if (_deactivateOnAwake)
        {
            _target.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        PlayerInputStaticEvents.OnAnyInput += OnInput;
    }
    
    private void OnInput(PlayerInput playerInput, InputAction.CallbackContext callbackContext)
    {
        // Doesn't do anything with the playerInput, but could use it to filter out only actions from a certain player
        
        if (callbackContext.action.id != _inputAction.action.id) return;
        if (!callbackContext.performed) return;

        switch (_activateBehavior)
        {
            case ActivateBehavior.Activate:
                _target.SetActive(true);
                break;
            case ActivateBehavior.Deactivate:
                _target.SetActive(false);
                break;
            case ActivateBehavior.Toggle:
                _target.SetActive(!_target.activeSelf);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OnDisable()
    {
        PlayerInputStaticEvents.OnAnyInput -= OnInput;
    }
    
    private enum ActivateBehavior
    {
        Activate,
        Deactivate,
        Toggle
    }
}