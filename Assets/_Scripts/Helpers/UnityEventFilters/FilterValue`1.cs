using System;
using Sirenix.OdinInspector;

[Serializable]
public class FilterValue<T>
{
    public bool filter;
    
    [ShowIf(nameof(filter))]
    public T value;

    public bool Matches(T testValue)
    {
        return !filter || value.Equals(testValue);
    }
}