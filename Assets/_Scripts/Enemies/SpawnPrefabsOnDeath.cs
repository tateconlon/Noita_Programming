using System.Collections.Generic;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(HealthV2))]
public class SpawnPrefabsOnDeath : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField] public List<GameObject> prefabs = new();
    
    private void OnEnable()
    {
        _health.OnDeath += OnDeath;
    }
    
    private void OnDeath(HealthV2.HpChangeParams hpChangeParams)
    {
        foreach (GameObject prefab in prefabs)
        {
            LeanPool.Spawn(prefab, transform.position, Quaternion.identity);
        }
    }
    
    private void OnDisable()
    {
        _health.OnDeath -= OnDeath;
    }
    
    private void Reset()
    {
        _health = GetComponent<HealthV2>();
    }
}
