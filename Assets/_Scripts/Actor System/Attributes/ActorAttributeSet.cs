using Sirenix.OdinInspector;
using UnityEngine;

public class ActorAttributeSet : MonoBehaviour, ISelfValidator
{
    [SerializeField, Required] private Actor _actor;
    [SerializeField] private AttributeDict _attributes;
    
    public void ExecuteEffect(InstantEffectInstance effectInstance)
    {
        foreach (AttributeModifier attributeModifier in effectInstance.type.modifiers)
        {
            if (!_attributes.TryGetValue(attributeModifier.targetAttribute, out AttributeComponent attribute)) continue;
            
            attribute.ExecuteModifier(attributeModifier, effectInstance);
        }
    }
    
    public void AddEffect(DurationEffectInstance effectInstance)
    {
        foreach (AttributeModifier attributeModifier in effectInstance.type.modifiers)
        {
            if (!_attributes.TryGetValue(attributeModifier.targetAttribute, out AttributeComponent attribute)) continue;
            
            attribute.AddModifier(attributeModifier, effectInstance);
        }
    }
    
    public void RemoveEffect(DurationEffectInstance effectInstance)
    {
        foreach (AttributeModifier attributeModifier in effectInstance.type.modifiers)
        {
            if (!_attributes.TryGetValue(attributeModifier.targetAttribute, out AttributeComponent attribute)) continue;
            
            attribute.RemoveModifier(attributeModifier, effectInstance);
        }
    }
    
    public bool TryGetAttributeValue(Attribute attribute, out float value)
    {
        if (_attributes.TryGetValue(attribute, out AttributeComponent attributeComponent))
        {
            value = attributeComponent.curValue;
            return true;
        }
        
        value = 0f;
        return false;
    }
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
    
    public void Validate(SelfValidationResult result)
    {
        foreach ((Attribute attribute, AttributeComponent attributeComponent) in _attributes)
        {
            if (attribute == null)
            {
                result.AddError($"Must assign {nameof(Attribute)}");
            }

            if (attributeComponent == null)
            {
                result.AddError($"Must assign {nameof(AttributeComponent)}");
            }
            else
            {
                if (attributeComponent.type != attribute)
                {
                    result.AddError($"{nameof(Attribute)} and {nameof(AttributeComponent)} {nameof(Attribute)} must match");
                }
            }
        }
    }
}
