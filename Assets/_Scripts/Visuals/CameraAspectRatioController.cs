using Sirenix.OdinInspector;
using UnityEngine;

// From: https://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html

[RequireComponent(typeof(Camera))]
public class CameraAspectRatioController : MonoBehaviour
{
    [SerializeField, Required] private Camera _camera;
    [SerializeField] private Vector2 _aspectRatio = new(16.0f, 9.0f);
    
    private float _cachedScaleHeight = float.NaN;
    
    private void LateUpdate()
    {
        // determine the game window's current aspect ratio
        float windowAspect = Screen.width / (float)Screen.height;
        
        // current viewport height should be scaled by this amount
        float scaleHeight = windowAspect / (_aspectRatio.x / _aspectRatio.y);
        
        if (Mathf.Approximately(scaleHeight, _cachedScaleHeight)) return;
        
        _cachedScaleHeight = scaleHeight;
        
        // if scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {  
            Rect rect = _camera.rect;
            
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            
            _camera.rect = rect;
        }
        else // add pillarbox
        {
            float scaleWidth = 1.0f / scaleHeight;
            
            Rect rect = _camera.rect;
            
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            
            _camera.rect = rect;
        }
    }
    
    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }
}