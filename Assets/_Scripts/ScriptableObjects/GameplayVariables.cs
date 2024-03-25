using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayVariables", menuName = "ScriptableObject/GameplayVariables", order = 0)]
public class GameplayVariables : ScriptableObject
{
    // TODO: Heroes, classes, enemies, level data, etc.

    [Header("Movement")]
    [Tooltip("seconds to complete a circle = circlingRadiansPerSecond / 2pi")]
    public float circlingRadiansPerSecond = 5.21504380496f;  // Hardcoded as 1.66*math.pi in source
    
    [Header("Enemy Spawns")]
    public List<Vector2> spawnOffsets;
    public List<int> levelToMaxWaves;
    public List<int> levelToDistributedEnemiesChance;
}