using System;
using UnityEngine;

/// <summary>
/// For buffs/debuffs/status effects that will be shared by multiple abilities, e.g. burning, poisoned, crippled
/// </summary>
[CreateAssetMenu(fileName = "Global Duration Effect", menuName = "ScriptableObject/Actor System/Global Duration Effect", order = 0)]
public class GlobalDurationEffect : ScriptableObject
{
    public string displayName = "Global Duration Effect";
    public Sprite icon;
    public DurationEffect effect;
    
    [Serializable] public class EffectToDurationDict : UnitySerializedDictionary<GlobalDurationEffect, float> { }
}