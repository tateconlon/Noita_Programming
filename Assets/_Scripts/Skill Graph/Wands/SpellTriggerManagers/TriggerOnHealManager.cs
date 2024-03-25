using System.Collections.Generic;

public class TriggerOnHealManager : SpellTriggerManagerBase<TriggerOnHealManager>
{
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyHealthChanged;
    }
    
    private void OnAnyHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (!_targetTrigger.IsAddedToProjectile(healthChangedParams.Attacker, out SpellProjectile spellProjectile)) return;
        if (healthChangedParams.CurHealth <= healthChangedParams.PrevHealth) return;  // Only proceed if healing was done, not damage
        
        TargetData targetData = new(healthChangedParams.Victim)
        {
            Origin = healthChangedParams.Victim.transform.position,
            Direction = healthChangedParams.Attacker.transform.up
        };
        
        if (spellProjectile.TriggerCastingBlocks.TryGetValue(_targetTrigger, out List<CastingBlock> onHealCastingBlocks))
        {
            foreach (CastingBlock onHealCastingBlock in onHealCastingBlocks)
            {
                onHealCastingBlock.Cast(targetData);
            }
        }
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyHealthChanged;
    }
}