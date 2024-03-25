using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

[System.Serializable]
public abstract class SkillNode : Node
{
    [SerializeField, HideInInspector] private SkillDefinition _definition;
    
    [ShowInInspector]
    public SkillDefinition Definition
    {
        get => _definition;
        set
        {
            if (_definition == value) return;
            
            _definition = value;
            OnModifyData();
        }
    }
    
    public int maxNumChildren = 0;
    public bool canBeMoved = true;
    public bool IgnoreReceiver => Definition != null && Definition.ignoreReceiver;
    
    public bool IsEmpty
    {
        get => Definition == null;
        set
        {
            if (value)
            {
                Definition = null;
            }
            else
            {
                Debug.LogWarning($"Setting {nameof(IsEmpty)} to false has no effect, assign {nameof(Definition)} instead");
            }
        }
    }
    
    public abstract bool HasParent { get; }

    public abstract SkillNode ParentBase { get; }
    
    public abstract SkillNode[] ChildrenBase { get; }
    
    private void OnModifyData()
    {
        if (IsEmpty)
        {
            name = "Empty";
            maxNumChildren = 0;
        }
        else
        {
            name = $"{Definition.displayName})";
            maxNumChildren = 1;
        }
    }
    
    // TODO: something like this could be used to set attributes to values from within random ranges
    // protected virtual void InitStats(SkillTierStats stats)
    // {
    //     maxNumChildren = stats.numChildrenRange.RandomInRangeRounded();
    //     
    //     foreach ((StatType statType, Vector2 valueRange) in stats.statValueRanges)
    //     {
    //         statValues[statType] = valueRange.RandomInRange();
    //     }
    // }

    public string StatsToString()
    {
        // TODO: this would be replaced with something like making a string of all attributes
        return "";
        
        // StringBuilder stringBuilder = new();
        //
        // foreach ((StatType statType, float value) in statValues)
        // {
        //     stringBuilder.AppendLine($"{statType.displayName}: {value}");
        // }
        //
        // return stringBuilder.ToString();
    }
}
