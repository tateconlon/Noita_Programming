using UnityEngine;

public class AbilityTriggerOnDamageUninjured : AbilityTrigger
{
    private void OnEnable()
    {
        Health.OnAnyHealthChanged += OnAnyActorHealthChanged;
    }
    
    private void OnAnyActorHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (healthChangedParams.Attacker != _actor) return;  // Your attack triggered this event
        if (healthChangedParams.Attacker == healthChangedParams.Victim) return;  // Ignore self damage for now

        if (Mathf.Approximately(healthChangedParams.PrevHealth, healthChangedParams.MaxHealth))
        {
            ActivateAbilities(new TargetData(healthChangedParams.Victim));
        }
    }
    
    private void OnDisable()
    {
        Health.OnAnyHealthChanged -= OnAnyActorHealthChanged;
    }
}