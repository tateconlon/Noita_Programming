using System.Collections;
using ThirteenPixels.Soda;
using UnityEngine;

public class LevelCalculator : MonoBehaviour
{
    [SerializeField] ExpPerLevelProvider expPerLevelProvider;
    
    public ScopedVariable<float> currentExp;
    public ScopedVariable<float> maxExp;
    public ScopedVariable<int> level;
    
    // Use temp values for the calculation so we don't send multiple level up events.
    float _tempCurrentExp;
    float _tempMaxExp;
    int _tempLevel;

    bool _tempValuesDirty = false;  // True if temp values have already been modified this frame

    bool _lockSettingValues = false;

    Coroutine _flushChangesCoroutine;

    void OnEnable()
    {
        // Set maxExp to proper value from expPerLevelProvider so calculations will be correct
        maxExp.value = expPerLevelProvider.GetExpForLevel(level);
        
        currentExp.onChangeValue.AddResponse(CalculateLevel);
        maxExp.onChangeValue.AddResponse(CalculateLevel);
        
        CalculateLevel();
    }

    void CalculateLevel()
    {
        if (_lockSettingValues) return;  // If we're the ones setting currentExp/maxExp, no need to recalculate now
        
        if (!_tempValuesDirty)
        {
            _tempCurrentExp = currentExp;
            _tempMaxExp = maxExp;
            _tempLevel = level;
        }
        
        while (_tempCurrentExp >= _tempMaxExp)
        {
            _tempCurrentExp -= _tempMaxExp;
            _tempLevel += 1;
            _tempMaxExp = expPerLevelProvider.GetExpForLevel(_tempLevel);

            _tempValuesDirty = true;
        }
        
        if (_tempValuesDirty && _flushChangesCoroutine == null)
        {
            _flushChangesCoroutine = StartCoroutine(FlushChanges());
        }
    }
    
    // Flush value updates (and fire events) at end of frame in case multiple onChangeValues happen per frame
    IEnumerator FlushChanges()
    {
        yield return new WaitForEndOfFrame();
        
        _lockSettingValues = true;
        
        currentExp.value = _tempCurrentExp;
        maxExp.value = _tempMaxExp;
        level.value = _tempLevel;

        _lockSettingValues = false;

        _tempValuesDirty = false;
        _flushChangesCoroutine = null;
    }

    void OnDisable()
    {
        currentExp.onChangeValue.RemoveResponse(CalculateLevel);
        maxExp.onChangeValue.RemoveResponse(CalculateLevel);
    }
}
