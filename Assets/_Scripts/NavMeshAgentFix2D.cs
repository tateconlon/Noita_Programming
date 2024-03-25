using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentFix2D : MonoBehaviour
{
    private void Awake()
    {
        if (TryGetComponent(out NavMeshAgent agent))
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;
        }
        else
        {
            Debug.LogException(new MissingComponentException($"No {nameof(NavMeshAgent)} found on {nameof(NavMeshAgentFix2D)}"), gameObject);
        }
        
        Destroy(this);
    }
}
