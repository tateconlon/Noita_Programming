using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// An ability that has requirements and takes time to use,
// e.g. casting a spell that requires mana, has cooldown, takes time to cast, can be interrupted, etc.
// https://docs.unrealengine.com/5.2/en-US/using-gameplay-abilities-in-unreal-engine/
// https://github.com/sjai013/unity-gameplay-ability-system/wiki/A-simple-ability

[Serializable]
public class Ability
{
    [Header("Owner Effects")]
    public List<InstantEffect> ownerInstantEffects;
    public DurationEffect.EffectToDurationDict ownerDurationEffects;
    public GlobalDurationEffect.EffectToDurationDict ownerGlobalDurationEffects;
    
    [Header("Target Effects")]
    public List<InstantEffect> targetInstantEffects;
    public DurationEffect.EffectToDurationDict targetDurationEffects;
    public GlobalDurationEffect.EffectToDurationDict targetGlobalDurationEffects;
    
    // TODO: add more tag-related fields from UE docs
    public Tag.RequiredAndBlockedTags activationOwnerTags;
    public Tag.RequiredAndBlockedTags activationTargetTags;
    
    public bool CanActivate(Actor owner, [CanBeNull] Actor target)
    {
        return activationOwnerTags.IsSatisfiedBy(owner) && 
               activationTargetTags.IsSatisfiedBy(target);
    }
}

//Might need empty IAbility so you can recieve abilities
public class PortalAbility
{
    //Fields

    public PortalAbility()
    {
        
    }
    
    public bool CanActivate()
    {
        return true;
    }
    
    public void Activate(Actor owner, Actor target)
    {
        
    }
} 

/*

public class Bomb
{
    
    void OnHitRecieved(list)
    {
    // some filtering
        BombAbility bomb = new();
        if(bomb.CanActivate(list[i]))
        {
            bomb.Activate(list[i]);
        }
    
    }
}






*/