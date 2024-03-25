using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StandaloneGizmoDrawer : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField]
    private bool _drawSelectedOnly = true;

    [SerializeField]
    private Color _color = Color.red;
    
    [SerializeField]
    private GizmoDrawType _drawType = GizmoDrawType.WireSphere;

    [Header("Draw Type Parameters")]
    [SerializeField, ShowIf(nameof(ShowCenter))]
    private Vector3 _center = Vector3.zero;
    private bool ShowCenter => _drawType is GizmoDrawType.WireSphere or GizmoDrawType.WireCube;
    
    [SerializeField, ShowIf(nameof(ShowRadius))]
    private float _radius = 1.0f;
    private bool ShowRadius => _drawType is GizmoDrawType.WireSphere;
    
    [SerializeField, ShowIf(nameof(ShowSize))]
    private Vector3 _size = Vector3.one;
    private bool ShowSize => _drawType is GizmoDrawType.WireCube;
    
    private void OnDrawGizmos()
    {
        if (!_drawSelectedOnly)
        {
            DrawGizmosInternal();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_drawSelectedOnly)
        {
            DrawGizmosInternal();
        }
    }

    private void DrawGizmosInternal()
    {
        Gizmos.color = _color;

        switch (_drawType)
        {
            case GizmoDrawType.WireSphere:
                Gizmos.DrawWireSphere(transform.TransformPoint(_center), _radius);
                break;
            case GizmoDrawType.WireCube:
                Gizmos.DrawWireCube(transform.TransformPoint(_center), transform.TransformVector(_size));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private enum GizmoDrawType
    {
        WireSphere,
        WireCube
    }
    #endif
}
