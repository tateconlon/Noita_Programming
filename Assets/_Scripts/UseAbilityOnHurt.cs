using Sirenix.OdinInspector;
using UnityEngine;

public class UseAbilityOnHurt : MonoBehaviour
{
    [SerializeField, Required] private Health _health;
    [SerializeField] private Target _target = Target.Self;
    [SerializeField] private AbilityReference _ability;
    
    private void OnEnable()
    {
        _health.OnHealthChanged += OnHealthChanged;
    }
    
    private void OnHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (healthChangedParams.HealthDeltaUnclamped >= 0f) return;
        
        TargetData targetData = new();
        targetData.Origin = transform.position;
        
        switch (_target)
        {
            case Target.Self:
                targetData.Targets.Add(healthChangedParams.Victim);
                break;
            case Target.Attacker:
                targetData.Targets.Add(healthChangedParams.Attacker);
                break;
        }
        
        // healthChangedParams.Victim.Abilities.TryActivateAbility(_ability.Value, targetData);
    }
    
    private void OnDisable()
    {
        _health.OnHealthChanged -= OnHealthChanged;
    }
    
    private void Reset()
    {
        _health = GetComponentInParent<Health>();
    }
    
    private enum Target
    {
        Self,
        Attacker
    }
}