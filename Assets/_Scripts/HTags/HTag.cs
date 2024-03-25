using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

[SuppressMessage("ReSharper", "InconsistentNaming")]  // To allow underscores in names
public static class HTags
{
    public static readonly HTag Player = new(name: "Player");
    public static readonly HTag Enemy = new(name: "Enemy");
    public static readonly HTag Enemy_Elite = new(Enemy, "Enemy_Elite");
    public static readonly HTag Enemy_Elite_Boss = new(Enemy_Elite, "Enemy_Elite_Boss");
    public static readonly HTag Flying = new(name: "Flying");
    public static readonly HTag DespawnOffScreen = new(name: "DespawnOffScreen");
    public static readonly HTag Pickup = new(name: "Pickup");
}

public class HTag
{
    private readonly HTag _parent;

    public readonly string name;    //For debugging
    
    private readonly HashSet<GameObject> _taggedGameObjects = new();
    private readonly Dictionary<HTagHolder, int> _taggedGameObjectCounts = new();
    
    /// <summary>
    /// Should not be modified outside this class
    /// </summary>
    public HashSet<GameObject> GameObjects => _taggedGameObjects;
    
    public event Action<GameObject> OnAddGameObject;
    public event Action<GameObject> OnRemoveGameObject;
    
    public HTag(HTag parent = null, string name = "")
    {
        _parent = parent;
        this.name = name;
    }

    public void Add(HTagHolder tagHolder)
    {
        _taggedGameObjectCounts.Increment(tagHolder);
        
        // If below 1, remain removed. If above 1, already added.
        if (_taggedGameObjectCounts[tagHolder] == 1)
        {
            _taggedGameObjects.Add(tagHolder.gameObject);
            tagHolder.AddTag(this);
            OnAddGameObject?.Invoke(tagHolder.gameObject);
        }
        
        _parent?.Add(tagHolder);
    }
    
    public void Remove(HTagHolder tagHolder)
    {
        _taggedGameObjectCounts.Decrement(tagHolder);
        
        // If below 0, already removed. If above 0, remain added.
        if (_taggedGameObjectCounts[tagHolder] == 0)
        {
            _taggedGameObjects.Remove(tagHolder.gameObject);
            tagHolder.RemoveTag(this);
            OnRemoveGameObject?.Invoke(tagHolder.gameObject);
        }
        
        _parent?.Remove(tagHolder);
    }
    
    public void Clear(HTagHolder tagHolder)
    {
        if (!_taggedGameObjects.Contains(tagHolder.gameObject))
        {
            //RACE CONDITION: When we clear a tag, we clear the parent tag.
            //However a gameobject tries to clear all its tags when it is disabled.
            //When we add a tag, we add the parent tag.
            //So in the HTagHolder's OnDisable foreach loop, first we clear the child which clears the parent.
            //Then loop continues and gets to the parent tag, which is already cleared.
            //Eg: foreach(): Clear(Elite->Enemy), Clear(Enemy)!! This is already cleared!!
            Debug.LogWarning($"Attempting to clear {name} from {tagHolder.gameObject.name} but it is not tagged {name}." +
                             $"This tag may be a parent of a previously cleared tag", tagHolder.gameObject);
            return;
        }
        if (_taggedGameObjectCounts[tagHolder] > 0)
        {
            _taggedGameObjects.Remove(tagHolder.gameObject);
            OnRemoveGameObject?.Invoke(tagHolder.gameObject);
        }
        
        _taggedGameObjectCounts.Remove(tagHolder);
        
        _parent?.Clear(tagHolder);
    }
}
