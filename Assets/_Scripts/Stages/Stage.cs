using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObject/Stage", order = 0)]

public class Stage : ScriptableObject
{
    public int maxNumEnemies = 300;
    
    [TableList(ShowIndexLabels = true, ShowPaging = true, NumberOfItemsPerPage = 5)]
    [SerializeField] List<Wave> waves;
    
    public int NumWaves => waves.Count;

    public Wave GetWave(int index)
    {
        return waves[Mathf.Clamp(index, 0, waves.Count - 1)];
    }
}

[Serializable]
public class Wave
{
    [TableColumnWidth(90, false)]
    public float duration = 20.0f;
    [TableColumnWidth(90, false)]
    public int minEnemies;
    [TableColumnWidth(100, false)]
    public float spawnInterval;
    [ListDrawerSettings(Expanded = true)]
    public List<WaveEnemy> enemies;
    [ListDrawerSettings(Expanded = true)]
    public List<BossData> bosses;
    [ListDrawerSettings(Expanded = true)]
    public List<EventData> events;
}

[Serializable]
public class WaveEnemy
{
    [AssetsOnly, Required]
    public GameObject enemyPrefab;
    public int quantity = 1;
    public int range = 1;
    public float weight = 100f;
    
}

[Serializable]
public class BossData
{
    [AssetsOnly]
    public GameObject enemyPrefab;
    public LootBoxData lootBoxData;
}

[Serializable]
public class LootBoxData
{
    
}

[Serializable]
public class EventData
{
    [InlineEditor]
    public WaveEvent waveEvent;
    
    [Range(0f, 1f)]
    public float chance = 1f;
    
    [Tooltip("How many seconds after the start of the wave to trigger the event")]
    public float delay = 0f;
    
    [Tooltip("How many times this event could be triggered")]
    public int maxIterations = 1;
}