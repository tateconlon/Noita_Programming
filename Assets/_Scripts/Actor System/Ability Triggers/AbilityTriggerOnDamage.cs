public class AbilityTriggerOnDamage : AbilityTrigger
{
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyActorHealthChanged;
    }
    
    private void OnAnyActorHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (healthChangedParams.Attacker != _actor) return;  // Your attack triggered this event
        if (healthChangedParams.Attacker == healthChangedParams.Victim) return;  // Ignore self damage for now
        
        ActivateAbilities(new TargetData(healthChangedParams.Victim));
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyActorHealthChanged;
    }
}