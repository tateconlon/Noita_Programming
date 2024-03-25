using System;
using ThirteenPixels.Soda;
using UnityEngine;

public class GoldPerLevelAdder : MonoBehaviour
{
    [SerializeField] private ScopedVariable<int> _playerLevel;
    [SerializeField] private ScopedVariable<int> _playerGold;
    [SerializeField] private LevelToGoldDict _levelToGold;
    
    [Serializable] private class LevelToGoldDict : UnitySerializedDictionary<int, Vector2> { }

    private void OnEnable()
    {
        _playerLevel.onChangeValue.AddResponse(OnLevelUp);
    }

    private void OnLevelUp(int newLevel)
    {
        if (_levelToGold.TryGetValue(newLevel, out Vector2 goldThisLevelRange))
        {
            _playerGold.value += goldThisLevelRange.RandomInRangeRounded();
        }
    }
    
    private void OnDisable()
    {
        _playerLevel.onChangeValue.RemoveResponse(OnLevelUp);
    }
}
