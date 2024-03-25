using System.Collections.Generic;

public class TriggerOnDamageManager : SpellTriggerManagerBase<TriggerOnDamageManager>
{
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyHealthChanged;
    }
    
    private void OnAnyHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (!_targetTrigger.IsAddedToProjectile(healthChangedParams.Attacker, out SpellProjectile spellProjectile)) return;
        if (healthChangedParams.CurHealth >= healthChangedParams.PrevHealth) return;  // Only proceed if damage was done, not healing
        
        TargetData targetData = new(healthChangedParams.Victim)
        {
            Origin = healthChangedParams.Victim.transform.position,
            Direction = healthChangedParams.Attacker.transform.up
        };
        
        if (spellProjectile.TriggerCastingBlocks.TryGetValue(_targetTrigger, out List<CastingBlock> onDmgCastingBlocks))
        {
            foreach (CastingBlock onDmgCastingBlock in onDmgCastingBlocks)
            {
                onDmgCastingBlock.Cast(targetData);
            }
        }
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyHealthChanged;
    }
}