using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpawnPrefabOnHurt : MonoBehaviour
{
    [SerializeField, Required] private Health _health;
    [SerializeField, Required, AssetsOnly] private GameObject _prefab;
    
    private void OnEnable()
    {
        _health.OnHealthChanged += OnHealthChanged;
    }
    
    private void OnHealthChanged(Health.HealthChangedParams healthChangedParams)
    {
        if (healthChangedParams.HealthDeltaUnclamped >= 0f) return;
        
        LeanPool.Spawn(_prefab, transform.position, Quaternion.identity);
    }
    
    private void OnDisable()
    {
        _health.OnHealthChanged -= OnHealthChanged;
    }
    
    private void Reset()
    {
        _health = GetComponentInParent<Health>();
    }
}