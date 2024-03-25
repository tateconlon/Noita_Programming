using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugVariables", menuName = "ScriptableObject/DebugVariables", order = 0)]
public class DebugVariables : ScriptableObject
{
    // TODO: Timescale slider, god mode, infinite gold, etc.

    public bool shouldCapFrameRate = false;
    [EnableIf(nameof(shouldCapFrameRate))]
    public int targetFrameRate = -1;
    
    public bool shouldSkipFramesRecordPreviousPositions = false;
    [EnableIf(nameof(shouldSkipFramesRecordPreviousPositions))]
    public int numSkipFramesRecordPreviousPositions = -1;

    public bool shouldOverrideMoveSpeed = false;
    [EnableIf(nameof(shouldOverrideMoveSpeed))]
    public float overrideMoveSpeed = 1.0f;

    // TODO: reimplement with runtime sets
    // [Button]
    // void ClearAllEnemies()
    // {
    //     Enemy[] enemies = FindObjectsOfType<Enemy>();
    //
    //     for (int i = enemies.Length - 1; i >= 0; i--)
    //     {
    //         Destroy(enemies[i].gameObject);
    //     }
    // }
}