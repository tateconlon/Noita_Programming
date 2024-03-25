using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

// This class could be expanded to do things like:
// - manage which input actions are enabled/disabled based on if any other components want them enabled/disabled
// - activate multiple action maps by default on startup (PlayerInput can only choose one)
// - manage input blocking so that key presses meant for some things don't trigger others with less priority

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputStaticEvents : MonoBehaviour
{
    // Thoughts on use cases for modifying input:
    // disable action map while something is active (e.g. UI panel), a tag is on actor (e.g. stunned),
    // game is in state (e.g. waiting for match start), player is in state (e.g. driving car)
    
    [SerializeField, Required]
    private PlayerInput _playerInput;
    
    public delegate void InputDelegate(PlayerInput playerInput, InputAction.CallbackContext callbackContext);
    
    public static event InputDelegate OnAnyInput;
    
    private void OnEnable()
    {
        Debug.Assert(_playerInput.notificationBehavior == PlayerNotifications.InvokeCSharpEvents, this);
        
        _playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext callbackContext)
    {
        OnAnyInput?.Invoke(_playerInput, callbackContext);
    }

    private void OnDisable()
    {
        _playerInput.onActionTriggered -= OnActionTriggered;
    }

    private void Reset()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
}