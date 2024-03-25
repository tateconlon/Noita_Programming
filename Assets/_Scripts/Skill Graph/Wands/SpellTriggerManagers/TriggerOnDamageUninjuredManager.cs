using System.Collections.Generic;
using UnityEngine;

public class TriggerOnDamageUninjuredManager : SpellTriggerManagerBase<TriggerOnDamageUninjuredManager>
{
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyHealthChanged;
    }
    
    private void OnAnyHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (!_targetTrigger.IsAddedToProjectile(healthChangedParams.Attacker, out SpellProjectile spellProjectile)) return;
        if (healthChangedParams.CurHealth >= healthChangedParams.PrevHealth) return;  // Only proceed if damage was done, not healing
        if (!Mathf.Approximately(healthChangedParams.PrevHealth, healthChangedParams.MaxHealth)) return;  // Only proceed if actor was at full health
        
        TargetData targetData = new(healthChangedParams.Victim)
        {
            Origin = healthChangedParams.Victim.transform.position,
            Direction = healthChangedParams.Attacker.transform.up
        };

        if (spellProjectile.TriggerCastingBlocks.TryGetValue(_targetTrigger, out List<CastingBlock> onDmgUninjuredCastingBlocks))
        {
            foreach (CastingBlock onDmgUninjuredCastingBlock in onDmgUninjuredCastingBlocks)
            {
                onDmgUninjuredCastingBlock.Cast(targetData);
            }   
        }
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyHealthChanged;
    }
}