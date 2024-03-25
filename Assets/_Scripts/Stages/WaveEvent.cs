using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveEvent", menuName = "ScriptableObject/WaveEvent", order = 0)]
public class WaveEvent : ScriptableObject
{
    [AssetsOnly]
    public GameObject enemyPrefab;
    public int numEnemies = 1;
    
    public SpawnPattern spawnPattern;
}