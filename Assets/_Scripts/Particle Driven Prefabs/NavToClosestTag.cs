using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavToClosestTag : MonoBehaviour
{
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private Tag _targetTag;
    [SerializeField] private bool _findTargetOnEnable = true;
    [SerializeField] private bool _autoFindNewTarget = true;

    private Transform _target;
    
    private void OnEnable()
    {
        if (_findTargetOnEnable)
        {
            FindTarget(Array.Empty<Actor>());
        }
    }
    
    public void FindTarget(ICollection<Actor> ignored)
    {
        if (_targetTag.TryFindClosest(transform.position, ignored, out Actor closest))
        {
            _target = closest.transform;
        }
    }
    
    private void Update()
    {
        // TODO: Need to also check if the target still has the tag we care about, if it's enabled, etc.
        
        if (_target != null && _target.gameObject.activeInHierarchy)
        {
            _agent.SetDestination(_target.position);
        }
        else
        {
            if (_autoFindNewTarget)
            {
                FindTarget(Array.Empty<Actor>());
            }
        }
    }
    
    private void Reset()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
}