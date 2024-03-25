using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class AbilityTrigger : MonoBehaviour
{
    [SerializeField] protected Actor _actor;
    [SerializeField] protected List<AbilityReference> _abilities;
    [Range(0f, 1f)]
    [SerializeField] private float _chance = 1.0f;  // TODO: should be somewhere else, but works for now
    
    // TODO: modifiers to pass along to abilities, e.g. force them to crit/not crit, modify chance of activation, damage
    
    private void Start()
    {
        if (_actor != null)
        {
            // Just going to grant the actor all the abilities too since they're only activated by trigger
            foreach (AbilityReference abilityRef in _abilities)
            {
                //_actor.Abilities.AddAbility(abilityRef.Value);
            }
        }
    }
    
    protected void ActivateAbilities(TargetData targetData)
    {
        if (!RandomExtensions.Bool(_chance)) return;
        
        foreach (AbilityReference abilityRef in _abilities)
        {
            //_actor.Abilities.TryActivateAbility(abilityRef.Value, targetData);
        }
    }
    
    protected virtual void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
}