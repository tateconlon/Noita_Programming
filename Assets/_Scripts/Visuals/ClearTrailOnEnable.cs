using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class ClearTrailOnEnable : MonoBehaviour
{
    [SerializeField, Required] private TrailRenderer _trailRenderer;
    
    private void OnEnable()
    {
        _trailRenderer.Clear();
    }
    
    private void Reset()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }
}