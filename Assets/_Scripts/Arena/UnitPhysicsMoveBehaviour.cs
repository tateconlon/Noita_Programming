using System;
using UnityEngine;

public abstract class UnitPhysicsMoveBehaviour : MonoBehaviour
{
    // https://forum.unity.com/threads/collisions-in-high-speed-without-unity-physic-rigidbody.1216875/#post-7763061
    [SerializeField] public Rigidbody2D rb2D;  // ONLY move with MovePosition() and MoveRotation()
    [NonSerialized] public bool shouldCollideWithWall = true;
    [NonSerialized] public float targetVelocityMagnitude = 0.0f;
    [NonSerialized] public float maxVelocityMagnitude = 3.0f;  // TODO: hardcoded for now
    [NonSerialized] public bool skipNextFixedUpdate = false;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (shouldCollideWithWall && collision.gameObject.TryGetComponent(out Wall wall))
        {
            skipNextFixedUpdate = true;
            
            ContactPoint2D contact = collision.GetContact(0);  // Only use the first point like in source code
            
            Bounce(-1.0f * contact.relativeVelocity, contact.normal, contact.point);
        }
    }

    void Bounce(Vector2 inVector, Vector2 inNormal, Vector2 contactPoint)
    {
        Vector2 outDirection = Vector2.Reflect(inVector, inNormal).normalized;
        
        rb2D.velocity = targetVelocityMagnitude * outDirection;
        rb2D.AlignRotationWithVelocity();
        
        DebugExtension.DebugArrow(contactPoint - inVector, inVector, Color.red, 1.0f, false);
        DebugExtension.DebugArrow(contactPoint, inNormal, Color.blue, 1.0f, false);
        DebugExtension.DebugArrow(contactPoint, outDirection, Color.green, 1.0f, false);
    }

    void OnDrawGizmos()
    {
        // TODO: Raycast from front of snake to test projected bounces
        // TODO: Test behavior at corners (simulate corner hit in source code to see how it works)
    }
}
