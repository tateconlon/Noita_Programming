using System.Collections.Generic;
using UnityEngine;

public class TriggerOnDamageInjuredManager : SpellTriggerManagerBase<TriggerOnDamageInjuredManager>
{
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyHealthChanged;
    }
    
    private void OnAnyHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (!_targetTrigger.IsAddedToProjectile(healthChangedParams.Attacker, out SpellProjectile spellProjectile)) return;
        if (healthChangedParams.CurHealth >= healthChangedParams.PrevHealth) return;  // Only proceed if damage was done, not healing
        if (Mathf.Approximately(healthChangedParams.PrevHealth, healthChangedParams.MaxHealth)) return;  // Only proceed if actor was not at full health
        
        TargetData targetData = new(healthChangedParams.Victim)
        {
            Origin = healthChangedParams.Victim.transform.position,
            Direction = healthChangedParams.Attacker.transform.up
        };
        
        if (spellProjectile.TriggerCastingBlocks.TryGetValue(_targetTrigger, out List<CastingBlock> onDmgInjuredCastingBlocks))
        {
            foreach (CastingBlock onDmgInjuredCastingBlock in onDmgInjuredCastingBlocks)
            {
                onDmgInjuredCastingBlock.Cast(targetData);
            }
        }
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyHealthChanged;
    }
}