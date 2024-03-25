public class AbilityTriggerOnKill : AbilityTrigger
{
    private void OnEnable()
    {
        Health.OnAnyDeath += OnAnyActorDeath;
    }
    
    private void OnAnyActorDeath(Health.HealthChangedParams healthChangedParams)
    {
        if (healthChangedParams.Attacker != _actor) return;  // Your attack triggered this event
        if (healthChangedParams.Attacker == healthChangedParams.Victim) return;  // Ignore self damage for now
        
        ActivateAbilities(new TargetData(healthChangedParams.Victim));
    }
    
    private void OnDisable()
    {
        Health.OnAnyDeath -= OnAnyActorDeath;
    }
}