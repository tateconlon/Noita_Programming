using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ExpPerLevelProvider : ScriptableObject
{
    [Header("Debug")]
    [MinMaxSlider(0, 150)]
    [SerializeField] Vector2Int debugLevels;

    public abstract float GetExpForLevel(int level);

    [Button]
    void DebugGetExpForLevel()
    {
        for (int i = debugLevels.x; i <= debugLevels.y; i++)
        {
            Debug.Log($"Exp for level {i} --> {i + 1} = {GetExpForLevel(i)}");
        }
    }
}