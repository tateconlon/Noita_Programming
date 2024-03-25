using System;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TimerToGlobalFloat : MonoBehaviour
{
    [SerializeField, Required] private Timer _timer;
    [SerializeField, Required] private GlobalFloat _globalFloat;
    [SerializeField] private TargetProperty _targetProperty;
    
    private void Update()
    {
        switch (_targetProperty)
        {
            case TargetProperty.TimeElapsed:
                _globalFloat.value = _timer.IsTimedOut ? 1f : _timer.TimeElapsed;
                break;
            case TargetProperty.TimeElapsedNormalized:
                _globalFloat.value = _timer.IsTimedOut ? 1f : _timer.TimeElapsedNormalized;
                break;
            case TargetProperty.TimeRemaining:
                _globalFloat.value = _timer.IsTimedOut ? 0f : _timer.TimeRemaining;
                break;
            case TargetProperty.TimeRemainingNormalized:
                _globalFloat.value = _timer.IsTimedOut ? 0f : _timer.TimeRemainingNormalized;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Reset()
    {
        _timer = GetComponent<Timer>();
    }
    
    private enum TargetProperty
    {
        TimeElapsed,
        TimeElapsedNormalized,
        TimeRemaining,
        TimeRemainingNormalized
    }
}
