using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PressButtonOnInputAction : MonoBehaviour
{
    [SerializeField, Required]
    private Button _button;
    
    [SerializeField, Required]
    private InputActionReference _inputAction;
    
    private void OnEnable()
    {
        PlayerInputStaticEvents.OnAnyInput += OnInput;
    }
    
    private void OnInput(PlayerInput playerInput, InputAction.CallbackContext callbackContext)
    {
        // Doesn't do anything with the playerInput, but could use it to filter out only actions from a certain player
        
        if (callbackContext.action.id != _inputAction.action.id) return;
        if (!callbackContext.performed) return;
        
        _button.SimulateClick();
    }
    
    private void OnDisable()
    {
        PlayerInputStaticEvents.OnAnyInput -= OnInput;
    }
    
    private void Reset()
    {
        _button = GetComponent<Button>();
    }
}