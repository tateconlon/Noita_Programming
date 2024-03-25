using System;
using Sirenix.OdinInspector;
using UnityEngine;

// These are hacky and there's probably a better way to make them, but they work for now

[Serializable]
public abstract class AttributeModifier
{
    [Required]
    public Attribute targetAttribute;
    
    public abstract float GetDelta(float curValue, EffectInstance effectInstance);
}

[Serializable]
public class AttributeAdd : AttributeModifier
{
    [SerializeField] private float _valueToAdd;
    
    public override float GetDelta(float curValue, EffectInstance effectInstance)
    {
        return _valueToAdd;
    }
}

[Serializable]
public class AttributeAddOther : AttributeModifier
{
    [SerializeField, Required] private Attribute _otherAttribute;
    [SerializeField] private float _multiplier = 1.0f;
    
    public override float GetDelta(float curValue, EffectInstance effectInstance)
    {
        // if (effectInstance.Owner.Attributes.TryGetAttributeValue(_otherAttribute, out float value))
        // {
        //     return value * _multiplier;
        // }
        //
        return 0;
    }
}

[Serializable]
public class AttributeMultiply : AttributeModifier
{
    [SerializeField] private float _multiplier = 1.0f;
    
    public override float GetDelta(float curValue, EffectInstance effectInstance)
    {
        return curValue * (_multiplier - 1.0f);  // Subtract 1 from multiple because we're returning a delta
    }
}

[Serializable]
public class AttributeSet : AttributeModifier
{
    [SerializeField] private float _newValue;
    
    public override float GetDelta(float curValue, EffectInstance effectInstance)
    {
        return _newValue - curValue;
    }
}

[Serializable]
public class AttributeSetToOther : AttributeModifier
{
    [SerializeField, Required] private Attribute _otherAttribute;
    
    public override float GetDelta(float curValue, EffectInstance effectInstance)
    {
        // if (effectInstance.Owner.Attributes.TryGetAttributeValue(_otherAttribute, out float value))
        // {
        //     return value - curValue;
        // }
        
        return 0;
    }
}