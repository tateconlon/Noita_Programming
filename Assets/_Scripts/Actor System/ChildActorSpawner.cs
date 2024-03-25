using System.Collections.Generic;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChildActorSpawner : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    [SerializeField] private List<Actor> _childPrefabs;
    [Tooltip("This ability will be cast on all spawned child actors")]
    [SerializeField] private AbilityReference _childSpawnAbility;
    
    [Button]
    public void SpawnChildren()
    {
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;
        
        List<Actor> children = new();
        
        foreach (Actor childPrefab in _childPrefabs)
        {
            Actor child = LeanPool.Spawn(childPrefab, spawnPos, spawnRot);
            child.Parent = _actor;
            
            children.Add(child);
        }
        
        //_actor.Abilities.TryActivateAbility(_childSpawnAbility.Value, new TargetData(children));
    }
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
}