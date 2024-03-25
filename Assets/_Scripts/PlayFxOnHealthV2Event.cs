using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MMF_Player))]
public class PlayFxOnHealthV2Event : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField] private HealthEvent _event = HealthEvent.Damaged;
    
    [InfoBox("The absolute value of the hp delta will be passed as the effect intensity")]
    [SerializeField, Required] private MMF_Player _feedbacks;
    
    private bool _isFirstOnEnable = true;
    
    private void OnEnable()
    {
        switch (_event)
        {
            case HealthEvent.Damaged:
            case HealthEvent.Healed:
                _health.OnHpChanged += OnHealthChanged;
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
    
    private void OnHealthChanged(HealthV2.HpChangeParams hpChangeParams)
    {
        if (hpChangeParams.HpDeltaUnclamped == 0f) return;
        
        float feedbacksIntensity = Mathf.Abs(hpChangeParams.HpDeltaUnclamped);
        
        if (hpChangeParams.HpDeltaUnclamped > 0f)
        {
            if (_event == HealthEvent.Healed)
            {
                _feedbacks.PlayFeedbacks(_health.transform.position, feedbacksIntensity);
            }
        }
        else if (_event == HealthEvent.Damaged)
        {
            _feedbacks.PlayFeedbacks(_health.transform.position, feedbacksIntensity);
        }
    }
    
    private void OnDeath(HealthV2.HpChangeParams hpChangeParams)
    {
        float feedbacksIntensity = Mathf.Abs(hpChangeParams.HpDeltaUnclamped);
        
        _feedbacks.PlayFeedbacks(_health.transform.position, feedbacksIntensity);
    }
    
    private void OnDisable()
    {
        switch (_event)
        {
            case HealthEvent.Damaged:
            case HealthEvent.Healed:
                _health.OnHpChanged -= OnHealthChanged;
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
        _health = GetComponentInParent<HealthV2>();
        _feedbacks = GetComponent<MMF_Player>();
    }
    
    private enum HealthEvent
    {
        Damaged,
        Healed,
        Died
    }
}