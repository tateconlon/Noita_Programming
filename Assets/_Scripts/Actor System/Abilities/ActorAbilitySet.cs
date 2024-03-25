using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ActorAbilitySet : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    [SerializeField] private AbilityHashSet _abilities;
    
    [ShowInInspector, NonSerialized, ReadOnly]
    private Dictionary<Ability, AbilityInstance> _activeAbilities = new();

    [Button]
    public bool CanActivateAbility(Ability ability)
    {
        // if (!_abilities.Contains(ability)) return false;  // TODO: for now ignore and let actors cast any abilities
        
        // TODO: other checks, like can't activate if stunned or already activating another ability
        
        return true;
    }
    
    // TODO: temp, for convenient use within NodeCanvas
    public bool TryActivateAbility(GlobalAbility globalAbility)
    {
        TargetData targetData = new()
        {
            Origin = _actor.transform.position
        };
        
        targetData.Targets.Add(_actor);
        
        return TryActivateAbility(globalAbility.value, targetData);
    }
    
    [Button]
    public bool TryActivateAbility(Ability ability, TargetData targetData)
    {
        if (!CanActivateAbility(ability)) return false;
        
        AbilityInstance abilityInstance = new(ability, _actor);
        
        return abilityInstance.Activate(targetData);
    }
    
    // TODO: method to check what abilities are currently being used/if a given ability is being used?
    
    [Button]
    public bool TryCancelAbility(Ability ability)
    {
        if (!_activeAbilities.TryGetValue(ability, out AbilityInstance abilityInstance)) return false;
        
        return abilityInstance.TryCancel();
    }

    [Button]
    public bool AddAbility(Ability ability)
    {
        return _abilities.Add(ability);
    }

    [Button]
    public bool RemoveAbility(Ability ability)
    {
        return _abilities.Remove(ability);
    }
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
    
    [Serializable] public class AbilityHashSet : UnitySerializedHashSet<Ability> { }
}