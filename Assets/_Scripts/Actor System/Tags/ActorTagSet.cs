using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ActorTagSet : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    
    [Tooltip("Add default tags through an initialization Ability, not here")]
    [SerializeField, ReadOnly] private TagRefCountDict _tagRefCounts = new();
    
    public delegate void ActorTagSetModifiedDelegate(ActorTagSet actorTagSet);

    [ShowInInspector]
    public event ActorTagSetModifiedDelegate OnChangeCurValue;
    
    public void Add(Tag actorTag)
    {
        _tagRefCounts.Increment(actorTag);
        
        // Ref count below 1 means the tag shouldn't be added.
        // Ref count above 1 means the tag has already been added.
        if (_tagRefCounts[actorTag] == 1)
        {
            actorTag.Add(_actor);
            
            OnChangeCurValue?.Invoke(this);
        }
    }
    
    public void Remove(Tag actorTag)
    {
        _tagRefCounts.Decrement(actorTag);
        
        // Ref count above 0 means the tag should remain added.
        // Ref count below 0 means the tag has already been removed.
        if (_tagRefCounts[actorTag] == 0)  
        {
            actorTag.Remove(_actor);
            
            OnChangeCurValue?.Invoke(this);
        }
    }
    
    public bool Matches(Tag targetTag)
    {
        return targetTag.Contains(_actor);
    }
    
    private void OnDisable()
    {
        foreach ((Tag actorTag, int refCount) in _tagRefCounts)
        {
            if (refCount > 0)
            {
                actorTag.Remove(_actor);
            }
        }
        
        _tagRefCounts.Clear();
    }
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
    
    [Serializable] private class TagRefCountDict : UnitySerializedDictionary<Tag, int> { }
}
