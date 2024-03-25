using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// https://ck.uesp.net/wiki/Actor_Value
// https://docs.unrealengine.com/5.0/en-US/gameplay-attributes-and-attribute-sets-for-the-gameplay-ability-system-in-unreal-engine/

public class AttributeComponent : MonoBehaviour
{
    public Attribute type;
    
    [SerializeReference] private List<PreAttributeChangeModifier> _preAttributeChangeModifiers;
    
    [ShowInInspector, ReadOnly]
    public float curValue { get; private set; } = 0f;
    
    [NonSerialized, ShowInInspector, ReadOnly] private float _baseValue = 0f;
    
    [NonSerialized, ShowInInspector, ReadOnly]
    private Dictionary<DurationEffectInstance, List<AttributeModifier>> _curValueModifierStack = new();
    
    [ShowInInspector]
    public event Action<ChangeValueParams> OnChangeCurValue;
    
    [Button]
    public void ExecuteModifier(AttributeModifier attributeModifier, InstantEffectInstance effectInstance)
    {
        // TODO: implement crit in the calculation like: https://github.com/tranek/GASDocumentation#cae-crit
        // (maybe using a PreAttributeChangeModifier if a "Can Crit" tag is present that fires a Crit event)
        
        _baseValue += attributeModifier.GetDelta(curValue, effectInstance);
        
        RecalculateCurValue(effectInstance);
    }

    public void AddModifier(AttributeModifier attributeModifier, DurationEffectInstance effectInstance)
    {
        _curValueModifierStack.GetOrCreate(effectInstance).Add(attributeModifier);
        
        RecalculateCurValue(effectInstance);
    }
    
    public void RemoveModifier(AttributeModifier attributeModifier, DurationEffectInstance effectInstance)
    {
        if (!_curValueModifierStack.TryGetValue(effectInstance, out List<AttributeModifier> modifiers)) return;
        
        modifiers.Remove(attributeModifier);
        
        RecalculateCurValue(effectInstance);
    }
    
    private void RecalculateCurValue(EffectInstance effectInstance)
    {
        float prev = curValue;
        
        float tempValue = _baseValue;  // I use a temp value so I can know that I only change curValue in one place
        
        foreach (List<AttributeModifier> modifiers in _curValueModifierStack.Values)
        {
            foreach (AttributeModifier modifier in modifiers)
            {
                tempValue += modifier.GetDelta(curValue, effectInstance);
            }
        }
        
        foreach (PreAttributeChangeModifier preAttributeChangeModifier in _preAttributeChangeModifiers)
        {
            tempValue = preAttributeChangeModifier.Modify(prev, tempValue);
        }
        
        float deltaUnclamped = tempValue - prev;
        curValue = Mathf.Clamp(tempValue, type.valueMinMax.x, type.valueMinMax.y);
        
        OnChangeCurValue?.Invoke(new ChangeValueParams(prev, curValue, deltaUnclamped, effectInstance));
    }
    
    // TODO: need to ensure this is being called after everything else so values can't be modified after this cleanup
    private void OnDisable()
    {
        // Fully reset state so initialization always leads to the same results
        _baseValue = 0f;
        _curValueModifierStack.Clear();
        curValue = 0f;
    }
    
    public readonly struct ChangeValueParams
    {
        public readonly float PrevValue;
        public readonly float NewValue;
        public readonly float DeltaUnclamped;
        public readonly EffectInstance Effect;
        
        public ChangeValueParams(AttributeComponent attributeComponent)
        {
            PrevValue = attributeComponent.curValue;
            NewValue = attributeComponent.curValue;
            DeltaUnclamped = 0f;
            Effect = null;
        }
        
        public ChangeValueParams(float prevValue, float newValue, float deltaUnclamped, EffectInstance effect)
        {
            PrevValue = prevValue;
            NewValue = newValue;
            DeltaUnclamped = deltaUnclamped;
            Effect = effect;
        }
    }
}