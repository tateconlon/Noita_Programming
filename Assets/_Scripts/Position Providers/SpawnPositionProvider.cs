using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionProvider : MonoBehaviour
{
    [SerializeField]
    private PrefabSpawnPointsDict _prefabSpawnPoints = new();
    
    [Serializable] private class PrefabSpawnPointsDict : UnitySerializedDictionary<GameObject, PositionProvider> { }
    
    public bool GetSpawnPositionRotation(GameObject prefab, out PositionRotation spawnPosRot)
    {
        if (!_prefabSpawnPoints.TryGetValue(prefab, out PositionProvider spawnTransformProvider))
        {
            Debug.LogException(new KeyNotFoundException($"No spawn transform provider for prefab: {prefab.name}"));
            spawnPosRot = default;
            return false;
        }
        
        return spawnTransformProvider.TryGetPositionRotation(out spawnPosRot);
    }
}
