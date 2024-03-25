using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class SpawnPrefabsOnDisable : MonoBehaviour
{
    [SerializeField] public List<GameObject> prefabs = new();
    
    private void OnDisable()
    {
        if (this.IsSceneUnloading()) return;
        
        foreach (GameObject prefab in prefabs)
        {
            LeanPool.Spawn(prefab, transform.position, Quaternion.identity);
        }
    }
}