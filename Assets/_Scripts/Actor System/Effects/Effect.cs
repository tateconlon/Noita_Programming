using System;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unrealengine.com/5.2/en-US/gameplay-attributes-and-gameplay-effects-for-the-gameplay-ability-system-in-unreal-engine/
// https://github.com/sjai013/unity-gameplay-ability-system#gameplay-effects

[Serializable]
public abstract class Effect
{
    [SerializeReference]
    public List<AttributeModifier> modifiers = new();
    
    [Range(0f, 1f)]
    public float successChance = 1.0f;
    
    public TagInfo tagInfo;
    
    [Serializable]
    public class TagInfo
    {
        public List<Tag> grantedTags;
        public List<Tag> removedTags;
        public List<Tag> removeEffectsWithTags;
        
        [Header("Requirements")]
        public Tag.RequiredAndBlockedTags applicationTags;
        public Tag.RequiredAndBlockedTags cancellationTags;
    }
    
    public bool CanApply(Actor target)
    {
        return tagInfo.applicationTags.IsSatisfiedBy(target);
    }
}
