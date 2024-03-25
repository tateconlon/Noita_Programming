using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// TODO: Tags, AddTag, and RemoveTag shouldn't be modified/called except by HTag. Make access to them internal?
/// <summary>
/// For all uses cases (except for listening to add/remove events) prefer the GameObject extension methods over directly
/// using this class - keeps things simple and consistent by allowing other code to focus on GameObjects.
/// </summary>
public class HTagHolder : MonoBehaviour
{
    /// <summary>
    /// Should not be modified outside of this class
    /// </summary>
    [NonSerialized, ShowInInspector, ReadOnly]
    public HashSet<HTag> Tags = new();
    
    public event Action<HTag> OnTagAdded;
    public event Action<HTag> OnTagRemoved;
    
    /// <summary>
    /// Should only call from HTag
    /// </summary>
    public void AddTag(HTag hTag)
    {
        Tags.Add(hTag);
        OnTagAdded?.Invoke(hTag);
    }
    
    /// <summary>
    /// Should only call from HTag
    /// </summary>
    public void RemoveTag(HTag hTag)
    {
        Tags.Remove(hTag);
        OnTagRemoved?.Invoke(hTag);
    }
    
    private void OnDisable()
    {
        foreach (HTag hTag in Tags)
        {
            hTag.Clear(this);
        }
        
        Tags.Clear();
    }
}

