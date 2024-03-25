using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Timer))]
public class DodgeRollOnInput : MonoBehaviour
{
    [SerializeField, Required]
    private InputActionReference _moveInput;
    
    [SerializeField, Required]
    private InputActionReference _dodgeInput;
    
    [SerializeField, Required]
    private CharacterController2D _controller;
    
    [SerializeField, Required]
    private AttributeComponent _dashAttribute;
    
    [SerializeField, Required] private Actor _actor;
    [SerializeField] private Ability _costAbility;
    
    [SerializeField, Required]
    private Timer _dodgeTimer;
    
    [SerializeField] private AnimationCurve _speedCurve;
    
    [SerializeField] private MMF_Player _dodgeStartFx;
    [SerializeField] private MMF_Player _dodgeEndFx;
    
    private Vector2 _dodgeDirection;
    
    public bool IsDodging { get; private set; } = false;
    
    public event Action DodgeStarted;
    public event Action DodgeEnded;
    public event Action<bool> DodgeChanged; 
    
    private void OnEnable()
    {
        _dodgeInput.action.performed += OnDodgeInput;
    }
    
    private void OnDodgeInput(InputAction.CallbackContext callbackContext)
    {
        if (IsDodging) return;
        Vector2 moveInput = _moveInput.action.ReadValue<Vector2>();
        
        if (Mathf.Approximately(moveInput.magnitude, 0f)) return;  // Don't roll while standing still
        if (_dashAttribute.curValue < 1.0f) return;  // Need at least one dash
        
        StartDodge(moveInput.normalized);
    }
    
    private void StartDodge(Vector2 moveInputNormalized)
    {
        IsDodging = true;
        _dodgeDirection = moveInputNormalized;
        
        _moveInput.action.Disable();
        
        _dodgeTimer.Duration = _speedCurve.Duration();
        
        //_actor.Abilities.TryActivateAbility(_costAbility, new TargetData(_actor));
        
        _dodgeTimer.OnTimeout += EndDodge;
        _dodgeTimer.Restart();
        
        DodgeStarted?.Invoke();
        DodgeChanged?.Invoke(true);
        _dodgeStartFx.PlayFeedbacks();
    }
    
    private void Update()
    {
        if (!IsDodging) return;
        
        Vector2 motion = Time.deltaTime * _speedCurve.Evaluate(_dodgeTimer.TimeElapsed) * _dodgeDirection;
        _controller.Move(motion);
    }
    
    private void EndDodge()
    {
        IsDodging = false;
        
        _dodgeTimer.OnTimeout -= EndDodge;
        
        _moveInput.action.Enable();
        
        DodgeEnded?.Invoke();
        DodgeChanged?.Invoke(false);
        _dodgeEndFx.PlayFeedbacks();
    }
    
    private void OnDisable()
    {
        _dodgeInput.action.performed -= OnDodgeInput;
        
        EndDodge();
    }
    
    private void Reset()
    {
        _dodgeTimer = GetComponent<Timer>();
    }
}