using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

// public static class Tags
// {
//     public static EliteTag Elite;
//     public static UnitTag Unit;
//     public static GruntTag Grunt;
//     public static FireTag Fire;
//     public static AttackTag Attack;
//     public static WaterTag Water;
//
//     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     public static void Init()
//     {
//         Elite = new EliteTag();
//         Unit = new UnitTag();
//         Grunt = new GruntTag();
//         Fire = new FireTag();
//         Attack = new AttackTag();
//         Water = new WaterTag();
//
//         // ---- ----
//         Unit._parentTag = null;
//         Grunt._parentTag = Unit;
//         Elite._parentTag = Unit;
//         
//         Fire._parentTag = null;
//         Water._parentTag = null;
//     }
// }

public class Tag : ScriptableObject, IReadOnlyCollection<Actor>
{
    [SerializeField] public Tag _parentTag;
    
    [ShowInInspector, ReadOnly, NonSerialized]
    private HashSet<Actor> _actorsWithTag = new();

    public int Count => _actorsWithTag.Count;
    
    public event Action<Actor> OnAddActor;
    public event Action<Actor> OnRemoveActor;

    //FINNBARR: Need to refactor to add count, Dictionary<Tag, int>???
    public virtual bool Add(Actor actor)
    {
        // Early return also helps prevent infinite recursion if this Tag is somehow its own parent
        if (_actorsWithTag.Contains(actor)) return false;
        
        _actorsWithTag.Add(actor);
        OnAddActor?.Invoke(actor);
        
        if (_parentTag != null)
        {
            _parentTag.Add(actor);
        }
        
        return true;
    }
    
    /// <summary>
    /// TODO: needs to be ref counted or something so that removing tags won't unintentionally remove parent tags.
    /// E.g. if an Actor has StatusEffect.Burning and StatusEffect.Poisoned, removing Burning shouldn't also remove the
    /// StatusEffect parent tag because Poisoned is still present.
    /// </summary>
    ///  //FINNBARR: Need to refactor to subtract count, Dictionary<Tag, int>???
    public bool Remove(Actor actor)
    {
        // Early return also helps prevent infinite recursion if this Tag is somehow its own parent
        if (!_actorsWithTag.Contains(actor)) return false;
        
        _actorsWithTag.Remove(actor);
        OnRemoveActor?.Invoke(actor);
        
        if (_parentTag != null)
        {
            _parentTag.Remove(actor);
        }
        
        return true;
    }
    
    public bool Contains(Actor actor)
    {
        return _actorsWithTag.Contains(actor);
    }
    
    public IEnumerator<Actor> GetEnumerator()
    {
        return _actorsWithTag.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_actorsWithTag).GetEnumerator();
    }
    
    // Inspired by example from: https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
    public bool TryFindClosest(Vector3 queryPos, ICollection<Actor> ignored, out Actor closest)
    {
        closest = null;
        float distance = Mathf.Infinity;
        bool foundMatch = false;
        
        foreach (Actor actor in _actorsWithTag)
        {
            if (ignored.Contains(actor)) continue;
            
            float curDistance = (actor.transform.position - queryPos).sqrMagnitude;
            
            if (curDistance < distance)
            {
                closest = actor;
                distance = curDistance;
                foundMatch = true;
            }
        }
        
        return foundMatch;
    }
    
    public void Validate(SelfValidationResult result)
    {
        Tag curTag = _parentTag;
        
        while (curTag != null)
        {
            if (curTag == this)
            {
                result.AddError("Circular tag hierarchy detected (this tag is its own parent)");
                return;
            }
            
            curTag = curTag._parentTag;
        }
    }
    
    [Serializable]
    public class RequiredAndBlockedTags
    {
        public List<Tag> requiredTags;
        public List<Tag> blockedTags;
        
        public bool IsSatisfiedBy([CanBeNull] Actor actor)
        {
            if (actor == null) return true;

            return true;//IsSatisfiedBy(actor.Tags);
        }
        
        public bool IsSatisfiedBy(ActorTagSet actorTags)
        {
            foreach (Tag requiredTag in requiredTags)
            {
                if (!actorTags.Matches(requiredTag)) return false;
            }
            
            foreach (Tag blockedTag in blockedTags)
            {
                if (actorTags.Matches(blockedTag)) return false;
            }
            
            return true;
        }
    }
}

//We can't inherit because then all the lists are the same, so there's no seperate
//EliteTag list and UnitTag list
public class EliteTag : UnitTag
{
    public override bool Add(Actor actor)
    {
        _parentTag.Add(actor);
        
        Debug.Log("Nothin'");
        return false;
    }
}

public class UnitTag : Tag
{
}

public class GruntTag : Tag
{
}


public class FireTag : Tag
{
}

public class WaterTag : Tag
{
}

public class AttackTag : Tag
{
}






[Serializable] public class TagHashSet : UnitySerializedHashSet<Tag> { }