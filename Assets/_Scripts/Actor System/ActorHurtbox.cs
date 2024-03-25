using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Represents a hurtbox that can modify the effects of received abilities.
/// </summary>
/// <example>
/// A "Head" hurtbox for a humanoid character can modify damage taken by 2 and make received attacks always be critical
/// </example>
/// <remarks>
/// Using dedicated hurtboxes on child GameObjects is needed in most cases so that the root Actor GameObject can use
/// a Collider for physics/movement that interacts with different layers than hitboxes/hurtboxes do.
/// Inspired by <a href="https://docs.neofps.com/manual/healthref-mb-basicdamagehandler.html">NeoFPS DamageHandler</a>
/// </remarks>
[RequireComponent(typeof(Collider2D))]
public class ActorHurtbox : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    
    public Actor Owner => _actor;
    
    // TODO: have a wrapper method to apply an Ability to the owner Actor with modifications unique to this hurtbox
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
    }
}