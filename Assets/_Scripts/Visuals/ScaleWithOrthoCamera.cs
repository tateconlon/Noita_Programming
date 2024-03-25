using Sirenix.OdinInspector;
using UnityEngine;

public class ScaleWithOrthoCamera : MonoBehaviour
{
    [SerializeField, Required] private Camera _camera;
    
    private void LateUpdate()
    {
        float camHeight = 2.0f * _camera.orthographicSize;
        float camWidth = camHeight * _camera.aspect;
        
        transform.localScale = new Vector3(camWidth, camHeight, 1.0f);
    }
}