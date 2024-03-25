using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class TriggerOnKillAssistManager : SpellTriggerManagerBase<TriggerOnKillAssistManager>
{
    [NonSerialized, ShowInInspector, ReadOnly]
    private Dictionary<Actor, HashSet<CastingBlock>> _blocksToCastOnKill = new();
    
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyHealthChanged;
        Health.OnAnyDeath += OnAnyDeath;
    }
    
    private void OnAnyHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (!_targetTrigger.IsAddedToProjectile(healthChangedParams.Attacker, out SpellProjectile spellProjectile)) return;
        if (healthChangedParams.CurHealth >= healthChangedParams.PrevHealth) return;  // Only proceed if damage was done, not healing
        if (healthChangedParams.CurHealth <= 0f) return;  // Only proceed if the damage done was non-lethal

        if (spellProjectile.TriggerCastingBlocks.TryGetValue(_targetTrigger, out List<CastingBlock> onKillAssistCastingBlocks))
        {
            foreach (CastingBlock onKillAssistCastingBlock in onKillAssistCastingBlocks)
            {
                _blocksToCastOnKill.GetOrCreate(healthChangedParams.Victim).Add(onKillAssistCastingBlock);
            }
        }
    }
    
    private void OnAnyDeath(Health.HealthChangedParams healthChangedParams)
    {
        if (!_blocksToCastOnKill.TryGetValue(healthChangedParams.Victim, out HashSet<CastingBlock> blocks)) return;
        
        TargetData targetData = new(healthChangedParams.Victim)
        {
            Origin = healthChangedParams.Victim.transform.position,
            Direction = healthChangedParams.Attacker.transform.up
        };
        
        foreach (CastingBlock block in blocks)
        {
            block.Cast(targetData);
        }
        
        _blocksToCastOnKill.Remove(healthChangedParams.Victim);
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyHealthChanged;
        Health.OnAnyDeath -= OnAnyDeath;
    }
}