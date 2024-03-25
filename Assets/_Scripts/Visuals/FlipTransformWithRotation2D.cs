using UnityEngine;

/// <example>
/// Flips a gun to be oriented right-side-up no matter what its rotation is
/// </example>
public class FlipTransformWithRotation2D : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 localScale = transform.localScale;
        
        float yScaleMult = transform.right.x > 0f ? 1.0f : -1.0f;
        float yScale = yScaleMult * Mathf.Abs(localScale.y);
        
        transform.localScale = new Vector3(localScale.x, yScale, localScale.z);
    }
}