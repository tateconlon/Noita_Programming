using System;
using Sirenix.OdinInspector;

[HideReferenceObjectPicker]
public class CounterBool
{
    [ShowInInspector, ReadOnly]
    public bool Value { get; private set; }
    
    [ShowInInspector, ReadOnly]
    public int Count { get; private set; } = 0;
    
    [ShowInInspector, ReadOnly]
    public readonly bool DefaultValue;
    
    public event Action<bool> OnChangeValue; 
    
    public CounterBool(bool defaultValue)
    {
        DefaultValue = defaultValue;
        Value = defaultValue;
    }
    
    [Button]
    public void Increment()
    {
        Count++;
        
        // 1 is the threshold count where value should change to the opposite of the default
        if (Count == 1)
        {
            Value = !DefaultValue;
            OnChangeValue?.Invoke(Value);
        }
    }
    
    [Button]
    public void Decrement()
    {
        Count--;
        
        // 0 is the threshold count where value should change back to the default
        if (Count == 0)
        {
            Value = DefaultValue;
            OnChangeValue?.Invoke(Value);
        }
    }
    
    public void Clear()
    {
        Count = 0;
        Value = DefaultValue;
    }
}