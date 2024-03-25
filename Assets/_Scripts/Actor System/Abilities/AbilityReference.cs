using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[InlineProperty]
public class AbilityReference : ISerializationCallbackReceiver
{
    [SerializeField, HideLabel, HorizontalGroup(width: 20), DisableInPlayMode]
    protected ReferenceMode _referenceMode = ReferenceMode.Local;
    
    [SerializeField, HideLabel, HorizontalGroup, ShowIf(nameof(_referenceMode), Value = ReferenceMode.Local)]
    private Ability _localValue;
    
    [SerializeField, HideLabel, HorizontalGroup, ShowIf(nameof(_referenceMode), Value = ReferenceMode.Global)]
    private GlobalAbility _globalAbility;
    
    public Ability Value { get; private set; }
    
    public void OnBeforeSerialize() { }
    
    public void OnAfterDeserialize()
    {
        switch (_referenceMode)
        {
            case ReferenceMode.Local:
                Value = _localValue;
                break;
            case ReferenceMode.Global:
                Value = _globalAbility.value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    protected enum ReferenceMode
    {
        Local,
        Global
    }
}