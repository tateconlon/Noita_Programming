using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using MoreMountains.Feedbacks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Camera spawnVisibilityCam;
    //[SerializeField] private BoundsWithHandle validSpawnBounds;
    //Making spawnBounds static for easy access for prototyping
    public static BoundsWithHandle validSpawnBounds;
    [SerializeField] private SpawnPattern defaultSpawnPattern;
    [SerializeField] private MMF_Player spawnWaveEventFx;
    
    private bool _hasCalledStart = false;
    
    private Transform _enemyParent;
    
    private void Awake()
    {
        _enemyParent = new GameObject("Enemy Parent").transform;
    }
    
    private void Start()
    {
        _hasCalledStart = true;
        
        GameManager.Instance.WaveCombat.OnSetIsActive += OnActivateWaveCombat;
        OnActivateWaveCombat(GameManager.Instance.WaveCombat.IsActive);
    }
    
    private void OnActivateWaveCombat(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    
    private void OnEnable()
    {
        if (!_hasCalledStart) return;
        
        StartCoroutine(PeriodicSpawnCR());
        
        SpawnWaveBosses();
        
        foreach (EventData eventData in GameManager.Instance.CurWave.events)
        {
            StartCoroutine(WaveEventCoroutine(eventData));
        }
    }
    
    private void SpawnWaveBosses()
    {
        foreach (BossData bossData in GameManager.Instance.CurWave.bosses)
        {
            SpawnRequest spawnRequest = new()
            {
                EnemyPrefab = bossData.enemyPrefab,
                Quantity = 1
            };
            
            SpawnEnemy(spawnRequest);
        }
    }

    private IEnumerator WaveEventCoroutine(EventData eventData)
    {
        for (int i = 0; i < eventData.maxIterations; i++)
        {
            if (!CodeHelpers.RandomBool(eventData.chance)) break;  // Prevent this AND future iterations on fail chance
            
            yield return new WaitForSeconds(eventData.delay);
            
            WaveEvent waveEvent = eventData.waveEvent;
            
            SpawnRequest spawnRequest = new()
            {
                EnemyPrefab = waveEvent.enemyPrefab,
                Quantity = waveEvent.numEnemies,
                SpawnPattern = waveEvent.spawnPattern
            };
            
            SpawnEnemy(spawnRequest);
            
            spawnWaveEventFx.PlayFeedbacks();
        }
    }

    private void Update()
    {
        EnforceMinEnemyCount();
    }

    private void EnforceMinEnemyCount()
    {
        if (HTags.Enemy.GameObjects.Count >= GameManager.Instance.CurStage.maxNumEnemies) return;
        if (HTags.Enemy.GameObjects.Count >= GameManager.Instance.CurWave.minEnemies) return;
        
        SpawnRandomEnemy(new SpawnRequest());
    }
    
    private IEnumerator PeriodicSpawnCR()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(GameManager.Instance.CurWave.spawnInterval);

            if (HTags.Enemy.GameObjects.Count < GameManager.Instance.CurStage.maxNumEnemies)
            {
                SpawnRandomEnemy(new SpawnRequest());
            }
        }
    }
    
    private void SpawnRandomEnemy(SpawnRequest spawnRequest)
    {
        if (GameManager.Instance.CurWave.enemies == null || GameManager.Instance.CurWave.enemies.Count == 0) return;  // If wave only has bosses/events
        
        WaveEnemy enemy = GameManager.Instance.CurWave.enemies.WeightedPick(waveEnemy => waveEnemy.weight);
        
        spawnRequest.EnemyPrefab = enemy.enemyPrefab;
        int numEnemies = Random.Range(enemy.quantity - enemy.range, enemy.quantity + enemy.range);
        numEnemies = Mathf.Max(1, numEnemies);
        spawnRequest.Quantity = numEnemies;
        
        SpawnEnemy(spawnRequest);
    }
    
    public void SpawnEnemy(SpawnRequest spawnRequest)
    {
        if (gameObject.IsSceneUnloading()) return;
        
        spawnRequest.SpawnVisibilityCam = spawnVisibilityCam;
        
        if (spawnRequest.SpawnPattern == null)
        {
            spawnRequest.SpawnPattern = defaultSpawnPattern;
        }
        
        spawnRequest.SpawnPattern.GetSpawnPositions(spawnRequest);
        
        RemoveInvalidSpawnPoints(spawnRequest);
        
        foreach (Vector2 spawnPoint in spawnRequest.SpawnPositions)
        {
            LeanPool.Spawn(spawnRequest.EnemyPrefab, spawnPoint, Quaternion.identity, _enemyParent);
        }
    }

    private void RemoveInvalidSpawnPoints(SpawnRequest spawnRequest)
    {
        Bounds bounds = validSpawnBounds.GetBounds();    //We have to convert to bounds
        spawnRequest.SpawnPositions.RemoveAll(spawnPoint => !bounds.Contains(spawnPoint));
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.WaveCombat.OnSetIsActive -= OnActivateWaveCombat;
    }

    public class SpawnRequest
    {
        public GameObject EnemyPrefab;
        public int Quantity = 1;
        public SpawnPattern SpawnPattern;
        public Camera SpawnVisibilityCam;
        public Vector2? CenterPos = null;
        public List<Vector2> SpawnPositions = new();
    }
}
