using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Timer))]
public class AbilityTriggerOnAimedInput : AbilityTrigger
{
    [SerializeField, Required] private InputActionReference _fireInput;
    [SerializeField, Required] private Timer _timer;
    [SerializeField, Required] private MMF_Player _fireFx;
    
    private void OnEnable()
    {
        _fireInput.action.performed += OnFirePerformed;
        _fireInput.action.canceled += OnFireCanceled;
        _timer.OnTimeout += OnTimeout;
        
        _timer.Restart();
    }
    
    private void OnFirePerformed(InputAction.CallbackContext callbackContext)
    {
        if (!_timer.enabled)
        {
            Activate();
        }
        
        _timer.Loop = true;  // So player can hold to fire, full-auto style
    }
    
    private void OnFireCanceled(InputAction.CallbackContext callbackContext)
    {
        _timer.Loop = false;
    }
    
    private void OnTimeout()
    {
        if (_fireInput.action.IsPressed())  // For full-auto firing style
        {
            Activate();
        }
    }

    private void Activate()
    {
        if (Camera.main == null) return;
        
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        Vector2 aimDirection = targetPos - (Vector2)transform.position;
        
        ActivateAbilities(new TargetData(aimDirection));
        _fireFx.PlayFeedbacks();
        _timer.Restart();
    }
    
    protected virtual void OnDisable()
    {
        _fireInput.action.performed -= OnFirePerformed;
        _fireInput.action.canceled -= OnFireCanceled;
        _timer.OnTimeout -= OnTimeout;
    }
    
    protected override void Reset()
    {
        base.Reset();
        
        _timer = GetComponent<Timer>();
    }
}