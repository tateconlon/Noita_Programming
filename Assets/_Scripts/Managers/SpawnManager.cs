using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] LeanGameObjectPool spawnMarkerPool;
    [SerializeField] GameObject _tempSeekerPrefab;

    [Header("Spawn Points")]
    [SerializeField] Transform bossSpawnPoint;
    [SerializeField] List<Transform> primarySpawnPoints;
    [SerializeField] List<Transform> secondarySpawnPoints;
    
    [NonSerialized] public bool isSpawningEnemies;
    
    Sequence _setNotSpawningEnemiesSequence;  // Equivalent to the 'spawning_enemies' tag in source
    // Equivalent to the 'spawn_enemies_' .. j tags in source (e.g. 'spawn_enemies_1', 'spawn_enemies_2', etc.)
    readonly Dictionary<int, Sequence> _spawnNumEnemiesSequences = new();

    int _numEnemySpawnsPrevented;

    Func<bool> _shouldSpawnEnemies;
    Action _checkWinCondition;

    public void Initialize(Func<bool> shouldSpawnEnemies, Action checkWinCondition)
    {
        _shouldSpawnEnemies = shouldSpawnEnemies;
        _checkWinCondition = checkWinCondition;
    }
    
    /// <remarks>
    /// Source code arena.lua spawn_distributed_enemies
    /// </remarks>
    void SpawnDistributedEnemies()
    {
        isSpawningEnemies = true;
        float setNotSpawningEnemiesDelay;
        
        SpawnType[] spawnTypes = (SpawnType[])Enum.GetValues(typeof(SpawnType));
        float[] spawnPatternWeights = { 20, 20, 10, 15, 10, 15 };

        //SpawnType spawnPattern = spawnTypes[spawnPatternWeights.WeightedPick(f => f)];
        SpawnType spawnPattern = SpawnType.FourPlusFourPlusFour;

        List<Transform> remainingSpawnPoints = new(secondarySpawnPoints);
        Vector2 spawnPoint;

        switch (spawnPattern)
        {
            case SpawnType.Four:
                spawnPoint = remainingSpawnPoints.RemoveRandom().position;
                InstantiateSpawnMarker(spawnPoint);
                
                DOTween.Sequence()
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint));

                setNotSpawningEnemiesDelay = 2.25f;
                break;
            case SpawnType.FourPlusFour:
                spawnPoint = remainingSpawnPoints.RemoveRandom().position;
                InstantiateSpawnMarker(spawnPoint);
                
                DOTween.Sequence()
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint))
                    .AppendInterval(2.0f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint));
                
                setNotSpawningEnemiesDelay = 4.25f;
                break;
            case SpawnType.FourPlusFourPlusFour:
                spawnPoint = remainingSpawnPoints.RemoveRandom().position;
                InstantiateSpawnMarker(spawnPoint);
                
                DOTween.Sequence()
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint))
                    .AppendInterval(1.0f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint))
                    .AppendInterval(1.0f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint));
                
                setNotSpawningEnemiesDelay = 4.25f;
                break;
            case SpawnType.TwoTimesFour:
                spawnPoint = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint, 0));
                
                Vector2 spawnPoint1 = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint1))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint1, 1));
                
                setNotSpawningEnemiesDelay = 2.25f;
                break;
            case SpawnType.ThreeTimesFour:
                spawnPoint = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint, 0));
                
                spawnPoint1 = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint1))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint1, 1));
                
                Vector2 spawnPoint2 = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint2))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint2, 2));
                
                setNotSpawningEnemiesDelay = 2.25f;
                break;
            case SpawnType.FourTimesTwo:
                spawnPoint = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint, 0, 2));
                
                spawnPoint1 = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint1))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint1, 1, 2));
                
                spawnPoint2 = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint2))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint2, 2, 2));
                
                Vector2 spawnPoint3 = remainingSpawnPoints.RemoveRandom().position;
                DOTween.Sequence()
                    .AppendInterval(Random.Range(0.0f, 0.2f))
                    .AppendCallback(() => InstantiateSpawnMarker(spawnPoint3))
                    .AppendInterval(1.125f)
                    .AppendCallback(() => SpawnNumEnemies(spawnPoint3, 3, 2));
                
                setNotSpawningEnemiesDelay = 2.25f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        UnsetIsSpawningEnemiesDelayed(setNotSpawningEnemiesDelay);
    }

    /// <remarks>
    /// Source code arena.lua spawn_n_enemies
    /// </remarks>
    void SpawnNumEnemies(Vector2 position, int j = 0, int numEnemies = 4, bool pass = false)
    {
        if (!_shouldSpawnEnemies.Invoke()) return;
        if (numEnemies < 1) return;

        if (_spawnNumEnemiesSequences.TryGetValue(j, out Sequence spawnNumEnemiesSequence))
        {
            spawnNumEnemiesSequence?.Kill();
        }
        
        _spawnNumEnemiesSequences[j] = DOTween.Sequence()
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                Vector2 offset = D.gv.spawnOffsets[_spawnNumEnemiesSequences[j].CompletedLoops() 
                                                   % D.gv.spawnOffsets.Count];
                
                // TODO: temp spawning code
                Instantiate(_tempSeekerPrefab.gameObject, position + offset, Quaternion.identity);

                // TODO: copy over behavior from SpawnEffect class in source code
                // SpawnEffect{group = self.effects, x = p.x + o.x, y = p.y + o.y, action = function(x, y)
                //   spawn1:play{pitch = random:float(0.8, 1.2), volume = 0.15}
                //   if not pass then
                //     check_circle:move_to(x, y)
                //     local objects = self.main:get_objects_in_shape(check_circle, {Seeker, EnemyCritter, Critter, Player, Sentry, Automaton, Bomb, Volcano, Saboteur, Pet, Turret})
                //     if #objects > 0 then self.enemy_spawns_prevented = self.enemy_spawns_prevented + 1; return end
                //   end
                //
                //   if random:bool(table.reduce(level_to_elite_spawn_weights[self.level], function(memo, v) return memo + v end)) then
                //     local elite_type = level_to_elite_spawn_types[self.level][random:weighted_pick(unpack(level_to_elite_spawn_weights[self.level]))]
                //     Seeker{group = self.main, x = x, y = y, character = 'seeker', level = self.level,
                //       speed_booster = elite_type == 'speed_booster', exploder = elite_type == 'exploder', shooter = elite_type == 'shooter', headbutter = elite_type == 'headbutter', tank = elite_type == 'tank', spawner = elite_type == 'spawner'}
                //   else
                //     Seeker{group = self.main, x = x, y = y, character = 'seeker', level = self.level}
                //   end
                // end}

            })
            .SetLoops(numEnemies);
    }
    
    /// <summary>
    /// Periodically tries to spawn enemies to counteract failed spawns that were prevented
    /// </summary>
    /// <remarks>
    /// Source code arena.lua lines 287-322
    /// </remarks>
    public IEnumerator RetryPreventedEnemySpawns()
    {
        _numEnemySpawnsPrevented = 0;

        while (_shouldSpawnEnemies.Invoke())
        {
            yield return new WaitForSeconds(8.0f);
            
            if (isSpawningEnemies) continue;
            if (Mathf.Floor(_numEnemySpawnsPrevented / 4.0f) <= 0) continue;
            
            isSpawningEnemies = true;
            int numEnemiesToSpawn = _numEnemySpawnsPrevented;
            List<Transform> remainingSpawnPoints = new(secondarySpawnPoints);

            Vector2 spawnPoint = remainingSpawnPoints.RemoveRandom().position;
            DOTween.Sequence()
                .AppendInterval(Random.Range(0.0f, 0.2f))
                .AppendCallback(() => InstantiateSpawnMarker(spawnPoint))
                .AppendInterval(1.125f)
                .AppendCallback(() => SpawnNumEnemies(spawnPoint, 0, Mathf.FloorToInt(numEnemiesToSpawn / 4.0f), true));
                
            Vector2 spawnPoint1 = remainingSpawnPoints.RemoveRandom().position;
            DOTween.Sequence()
                .AppendInterval(Random.Range(0.0f, 0.2f))
                .AppendCallback(() => InstantiateSpawnMarker(spawnPoint1))
                .AppendInterval(1.125f)
                .AppendCallback(() => SpawnNumEnemies(spawnPoint1, 1, Mathf.FloorToInt(numEnemiesToSpawn / 4.0f), true));
                
            Vector2 spawnPoint2 = remainingSpawnPoints.RemoveRandom().position;
            DOTween.Sequence()
                .AppendInterval(Random.Range(0.0f, 0.2f))
                .AppendCallback(() => InstantiateSpawnMarker(spawnPoint2))
                .AppendInterval(1.125f)
                .AppendCallback(() => SpawnNumEnemies(spawnPoint2, 2, Mathf.FloorToInt(numEnemiesToSpawn / 4.0f), true));
                
            Vector2 spawnPoint3 = remainingSpawnPoints.RemoveRandom().position;
            DOTween.Sequence()
                .AppendInterval(Random.Range(0.0f, 0.2f))
                .AppendCallback(() => InstantiateSpawnMarker(spawnPoint3))
                .AppendInterval(1.125f)
                .AppendCallback(() => SpawnNumEnemies(spawnPoint3, 3, Mathf.FloorToInt(numEnemiesToSpawn / 4.0f), true));
            
            UnsetIsSpawningEnemiesDelayed(1.125f + Mathf.Floor(numEnemiesToSpawn / 4.0f) * 0.25f);

            _numEnemySpawnsPrevented = 0;
        }
    }

    void UnsetIsSpawningEnemiesDelayed(float delay)
    {
        _setNotSpawningEnemiesSequence?.Kill();
        _setNotSpawningEnemiesSequence = DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() =>
            {
                isSpawningEnemies = false;
                _checkWinCondition.Invoke();
            });
    }
    
    void InstantiateSpawnMarker(Vector2 position)
    {
        GameObject newSpawnMarker = spawnMarkerPool.Spawn(position, Quaternion.identity);
        
        spawnMarkerPool.Despawn(newSpawnMarker, 1.125f);  // Automatically despawn after interval
    }

    public IEnumerator SpawnBoss(int level, Action<GameObject> bossSpawnedCallback)
    {
        // self.boss = Seeker{group = self.main, x = x, y = y, character = 'seeker', level = self.level, boss = level_to_boss[self.level]}
        
        // TODO: Wait for the duration of the SpawnEffect before continuing. Should SpawnEffect return wait duration?
        // SpawnEffect{group = self.effects, x = gw/2, y = gh/2}  // TODO: pass in bossSpawnPoint as position
        yield return new WaitForSeconds(0.1f);  // This is the hardcoded duration of SpawnEffect's init tween in source
        
        // spawn1:play{pitch = random:float(0.8, 1.2), volume = 0.15}
        InstantiateSpawnMarker(bossSpawnPoint.position);

        yield return new WaitForSeconds(1.125f);
        
        // TODO: replace with choosing boss type and passing in parameters
        GameObject boss = Instantiate(_tempSeekerPrefab.gameObject, bossSpawnPoint.position, Quaternion.identity);
        
        bossSpawnedCallback?.Invoke(boss);
    }

    // TODO: Put this and SpawnEnemiesWaveLevel into different classes, one for each type of level (strategy pattern?)
    public IEnumerator SpawnEnemiesBossLevel(int level)
    {
        // self.hfx:use('condition1', 0.25, 200, 10)
        // self.hfx:pull('condition2', 0.0625)

        yield return new WaitForSeconds(0.5f);

        isSpawningEnemies = true;
        UnsetIsSpawningEnemiesDelayed((8.0f + Mathf.Floor(level / 2.0f)) * 0.1f + 1.25f);
        
        Vector2 spawnPoint = primarySpawnPoints.GetRandom().position;
            
        InstantiateSpawnMarker(spawnPoint);

        yield return new WaitForSeconds(1.125f);
            
        SpawnNumEnemies(spawnPoint, 0, 8 + Mathf.FloorToInt(level / 2.0f));
    }

    public IEnumerator SpawnEnemiesWaveLevel(int level, int wave, int loop)
    {
        // self.hfx:use('condition1', 0.25, 200, 10)
        // self.hfx:pull('condition2', 0.0625)
        
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log($"Starting wave {wave}");
        
        if (CodeHelpers.RandomBool(D.gv.levelToDistributedEnemiesChance[
                (level - 1) % D.gv.levelToDistributedEnemiesChance.Count]))
        {
            int numDistributedEnemySpawns = Mathf.CeilToInt(
                (8.0f + (wave + Mathf.Min(loop * 6.0f, 60.0f)) * 2.0f) / 7.0f);

            for (int j = 0; j < numDistributedEnemySpawns; j++)
            {
                SpawnDistributedEnemies();

                yield return new WaitForSeconds(2.0f);
            }
        }
        else
        {
            isSpawningEnemies = true;
            UnsetIsSpawningEnemiesDelayed((8.0f + wave * 2.0f) * 0.1f + 1.25f);
            
            Vector2 spawnPoint = primarySpawnPoints.GetRandom().position;
                
            InstantiateSpawnMarker(spawnPoint);

            yield return new WaitForSeconds(1.125f);
                
            SpawnNumEnemies(spawnPoint, 0, 8 + (wave + Mathf.Min(loop * 12, 200)) * 2);
        }
    }
    
    enum SpawnType
    {
        Four,
        FourPlusFour,
        FourPlusFourPlusFour,
        TwoTimesFour,
        ThreeTimesFour,
        FourTimesTwo
    }
}
