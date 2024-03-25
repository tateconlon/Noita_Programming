using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ActorEffectReceiver : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    [ShowInInspector, NonSerialized, ReadOnly] private DurationEffect.DurationEffectDict _durationEffects = new();
    
    public bool TryReceiveInstantEffect(InstantEffect instantEffect, AbilityInstance abilityInstance)
    {
        if (!instantEffect.CanApply(_actor) || !RandomExtensions.Bool(instantEffect.successChance)) return false;
        
        EffectModifyTags(instantEffect);
        
        InstantEffectInstance effectInstance = new();
        effectInstance.ExecuteOnTarget(instantEffect, abilityInstance, _actor);
        
        return true;
    }

    public bool TryReceiveDurationEffect(DurationEffect durationEffect, AbilityInstance abilityInstance, float duration)
    {
        if (!durationEffect.CanApply(_actor) || !RandomExtensions.Bool(durationEffect.successChance)) return false;

        List<DurationEffectInstance> effectsOfType = _durationEffects.GetOrCreate(durationEffect);
        
        if (effectsOfType.Count == 0 || durationEffect.stackingInfo.stackingMode == DurationEffect.StackingInfo.StackingMode.Unique)
        {
            DurationEffectInstance effectInstance = new();
            effectInstance.AddToTarget(durationEffect, abilityInstance, _actor, duration);
            
            effectsOfType.Add(effectInstance);

            EnforceMaxNumStacks(durationEffect);
        }
        else
        {
            effectsOfType[0].AddDurationStack(duration);
        }
        
        EffectModifyTags(durationEffect);
        
        return true;
    }
    
    private void EnforceMaxNumStacks(DurationEffect effect)
    {
        int numStacksOverMax = _durationEffects[effect].Count - effect.stackingInfo.maxNumStacks;
        
        for (int i = 0; i < numStacksOverMax; i++)
        {
            RemoveEffectInternal(effect, 0);  // Remove oldest effects first
        }
    }
    
    public bool TryReceiveGlobalDurationEffect(GlobalDurationEffect globalDurationEffect, AbilityInstance abilityInstance, float duration)
    {
        // TODO: do something like show icon for global duration effect in UI in response to this
        
        return TryReceiveDurationEffect(globalDurationEffect.effect, abilityInstance, duration);
    }
    
    private void EffectModifyTags(Effect effect)
    {
        foreach (Tag grantedTag in effect.tagInfo.grantedTags)
        {
           // _actor.Tags.Add(grantedTag);
        }
        
        foreach (Tag removedTag in effect.tagInfo.removedTags)
        {
            //_actor.Tags.Remove(removedTag);
        }
    }
    
    private void Update()
    {
        UpdateDurationEffects();
    }
    
    private void UpdateDurationEffects()
    {
        foreach ((DurationEffect effect, List<DurationEffectInstance> instances) in _durationEffects)
        {
            for (int i = instances.Count - 1; i >= 0; i--)
            {
                DurationEffectInstance instance = instances[i];
                
                instance.Update();

                if (instance.IsExpired)
                {
                    RemoveEffectInternal(effect, i);
                }
            }
        }
    }
    
    // TODO: Not sure how we'll want to remove effects - by type, by instance, oldest instance, newest instance, etc? 
    public bool TryRemoveEffect(DurationEffectInstance effectInstance)
    {
        throw new NotImplementedException();
    }
    
    private void RemoveEffectInternal(DurationEffect effectToRemove, int index)
    {
        _durationEffects[effectToRemove][index].RemoveFromTarget();
        _durationEffects[effectToRemove].RemoveAt(index);
        
        foreach (Tag tagToRemove in effectToRemove.tagInfo.grantedTags)
        {
           // _actor.Tags.Remove(tagToRemove);
        }
        
        foreach (Tag tagToAdd in effectToRemove.tagInfo.removedTags)
        {
            //_actor.Tags.Add(tagToAdd);
        }
    }
    
    private void OnDisable()
    {
        // Only need this because AttributeComponents will remove all modifiers and ActorTagSet will remove all tags
        _durationEffects.Clear();
    }
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
}
