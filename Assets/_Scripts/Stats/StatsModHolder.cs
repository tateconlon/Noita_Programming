using System;
using UnityEditor;
using UnityEngine;

public class StatsModHolder : MonoBehaviour
{
    private StatMod[] _data = new StatMod[21];

    public StatMod this[StatType s] => _data[(int)s];
    
    public int Length => _data.Length;
    public bool IsEmpty => _data.Length == 0;
    
    public static event Action<StatsModHolder> OnChange;

    private void Awake()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = new StatMod();
        }
    }

    public void ApplyStatChanges(StatChange[] statChanges)
    {
        for (int i = 0; i < statChanges.Length; i++)
        {
            StatChange statChange = statChanges[i];
            if (statChange.isFlatMod)
            {
                this[statChange.type].AddFlatBonus(statChange.flatChange);
            }
            else if (statChange.multChange > 0f)
            {
                this[statChange.type].AddMultiplierBonus(statChange.multChange);
            }
            else if (statChange.multChange < 0f)
            {
                float absReduction = 1 - Mathf.Abs(statChange.multChange);    //if we enter -0.1, we want .9 reduction
                this[statChange.type].AddMultiplierReduction(absReduction);
                //print($"ADD: absReduction: {absReduction} | res: {this[statChange.type].MultiplierReduction}");
            }
        }
        OnChangeInvoke();
    }
    
    public void RemoveStatChanges(StatChange[] statChanges)
    {
        for (int i = 0; i < statChanges.Length; i++)
        {
            StatChange statChange = statChanges[i];
            if (statChange.isFlatMod)
            {
                this[statChange.type].AddFlatBonus(-statChange.flatChange);
            }
            else if (statChange.multChange > 0f)
            {
                this[statChange.type].AddMultiplierBonus(-statChange.multChange);
            }
            else if (statChange.multChange < 0f)
            {
                float absReduction = 1 - Mathf.Abs(statChange.multChange);   //if we enter -0.1, we want .9 reduction
                
                //Since "adding" the multiplier reduction is actually multiplying,
                //to remove it we need to divide by the value (since dividing is the inverse of multiplying)
                //This is just like how to remove a flat bonus you subtract the bonus
                
                //Avoid divide by zero
                absReduction = absReduction.IsApprox(0) ? StatMod.ZERO_MULT_RED : absReduction;
                this[statChange.type].AddMultiplierReduction(1f / (absReduction));
                //print($"REMOVE: absReduction: {absReduction} | res: {this[statChange.type].MultiplierReduction}");
            }
        }
        OnChangeInvoke();
    }

    void OnChangeInvoke()
    {
        OnChange?.Invoke(this);
#if UNITY_EDITOR
        //Trigger Custom Inspector Repaint with new values
        if (this != null)
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
    
    public void Reset()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            if (_data[i] == null) continue;
            _data[i].Clear();
        }
        
        OnChangeInvoke();
    }
}