using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Collects hits on Hurtboxes over a frame and then fires an event with all data.
/// One entity can have multiple hitboxes each with different data, see:
/// <a href="https://ultimate-hitboxes.com/donkey-kong/DonkeyKongFAir">Smash hitboxes</a>
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField, Required] public GameObject Owner;
    [SerializeField, Required] public Rigidbody2D Rb2d;
    [SerializeField, Required] public Collider2D Collider;
    
    private readonly HashSet<Hurtbox> _hurtboxesHitThisFrame = new();
    private Coroutine _activateOnAllTargetsCoroutine = null;
    
    private ContactFilter2D _overlapContactFilter;
    private readonly List<Collider2D> _overlapResults = new();
    
    public event Action<Hitbox, HashSet<Hurtbox>> OnHit;
    
    private void Awake()
    {
        InitContactFilter();
    }
    
    private void InitContactFilter()
    {
        _overlapContactFilter.NoFilter();
        
        _overlapContactFilter.useTriggers = Collider.isTrigger;
        _overlapContactFilter.useLayerMask = true;
        _overlapContactFilter.layerMask = Physics2D.GetLayerCollisionMask(Collider.gameObject.layer);
    }
    
    /// <summary>
    /// Find all Colliders currently inside and process them as usual whenever we enable the hitbox
    /// </summary>
    private void OnEnable()
    {
        int numHits = Rb2d.OverlapCollider(_overlapContactFilter, _overlapResults);
        
        for (int i = 0; i < numHits; i++)
        {
            OnTriggerEnter2D(_overlapResults[i]);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled) return;  // Necessary because trigger events are sent to disabled MonoBehaviors by design
        if (!other.TryGetComponent(out Hurtbox hurtbox)) return;
        
        _hurtboxesHitThisFrame.Add(hurtbox);
        
        if (_activateOnAllTargetsCoroutine == null)
        {
            _activateOnAllTargetsCoroutine = StartCoroutine(CollectHitsOverFrame());
        }
    }
    
    private IEnumerator CollectHitsOverFrame()
    {
        yield return new WaitForFixedUpdate();  // This delays until all OnTrigger events have been called this frame
        
        // Create and pass a new collection in case the original is modified in the meantime (e.g. new GameObjects spawning)
        OnHit?.Invoke(this, new HashSet<Hurtbox>(_hurtboxesHitThisFrame));
        
        _hurtboxesHitThisFrame.Clear();
        _activateOnAllTargetsCoroutine = null;
    }
    
    private void OnDisable()
    {
        if (_activateOnAllTargetsCoroutine != null)
        {
            StopCoroutine(CollectHitsOverFrame());
            _activateOnAllTargetsCoroutine = null;
        }
        
        _hurtboxesHitThisFrame.Clear();
    }
    
    private void Reset()
    {
        Rb2d = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
    }
}
