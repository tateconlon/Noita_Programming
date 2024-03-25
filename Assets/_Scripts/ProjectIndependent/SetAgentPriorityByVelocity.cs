using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SetAgentPriorityByVelocity : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    
    private Coroutine _updatePrioritiesCoroutine;
    
    private static readonly Vector2 DelayBetweenUpdates = new(0.75f, 1.25f);
    private static readonly List<SetAgentPriorityByVelocity> Instances = new();

    private void OnEnable()
    {
        Instances.Add(this);
        
        _updatePrioritiesCoroutine = StartCoroutine(UpdatePriorities());
    }

    private IEnumerator UpdatePriorities()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(DelayBetweenUpdates.x, DelayBetweenUpdates.y));

            if (Instances[0] == this)  // Delegate the sorting to the first instance in the list
            {
                SetPrioritiesByVelocities();
            }
        }
    }

    private static void SetPrioritiesByVelocities()
    {
        Instances.Sort((agentA, agentB) => 
            agentA._agent.velocity.sqrMagnitude.CompareTo(agentB._agent.velocity.sqrMagnitude));

        for (int i = 0; i < Instances.Count; i++)
        {
            Instances[i]._agent.avoidancePriority = i;
        }
    }
    
    private void OnDisable()
    {
        Instances.Remove(this);
        
        if (_updatePrioritiesCoroutine != null)
        {
            StopCoroutine(_updatePrioritiesCoroutine);
        }
    }

    private void Reset()
    {
        if (TryGetComponent(out NavMeshAgent agent))
        {
            _agent = agent;
        }
    }
}
