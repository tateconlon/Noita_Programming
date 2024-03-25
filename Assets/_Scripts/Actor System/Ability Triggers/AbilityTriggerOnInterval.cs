using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class AbilityTriggerOnInterval : AbilityTrigger
{
    [SerializeField, Required] private Timer _timer;
    
    private void OnEnable()
    {
        _timer.OnTimeout += OnTimeout;
        
        _timer.Restart();
    }
    
    private void OnTimeout()
    {
        ActivateAbilities(new TargetData());
    }
    
    protected virtual void OnDisable()
    {
        _timer.OnTimeout -= OnTimeout;
    }
    
    protected override void Reset()
    {
        base.Reset();
        
        _timer = GetComponent<Timer>();
    }
}