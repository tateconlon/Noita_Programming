using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes this GameObject move using a dynamic Rigidbody that has forces added to it by a NavMeshAgent. The end result
/// is that the pathfinding system provides movement input, but ultimately the GameObject is moved by the physics
/// system and can therefore be affected by other forces like knockback, explosions, etc.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class DriveRigidbodyWithAgent : MonoBehaviour
{
    [SerializeField, Required] private Rigidbody2D _rb2d;
    [SerializeField, Required] private NavMeshAgent _agent;
    
    private RigidbodyType2D _prevBodyType;
    private bool _prevAgentUpdatePos;
    
    private void OnEnable()
    {
        _prevBodyType = _rb2d.bodyType;
        _rb2d.bodyType = RigidbodyType2D.Dynamic;

        _prevAgentUpdatePos = _agent.updatePosition;
        _agent.updatePosition = false;
    }
    
    private void Update()
    {
        _agent.nextPosition = transform.position;  // This position will have Rigidbody interpolation applied
    }
    
    private void FixedUpdate()
    {
        // This ignores the agent trying to decelerate to stop exactly at its destination; we want enemies to move 
        // at full speed all the time so that they don't slow down as they near the player.
        Vector2 acceleration = 0.2f * _agent.acceleration * ((Vector2)_agent.desiredVelocity).normalized;
        Vector2 force = _rb2d.mass * acceleration;  // Force = mass * acceleration
        
        _rb2d.AddForce(force, ForceMode2D.Force);  // Use ForceMode2D.Force to add force over time
    }
    
    private void OnDisable()
    {
        _rb2d.bodyType = _prevBodyType;
        _agent.updatePosition = _prevAgentUpdatePos;
    }
    
    private void Reset()
    {
        _rb2d = GetComponentInParent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
    }
}