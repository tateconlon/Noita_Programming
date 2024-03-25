using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D), typeof(NavMeshObstacle))]
public class AgentPhysicsControlHandler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private NavMeshObstacle _obstacle;
    
    private MovementSource _movementSource = MovementSource.None;
    
    private void FixedUpdate()
    {
        if (_rigidbody2D.velocity.magnitude > _agent.acceleration)
        {
            SetIsPhysicsDriven();
        }
        else
        {
            SetIsAgentDriven();
        }
    }

    private void SetIsPhysicsDriven()
    {
        if (_movementSource == MovementSource.Physics) return;
        _movementSource = MovementSource.Physics;
        
        _agent.enabled = false;
        _obstacle.enabled = true;
    }

    private void SetIsAgentDriven()
    {
        if (_movementSource == MovementSource.Agent) return;
        _movementSource = MovementSource.Agent;
        
        _obstacle.enabled = false;
        _agent.enabled = true;
        
        // Zero out the RigidBody2D
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
    }
    
    private void Reset()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _obstacle = GetComponent<NavMeshObstacle>();
    }
    
    private enum MovementSource
    {
        None,
        Agent,
        Physics
    }
}
