using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class DurationEffect : Effect
{
    public PeriodicEffects periodicEffects;
    
    public StackingInfo stackingInfo;
    
    [Serializable]
    public class PeriodicEffects
    {
        public List<PeriodicEffect> effects;
        
        [Min(0f)]
        public float period = 0.25f;
        public bool waitOnePeriod = false;
    }
    
    /// <summary>
    /// See https://wiki.guildwars2.com/wiki/Effect_stacking
    /// </summary>
    [Serializable]
    public class StackingInfo
    {
        public StackingMode stackingMode = StackingMode.Unique;
        
        public int maxNumStacks = 25;
        public float maxStackedDuration = 30.0f;
        
        public enum StackingMode
        {
            Duration,
            Unique
        }
    }
    
    [Serializable] public class EffectToDurationDict : UnitySerializedDictionary<DurationEffect, float> { }
    [Serializable] public class DurationEffectDict : UnitySerializedDictionary<DurationEffect, List<DurationEffectInstance>> { }
}