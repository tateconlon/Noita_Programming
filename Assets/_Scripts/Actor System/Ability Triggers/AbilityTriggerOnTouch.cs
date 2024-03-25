using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AbilityTriggerOnTouch : AbilityTrigger
{
    private readonly HashSet<Actor> _actorsHitThisFrame = new();
    private Coroutine _activateOnAllTargetsCoroutine = null;
    
    private readonly HashSet<Actor> _ignoredActors = new();
    
    public void IgnoreActor(Actor actorToIgnore)
    {
        _ignoredActors.Add(actorToIgnore);
    }
    
    private bool TryGetValidActor(Collider2D other, out Actor actor)
    {
        if (!other.TryGetComponent(out ActorHurtbox actorHurtbox))
        {
            actor = null;
            return false;
        }
        
        actor = actorHurtbox.Owner;
        
        return !_ignoredActors.Contains(actor);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled) return;
        if (!TryGetValidActor(other, out Actor targetActor)) return;
        
        _actorsHitThisFrame.Add(targetActor);
        
        if (_activateOnAllTargetsCoroutine == null)
        {
            _activateOnAllTargetsCoroutine = StartCoroutine(ActivateAbilityOnAllTargets());
        }
    }

    private IEnumerator ActivateAbilityOnAllTargets()
    {
        yield return new WaitForFixedUpdate();  // This delays until all OnTrigger/OnCollision events have been called
        
        // Create and pass a new list in case _actorsHitThisFrame is modified in the meantime (e.g. new Actors spawning)
        ActivateAbilities(new TargetData(_actorsHitThisFrame));
        
        _actorsHitThisFrame.Clear();
        
        _activateOnAllTargetsCoroutine = null;
    }

    private void OnDisable()
    {
        if (_activateOnAllTargetsCoroutine != null)
        {
            StopCoroutine(ActivateAbilityOnAllTargets());
            _activateOnAllTargetsCoroutine = null;
        }
        
        _actorsHitThisFrame.Clear();
        _ignoredActors.Clear();
    }
}