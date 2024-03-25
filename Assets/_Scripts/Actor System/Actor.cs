using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IActor
{
    public Actor actor { get; }
}

public class Actor : MonoBehaviour
{
    [SerializeField, Required] private GameObject attributeRoot;   
    // [Header("Components")]
    // [SerializeField, Required] public ActorAttributeSet Attributes;
    // [SerializeField, Required] public ActorAbilitySet Abilities;
    // [SerializeField, Required] public ActorEffectReceiver Effects;
    // [SerializeField, Required] public ActorTagSet Tags;

    [ShowInInspector]
    public Dictionary<AttributeEnum, IAttribute> Attributes = new();

    [CanBeNull] public HealthAttribute health
    {
        get
        {
            if (Attributes.TryGetValue(AttributeEnum.HEALTH, out IAttribute health))
            {
                return health as HealthAttribute;
            }

            gameObject.GetComponents<IAttribute>();
            
            return null;
        }
    }
    public TAttribute AddAttribute<TAttribute>() where TAttribute : IAttribute
    {
        TAttribute attr = attributeRoot.GetComponentInChildren<TAttribute>();
        if (attr = attributeRoot.GetComponentInChildren<TAttribute>())
        {
            return attr;
        }
        return attributeRoot.AddComponent<TAttribute>();
    }
    
    public TAttribute GetAttribute<TAttribute>() where TAttribute : IAttribute
    {
        return attributeRoot.GetComponentInChildren<TAttribute>();
    }

    // [Header("Initialization")]
    // [Tooltip("This Actor will run these abilities whenever it spawns. Use to set initial effects and attributes")]
    // [SerializeField] private List<AbilityReference> _initAbilities;
    // [Tooltip("Cache Transform on startup and reset to it every time this Actor is re-initialized")]
    // [SerializeField] private bool _resetTransformOnInit = false;
    
    [Tooltip("E.g. summoned zombies are children of a necromancer")]
    [NonSerialized, ShowInInspector, ReadOnly] public Actor Parent;
    
    // private Vector3 _awakePosition;
    // private Quaternion _awakeRotation;
    // private Vector3 _awakeScale;
    
    // protected virtual void Awake()
    // {
    //     foreach (AbilityReference initAbilityRef in _initAbilities)
    //     {
    //         Abilities.AddAbility(initAbilityRef.Value);
    //     }
    //     
    //     _awakePosition = transform.position;
    //     _awakeRotation = transform.rotation;
    //     _awakeScale = transform.localScale;
    // }
    
    // protected virtual void OnEnable()
    // {
    //     if (_initAbilities.Count == 0)
    //     {
    //         Debug.LogWarning($"{nameof(Actor)} '{gameObject.name}' was not assigned any initialization abilities", this);
    //     }
    //     
    //     foreach (AbilityReference initAbilityRef in _initAbilities)
    //     {
    //         Abilities.TryActivateAbility(initAbilityRef.Value, new TargetData(this));
    //     }
    //     
    //     if (_resetTransformOnInit)
    //     {
    //         transform.position = _awakePosition;
    //         transform.rotation = _awakeRotation;
    //         transform.localScale = _awakeScale;
    //     }
    // }
    
    // Crude, but straightforward and works for now
    // public void Init()
    // {
    //     gameObject.SetActive(false);
    //     gameObject.SetActive(true);
    // }

    public void AddTag(Tag targetTag)
    {
        targetTag.Add(this);
    }

    public bool HasTag(Tag targetTag)
    {
        return targetTag.Contains(this);
    }
    
    public void RemoveTag(Tag targetTag)
    {
        targetTag.Remove(this);
    }
    
    // public bool TryReceiveAbility(AbilityInstance abilityInstance)
    // {
    //     if (!gameObject.activeInHierarchy) return false;  // Feel like this is always just a valid/safe check to include
    //     if (!abilityInstance.Type.CanActivate(abilityInstance.Owner, this)) return false;
    //     
    //     // Apply target effects first in case applying owner effects would disable the owner
    //     TryReceiveTargetEffects(abilityInstance);
    //     TryReceiveOwnerEffects(abilityInstance);
    //     
    //     return true;
    // }
    //
    // private bool TryReceiveTargetEffects(AbilityInstance abilityInstance)
    // {
    //     foreach (InstantEffect instantEffect in abilityInstance.Type.targetInstantEffects)
    //     {
    //         Effects.TryReceiveInstantEffect(instantEffect, abilityInstance);
    //     }
    //     
    //     foreach ((DurationEffect durationEffect, float duration) in abilityInstance.Type.targetDurationEffects)
    //     {
    //         Effects.TryReceiveDurationEffect(durationEffect, abilityInstance, duration);
    //     }
    //     
    //     foreach ((GlobalDurationEffect globalDurationEffect, float duration) in abilityInstance.Type.targetGlobalDurationEffects)
    //     {
    //         Effects.TryReceiveGlobalDurationEffect(globalDurationEffect, abilityInstance, duration);
    //     }
    //     
    //     return true;
    // }
    //
    // private bool TryReceiveOwnerEffects(AbilityInstance abilityInstance)
    // {
    //     foreach (InstantEffect instantEffect in abilityInstance.Type.ownerInstantEffects)
    //     {
    //         abilityInstance.Owner.Effects.TryReceiveInstantEffect(instantEffect, abilityInstance);
    //     }
    //     
    //     foreach ((DurationEffect durationEffect, float duration) in abilityInstance.Type.ownerDurationEffects)
    //     {
    //         abilityInstance.Owner.Effects.TryReceiveDurationEffect(durationEffect, abilityInstance, duration);
    //     }
    //     
    //     foreach ((GlobalDurationEffect globalDurationEffect, float duration) in abilityInstance.Type.ownerGlobalDurationEffects)
    //     {
    //         abilityInstance.Owner.Effects.TryReceiveGlobalDurationEffect(globalDurationEffect, abilityInstance, duration);
    //     }
    //     
    //     return true;
    // }

    private void OnDisable()
    {
        // clear attributes and tags
        // Attributes.Reset();
        // TODO Finnbarr: need a way to find all of this actor's tags so we can remove them
    }

    // protected virtual void Reset()
    // {
    //     Attributes = GetComponentInChildren<ActorAttributeSet>();
    //     Abilities = GetComponentInChildren<ActorAbilitySet>();
    //     Effects = GetComponentInChildren<ActorEffectReceiver>();
    //     Tags = GetComponentInChildren<ActorTagSet>();
    // }
}
