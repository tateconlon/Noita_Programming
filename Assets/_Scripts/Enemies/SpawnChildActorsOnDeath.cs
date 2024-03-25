using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ChildActorSpawner))]
public class SpawnChildActorsOnDeath : MonoBehaviour
{
    [SerializeField, Required] private Health _health;
    [SerializeField, Required] private ChildActorSpawner _childSpawner;
    
    private void OnEnable()
    {
        _health.OnDeath += OnDeath;
    }
    
    private void OnDeath(Health.HealthChangedParams healthChangedParams)
    {
        _childSpawner.SpawnChildren();
    }
    
    private void OnDisable()
    {
        _health.OnDeath -= OnDeath;
    }
    
    private void Reset()
    {
        _health = GetComponent<Health>();
        _childSpawner = GetComponent<ChildActorSpawner>();
    }
}