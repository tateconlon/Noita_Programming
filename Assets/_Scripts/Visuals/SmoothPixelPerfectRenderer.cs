using UnityEngine;

// Extrapolation approach from: https://gamedev.stackexchange.com/a/18801
// Example implementation for Godot here: https://www.reddit.com/r/godot/comments/cvn6qn/comment/ey6hfe2

/// <summary>
/// When using the Pixel Perfect Camera, this smooths out the "staircase" jitter effect caused by rounding to the
/// nearest pixel. Place on a GameObject containing a Renderer component (most likely a SpriteRenderer) that is the
/// child of a GameObject that is moved through player input, AI, physics, etc. 
/// </summary>
[RequireComponent(typeof(Renderer))]
public class SmoothPixelPerfectRenderer : MonoBehaviour
{
    private const float PixelsPerUnit = 16.0f;
    
    private Vector2 _prevTargetPos = Vector2.zero;
    
    private void OnEnable()
    {
        _prevTargetPos = transform.parent.position;
    }
    
    private void LateUpdate()
    {
        Vector3 targetPos = transform.parent.position;
        Vector2 velocity = (Vector2)targetPos - _prevTargetPos;  // Or use velocity from AI agent, Rigidbody, etc.
        _prevTargetPos = targetPos;
        
        float x;
        float y;
        
        if (velocity == Vector2.zero)
        {
            x = RoundToPixel(targetPos.x);
            y = RoundToPixel(targetPos.y);
        }
        else if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            x = RoundToPixel(targetPos.x);
            y = RoundToPixel(targetPos.y + (x - targetPos.x) * velocity.y / velocity.x);
        }
        else
        {
            y = RoundToPixel(targetPos.y);
            x = RoundToPixel(targetPos.x + (y - targetPos.y) * velocity.x / velocity.y);
        }
        
        transform.position = new Vector3(x, y, targetPos.z);
    }
    
    private float RoundToPixel(float inCoord)
    {
        return Mathf.Round(PixelsPerUnit * inCoord) / PixelsPerUnit;
    }
}