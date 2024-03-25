using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MoveOnVector2Input : MonoBehaviour
{
    [SerializeField, Required]
    private InputActionReference _inputAction;
    
    [SerializeField, Required]
    private InputActionReference _shootingAction;

    [SerializeField, Required]
    private CharacterController2D _controller;
    
    [SerializeField] private float _runSpeed = 3.5f;
    
    private void Update()
    {
        Vector2 moveInput = Vector2.ClampMagnitude(_inputAction.action.ReadValue<Vector2>(), 1.0f);
        float speed = _runSpeed;
        if (_shootingAction != null && _shootingAction.action.IsPressed())
        {
            speed /= 2f;
        }
        
        Vector2 motion = speed * Time.deltaTime * moveInput;
        
        _controller.Move(motion);
    }
}