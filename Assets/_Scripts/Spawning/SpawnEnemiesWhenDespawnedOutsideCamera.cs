using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Inspired a little by <a href="https://twitter.com/terrybrash/status/1550061786115854341">this enemy wrapping method</a>,
/// but spawns enemies on the opposite side of the viewport, e.g. despawning an enemy at the bottom-left corner of the
/// viewport will spawn a new enemy at the top-right.
/// </summary>
public class SpawnEnemiesWhenDespawnedOutsideCamera : MonoBehaviour
{
    [SerializeField, Required]
    private EnemySpawner _enemySpawner;
    
    private EnemySpawner.SpawnRequest _pendingSpawnRequest = null;
    private int _numEnemyDespawnsInQueue = 0;
    
    private void OnEnable()
    {
        DespawnEnemiesOutsideCamera.OnWillDespawnEnemy += OnEnemyDespawnedOutsideCamera;

        _pendingSpawnRequest = null;
        _numEnemyDespawnsInQueue = 0;
    }
    
    private void OnEnemyDespawnedOutsideCamera(GameObject despawnedEnemy, Camera despawnCamera)
    {
        _numEnemyDespawnsInQueue += 1;
        
        if (_pendingSpawnRequest == null)
        {
            _pendingSpawnRequest = new EnemySpawner.SpawnRequest();
            
            WaveEnemy enemy = GameManager.Instance.CurWave.enemies.WeightedPick(waveEnemy => waveEnemy.weight);
            
            _pendingSpawnRequest.EnemyPrefab = enemy.enemyPrefab;
            _pendingSpawnRequest.Quantity = enemy.quantity;
        }
        
        if (_numEnemyDespawnsInQueue >= _pendingSpawnRequest.Quantity)
        {
            _pendingSpawnRequest.CenterPos = GetPositionAcrossViewport(despawnedEnemy.transform.position, despawnCamera);
            
            _enemySpawner.SpawnEnemy(_pendingSpawnRequest);
            
            _pendingSpawnRequest = null;
            _numEnemyDespawnsInQueue = 0;
        }
    }
    
    private static Vector2 GetPositionAcrossViewport(Vector2 originPosWorld, Camera camera)
    {
        Vector2 originPosViewport = camera.WorldToViewportPoint(originPosWorld);
        Vector2 destPosViewport = Vector2.one - originPosViewport;  // Subtract from (1, 1) to invert position
        Vector2 destPosWorld = camera.ViewportToWorldPoint(destPosViewport);
        
        Debug.DrawRay(originPosWorld, destPosWorld - originPosWorld, Color.cyan, 1.0f);
        
        return destPosWorld;
    }
    
    private void OnDisable()
    {
        DespawnEnemiesOutsideCamera.OnWillDespawnEnemy -= OnEnemyDespawnedOutsideCamera;
    }
    
    private void Reset()
    {
        _enemySpawner = GetComponentInParent<EnemySpawner>();
    }
}