using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIDestinationSetterGlobalGameObject : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GlobalGameObject targetGameObject;

    private void Update()
    {
        if (targetGameObject.value != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(targetGameObject.value.transform.position);
        }
    }
}
