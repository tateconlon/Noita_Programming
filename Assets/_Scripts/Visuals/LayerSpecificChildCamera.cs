using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LayerSpecificChildCamera : MonoBehaviour
{
    [SerializeField, Required] private Camera _camera;
    [SerializeField, Required] private Camera _parentCamera;
    
    private int _cullingMask;
    private RenderTexture _targetTexture;
    
    private void Awake()
    {
        _cullingMask = _camera.cullingMask;
        _targetTexture = _camera.targetTexture;
    }
    
    private void LateUpdate()
    {
        _camera.CopyFrom(_parentCamera);
        
        _camera.cullingMask = _cullingMask;
        _camera.targetTexture = _targetTexture;
        
        _camera.depth = _parentCamera.depth - 1.0f;  // So this camera's output will be usable by parent
    }
    
    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }
}