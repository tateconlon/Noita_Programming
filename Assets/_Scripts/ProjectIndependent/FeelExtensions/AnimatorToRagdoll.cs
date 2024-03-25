using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorToRagdoll : MMRagdoller
{
    [Tooltip("Useful if the same colliders used for ragdolling are also used for collision detection, e.g. shooting")]
    [SerializeField] private bool _alwaysEnableCollisions = true;
    
    [Header("Changing Layers")]
    [SerializeField] private bool _useRagdollLayer = false;
    
    [Layer, ShowIf(nameof(_useRagdollLayer))]
    [SerializeField] private int _ragdollLayer;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool> _onSetIsKinematic;

    private readonly Dictionary<Rigidbody, int> _kinematicRigidbodyLayers = new();

    protected override void Initialization()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.TryGetComponent(out MMRagdollerIgnore _)) continue;
            
            _kinematicRigidbodyLayers[rb] = rb.gameObject.layer;  // Need to build dictionary before calling base init
        }
        
        base.Initialization();
    }

    protected override void SetIsKinematic(bool isKinematic)
    {
        base.SetIsKinematic(isKinematic);
        
        foreach ((Rigidbody rb, int kinematicLayer) in _kinematicRigidbodyLayers)
        {
            if (rb.transform == transform) continue;  // Including this ignore line because the base class does it
            
            if (_alwaysEnableCollisions)
            {
                rb.detectCollisions = true;
            }

            if (_useRagdollLayer)
            {
                rb.gameObject.layer = isKinematic ? kinematicLayer : _ragdollLayer;
            }
        }
        
        _onSetIsKinematic.Invoke(isKinematic);
    }
}
