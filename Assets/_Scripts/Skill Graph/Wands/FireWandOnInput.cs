using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireWandOnInput : MonoBehaviour
{
    [SerializeField, Required] private InputActionReference _fireInput;
    [SerializeField] private WandV2 _wand;
    
    public static event Action OnWandFired;
    
    private void OnEnable()
    {
        _fireInput.action.performed += OnFireInput;
        
        _wand.OnReadyToCast += OnWandReady;
    }
    
    private void OnFireInput(InputAction.CallbackContext callbackContext)
    {
        // if (!PauseManager.IsPaused.Value && !PointerManager.IsOverUi && !MouseInventorySlotV2.main.isHolding)
        // {
        //     //Fire();
        // }
    }
    
    private void OnWandReady()
    {
        if (_fireInput.action.IsPressed())  // For full-auto firing style
        {
            //Fire();
        }
    }

    private void Update()
    {
        if (PauseManager.IsPaused.Value 
            || !(GameManager.Instance.WaveCombat.IsActive || GameManager.Instance.WaveCountdown.IsActive || GameManager.Instance.Testing.IsActive)
            || Camera.main == null)
        {
            return;
        }
        
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        Vector2 aimDirection = targetPos - (Vector2)transform.position;
        aimDirection.Normalize();
        
        bool success = _wand.TryCast(aimDirection);
        
        // if (success)
        // {
        //     OnWandFired?.Invoke();
        // }
    }
    
    private void OnDisable()
    {
        _fireInput.action.performed -= OnFireInput;
        
        _wand.OnReadyToCast -= OnWandReady;
    }
}