using Sirenix.OdinInspector;
using UnityEngine;

// https://github.com/tranek/GASDocumentation#concepts-ge-spec

public abstract class EffectInstance
{
    public Actor Owner { get; private set; }
    public Actor Target { get; private set; }
    public AbilityInstance Ability { get; private set; }
    
    protected void Init(AbilityInstance abilityInstance, Actor target)
    {
        Owner = abilityInstance.Owner;
        Target = target;
        Ability = abilityInstance;
    }
}

public class InstantEffectInstance : EffectInstance
{
    public InstantEffect type { get; private set; }
    
    public void ExecuteOnTarget(InstantEffect instantEffect, AbilityInstance ability, Actor target)
    {
        Init(ability, target);
        
        type = instantEffect;
        
        //Target.Attributes.ExecuteEffect(this);
    }
}

public class DurationEffectInstance : EffectInstance
{
    public DurationEffect type { get; private set; }
    
    [ShowInInspector]
    public float Duration { get; private set; }

    public bool IsExpired => TimeElapsed >= Duration;
    
    public float TimeElapsed { get; private set; } = 0f;
    public float TimeElapsedNormalized => Mathf.InverseLerp(0f, Duration, TimeElapsed);
    
    [ShowInInspector, ProgressBar(0, nameof(Duration))]
    public float TimeRemaining => Duration - TimeElapsed;
    public float TimeRemainingNormalized => 1.0f - TimeElapsedNormalized;
    
    private float _lastApplyTime = float.NegativeInfinity;
    
    public void AddToTarget(DurationEffect durationEffect, AbilityInstance ability, Actor target, float duration)
    {
        Init(ability, target);
        
        type = durationEffect;
        
        //Target.Attributes.AddEffect(this);
        
        Duration = duration;
        TimeElapsed = 0f;

        // Determine our starting behavior for when we begin to try applying on interval in Update
        _lastApplyTime = type.periodicEffects.waitOnePeriod ? Time.time : float.NegativeInfinity;
    }
    
    public void AddDurationStack(float duration)
    {
        float maxDurationToAdd = type.stackingInfo.maxStackedDuration - TimeRemaining;
        
        // Ensure that the time remaining on an Effect never exceeds its max duration
        Duration += Mathf.Min(duration, maxDurationToAdd);
    }
    
    public void Update()
    {
        TimeElapsed += Time.deltaTime;
        
        TryApplyPeriodicEffects();
    }
    
    private bool TryApplyPeriodicEffects()
    {
        if (Time.time < _lastApplyTime + type.periodicEffects.period) return false;
        
        foreach (PeriodicEffect periodicEffect in type.periodicEffects.effects)
        {
            if (!periodicEffect.CanApply(Target) || !RandomExtensions.Bool(periodicEffect.successChance)) continue;
            
            InstantEffectInstance effectInstance = new();
            effectInstance.ExecuteOnTarget(periodicEffect, Ability, Target);
        }
        
        _lastApplyTime = Time.time;
        
        return true;
    }
    
    public void RemoveFromTarget()
    {
        //Target.Attributes.RemoveEffect(this);
    }
}