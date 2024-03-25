using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Called before the curValue of an Attribute is changed. Use to apply preprocessing like clamping the new value.
/// </summary>
[HelpURL("https://github.com/tranek/GASDocumentation#concepts-as-preattributechange")]
[Serializable]
public abstract class PreAttributeChangeModifier
{
    public abstract float Modify(float prev, float value);
}

// This could be generalized to work with other effects too, e.g. don't damage health if shield points aren't zero
[Serializable]
public class PreBounceChangeModifier : PreAttributeChangeModifier
{
    [SerializeField, Required] private AttributeComponent _pierceAttribute;
    
    public override float Modify(float prev, float value)
    {
        // Don't start bouncing until pierce has run out, like in 20MTD
        // https://minutes-till-dawn.fandom.com/wiki/Bounce
        if (_pierceAttribute.curValue > 0)
        {
            return prev;  
        }
        
        return value;
    }
}