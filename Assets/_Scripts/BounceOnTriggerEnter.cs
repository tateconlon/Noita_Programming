using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BounceOnTriggerEnter : MonoBehaviour
{
    [SerializeField, Required] private AttributeComponent _pierceAttribute;
    [SerializeField, Required] private AttributeComponent _bounceAttribute;
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private LayerMask _bounceLayers;
    [SerializeField] private Tag.RequiredAndBlockedTags _bounceTags;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Use up pierces before we start bouncing
        if (_pierceAttribute.curValue > 0 || _bounceAttribute.curValue < 0) return;
        
        if (!_bounceLayers.IncludesLayer(other.gameObject.layer)) return;
        if (!other.TryGetComponent(out Actor actor)) return;
        if (!_bounceTags.IsSatisfiedBy(actor)) return;
        
        ColliderDistance2D colliderDistance2D = _rb2d.Distance(other);

        if (colliderDistance2D.isValid)
        {
            // ensure normal is pointing IN to the other collider
            Vector2 inNormal = colliderDistance2D.normal;
            if (colliderDistance2D.distance > 0f)
            {
                inNormal *= -1.0f;
            }

            Vector2 outDirection = Vector2.Reflect(_rb2d.velocity, inNormal);

            _rb2d.velocity = outDirection.normalized * _rb2d.velocity.magnitude;
            _rb2d.rotation = Vector2.SignedAngle(Vector2.up, _rb2d.velocity);
        }
        else
        {
            Debug.LogWarning($"Invalid bounce attempt, is {other.name} disabled?", this);
        }
    }
    
    private void Reset()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }
}