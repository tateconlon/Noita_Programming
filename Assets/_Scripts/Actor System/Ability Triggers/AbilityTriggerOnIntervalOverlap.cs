using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Timer))]
public class AbilityTriggerOnIntervalOverlap : AbilityTrigger
{
    [SerializeField, Required] private Timer _timer;
    [SerializeField, Required] private Rigidbody2D _rb2d;
    
    [SerializeField] private bool startImmediately = true;
    
    private void OnEnable()
    {
        _timer.OnTimeout += ActivateOnOverlappingActors;
        
        if (startImmediately)
        {
            ActivateOnOverlappingActors();
        }
        
        _timer.Restart();
    }
    
    private void ActivateOnOverlappingActors()
    {
        ContactFilter2D contactFilter = new ContactFilter2D()
        {
            useTriggers = true
        };
        
        List<Collider2D> results = new();
        TargetData targetData = new();
        
        _rb2d.OverlapCollider(contactFilter, results);
        
        foreach (Collider2D result in results)
        {
            if (!result.TryGetComponent(out ActorHurtbox target)) continue;
            
            targetData.Targets.Add(target.Owner);
        }
        
        ActivateAbilities(targetData);
    }
    
    protected virtual void OnDisable()
    {
        _timer.OnTimeout -= ActivateOnOverlappingActors;
    }

    protected override void Reset()
    {
        base.Reset();
        
        _timer = GetComponent<Timer>();
        _rb2d = GetComponent<Rigidbody2D>();
    }
}