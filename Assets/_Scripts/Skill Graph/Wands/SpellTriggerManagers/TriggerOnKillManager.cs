using System.Collections.Generic;

public class TriggerOnKillManager : SpellTriggerManagerBase<TriggerOnKillManager>
{
    private void OnEnable()
    {
        Health.OnAnyDeath += OnAnyDeath;
    }
    
    private void OnAnyDeath(Health.HealthChangedParams healthChangedParams)
    {
        if (!_targetTrigger.IsAddedToProjectile(healthChangedParams.Attacker, out SpellProjectile spellProjectile)) return;
        
        TargetData targetData = new(healthChangedParams.Victim)
        {
            Origin = healthChangedParams.Victim.transform.position,
            Direction = healthChangedParams.Attacker.transform.up
        };
        
        if (spellProjectile.TriggerCastingBlocks.TryGetValue(_targetTrigger, out List<CastingBlock> onKillCastingBlocks))
        {
            foreach (CastingBlock onKillCastingBlock in onKillCastingBlocks)
            {
                onKillCastingBlock.Cast(targetData);
            }
        }
    }
    
    private void OnDisable()
    {
        Health.OnAnyDeath -= OnAnyDeath;
    }
}