using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SpawnPattern : ScriptableObject
{
    public abstract void GetSpawnPositions(EnemySpawner.SpawnRequest spawnRequest);
    
    [Button]
    private void DebugDrawSpawnPositions(EnemySpawner.SpawnRequest debugSpawnRequest)
    {
        if (debugSpawnRequest.SpawnPattern == null)
        {
            debugSpawnRequest.SpawnPattern = this;
        }
        
        if (debugSpawnRequest.SpawnVisibilityCam == null)
        {
            debugSpawnRequest.SpawnVisibilityCam = Camera.main;
        }
        
        GetSpawnPositions(debugSpawnRequest);
        
        foreach (Vector2 debugSpawnPoint in debugSpawnRequest.SpawnPositions)
        {
            DebugExtension.DebugPoint(debugSpawnPoint, Color.red, duration:1.0f);
        }
    }
}
