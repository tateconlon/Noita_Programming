using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MMF_Player))]
public class PlayFxOnHealthEvent : MonoBehaviour
{
    [SerializeField, Required] private Health _health;
    [SerializeField] private HealthEvent _event = HealthEvent.Damaged;
    
    [InfoBox("The absolute value of the health delta will be passed as the effect intensity")]
    [SerializeField, Required] private MMF_Player _feedbacks;
    
    private bool _isFirstOnEnable = true;
    
    private void OnEnable()
    {
        switch (_event)
        {
            case HealthEvent.Damaged:
            case HealthEvent.Healed:
                _health.OnHealthChanged += OnHealthChanged;
                break;
            case HealthEvent.Died:
                _health.OnDeath += OnDeath;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (_isFirstOnEnable)
        {
            _isFirstOnEnable = false;
        }
        else
        {
            _feedbacks.RestoreInitialValues();
        }
    }
    
    private void OnHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (healthChangedParams.HealthDeltaUnclamped == 0f) return;
        
        if (healthChangedParams.HealthDeltaUnclamped > 0f)
        {
            if (_event == HealthEvent.Healed)
            {
                _feedbacks.PlayFeedbacks(_health.transform.position, healthChangedParams.HealthDeltaUnclamped);
            }
        }
        else if (_event == HealthEvent.Damaged)
        {
            _feedbacks.PlayFeedbacks(_health.transform.position, Mathf.Abs(healthChangedParams.HealthDeltaUnclamped));
        }
    }
    
    private void OnDeath(Health.HealthChangedParams healthChangedParams)
    {
        _feedbacks.PlayFeedbacks(_health.transform.position, Math.Min(1, Mathf.Abs(healthChangedParams.HealthDeltaUnclamped)));
    }
    
    private void OnDisable()
    {
        switch (_event)
        {
            case HealthEvent.Damaged:
            case HealthEvent.Healed:
                _health.OnHealthChanged -= OnHealthChanged;
                break;
            case HealthEvent.Died:
                _health.OnDeath -= OnDeath;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Reset()
    {
        _feedbacks = GetComponent<MMF_Player>();
    }
    
    private enum HealthEvent
    {
        Damaged,
        Healed,
        Died
    }
}