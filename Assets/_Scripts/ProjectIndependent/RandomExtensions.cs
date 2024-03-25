using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RandomExtensions
{
    /// <summary>
    /// Returns true at the given chance.
    /// </summary>
    /// <example>
    /// <code>
    /// RandomBool(0.50) -> returns true 50% of the time
    /// RandomBool(0.25) -> returns true 25% of the time
    /// RandomBool(0.03) -> returns true 3% of the time
    /// </code>
    /// </example>
    // Ported from the SNKRX Random:bool() method
    public static bool Bool(float chance = 0.5f)
    {
        return Random.Range(0f, 1f) < chance;
    }
    
    public static T WeightedPick<T>(this IEnumerable<T> iEnumerable, Func<T, float> weightSelector)
    {
        List<T> list = iEnumerable.ToList();
        
        float targetWeight = Random.Range(0.0f, list.Sum(weightSelector));
        
        foreach (T item in list)
        {
            float weight = weightSelector(item);
            
            if (targetWeight < weight)
            {
                return item;
            }

            targetWeight -= weight;
        }
        
        return list[list.Count - 1];
    }
    
    public static TKey WeightedPick<TKey, TValue>(this IDictionary<TKey, TValue> iDictionary, 
        Func<KeyValuePair<TKey, TValue>, float> weightSelector)
    {
        float targetWeight = Random.Range(0.0f, iDictionary.Sum(weightSelector));

        foreach (KeyValuePair<TKey,TValue> pair in iDictionary)
        {
            float weight = weightSelector(pair);
            
            if (targetWeight < weight)
            {
                return pair.Key;
            }

            targetWeight -= weight;
        }

        return iDictionary.LastOrDefault().Key;
    }
    
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count <= 0)
        {
            Debug.LogException(new IndexOutOfRangeException("GetRandom called on empty List"));
            return default;
        }

        return list[Random.Range(0, list.Count)];
    }
    
    public static T GetRandom<T>(this T[] array)
    {
        if (array.Length <= 0)
        {
            Debug.LogException(new IndexOutOfRangeException("GetRandom called on empty array"));
            return default;
        }

        return array[Random.Range(0, array.Length)];
    }

    public static Vector3 RandomPoint(this Bounds bounds)
    {
        return new Vector3(
            Random.Range(-bounds.extents.x, bounds.extents.x),
            Random.Range(-bounds.extents.y, bounds.extents.y),
            Random.Range(-bounds.extents.z, bounds.extents.z)
        ) + bounds.center;
    }
    
    public static Vector3 RandomPoint(this BoxCollider boxCollider)
    {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(
            Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y),
            Random.Range(-extents.z, extents.z)
        )  + boxCollider.center;
        return boxCollider.transform.TransformPoint(point);
    }

    public static float RandomInRange(this Vector2 vector2)
    {
        return Random.Range(vector2.x, vector2.y);
    }

    public static int RandomInRangeRounded(this Vector2 vector2)
    {
        return Mathf.RoundToInt(vector2.RandomInRange());
    }
}
