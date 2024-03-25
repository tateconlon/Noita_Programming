using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AimAtClosestTag : MonoBehaviour
{
    [SerializeField, Required] private Tag _targetTag;
    [SerializeField] private bool _aimOnEnable = false;
    [SerializeField] private bool _aimOnUpdate = false;
    
    private void OnEnable()
    {
        if (_aimOnEnable)
        {
            AimAtClosest(Array.Empty<Actor>());
        }
    }
    
    private void Update()
    {
        if (_aimOnUpdate)
        {
            AimAtClosest(Array.Empty<Actor>());
        }
    }
    
    public void AimAtClosest(ICollection<Actor> ignored)
    {
        if (_targetTag.TryFindClosest(transform.position, ignored, out Actor closest))
        {
            AimAtTarget(closest.transform);
        }
    }
    
    private void AimAtTarget(Transform target)
    {
        transform.right = target.position - transform.position;  // Rotates in 2D to face the target
        
        transform.right -= Vector3.back * transform.right.z;  // Zeroes out X and Y rotation/aligns on Z plane
    }
}