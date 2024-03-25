using System;
using ThirteenPixels.Soda;
using UnityEngine;

public class LevelToShopLevelCalculator : MonoBehaviour
{
    [SerializeField] private ScopedVariable<int> _playerLevel;
    [SerializeField] private ScopedVariable<int> _shopLevel;
    [SerializeField] private LevelToShopLevelDict _levelToShopLevel;
    
    [Serializable] private class LevelToShopLevelDict : UnitySerializedDictionary<int, int> { }

    private void OnEnable()
    {
        _playerLevel.onChangeValue.AddResponse(OnLevelUp);
    }

    private void OnLevelUp(int newLevel)
    {
        if (_levelToShopLevel.TryGetValue(newLevel, out int shopLevel))
        {
            _shopLevel.value = shopLevel;
        }
    }
    
    private void OnDisable()
    {
        _playerLevel.onChangeValue.RemoveResponse(OnLevelUp);
    }
}