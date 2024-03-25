using System.Collections.Generic;

public static class CollectionExtensions
{
    public static void Increment<T>(this Dictionary<T, int> dictionary, T key)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] += 1;
        }
        else
        {
            dictionary[key] = 1;
        }
    }
    
    public static void Decrement<T>(this Dictionary<T, int> dictionary, T key)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] -= 1;
        }
        else
        {
            dictionary[key] = -1;
        }
    }
    
    public static TValue GetValueOrDefault<TKey,TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out TValue foundValue) ? foundValue : default;
    }
    
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
    {
        if (dictionary.TryGetValue(key, out TValue existingValue)) return existingValue;
        
        TValue newValue = new();
        dictionary[key] = newValue;
        return newValue;
    }
}
