using System.Collections.Generic;
using UnityEngine;

public static class HTagExtensions
{
    public static void AddHTag(this GameObject gameObject, HTag hTag)
    {
        if (!gameObject.TryGetComponent(out HTagHolder hTagHolder))
        {
            Debug.LogWarning($"No {nameof(hTagHolder)} found on {gameObject.name}", gameObject);
            
            hTagHolder = gameObject.AddComponent<HTagHolder>();  // Add a tag holder so we can add the tag as requested
        }
        
        hTag.Add(hTagHolder);
    }
    
    public static void RemoveHTag(this GameObject gameObject, HTag hTag)
    {
        if (!gameObject.TryGetComponent(out HTagHolder hTagHolder))
        {
            Debug.LogWarning($"No {nameof(hTagHolder)} found on {gameObject.name}", gameObject);
        }
        
        hTag.Remove(hTagHolder);
    }
    
    public static bool HasHTag(this GameObject gameObject, HTag hTag)
    {
        return hTag.GameObjects.Contains(gameObject);
    }
    
    public static HashSet<HTag> GetHTags(this GameObject gameObject)
    {
        if (!gameObject.TryGetComponent(out HTagHolder hTagHolder))
        {
            Debug.LogWarning($"No {nameof(hTagHolder)} found on {gameObject.name}", gameObject);
            return new HashSet<HTag>();
        }
        
        return hTagHolder.Tags;
    }
    
    public static bool IsTaggedObjectNearby(HTag hTag, Vector2 searchOrigin, float searchDistance)
    {
        foreach (GameObject gameObject in hTag.GameObjects)
        {
            if (Vector2.Distance(searchOrigin, gameObject.transform.position) < searchDistance)
            {
                return true;
            }
        }
        
        return false;
    }
}
