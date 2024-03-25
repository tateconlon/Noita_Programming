using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

public class LookAtTarget2D : MonoBehaviour
{
    [Required]
    [SerializeField] private ScopedVariable<Vector2> _target;
    
    [SerializeField] private float _turnSpeed = 10.0f;
    [Range(0f, 360f)]
    [SerializeField] private float _zRotationOffsetAngle = 0f;
    [SerializeField] private bool _useUnscaledTime = false;
    
    private void LateUpdate()
    {
        Vector3 targetDirection = (_target.value - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(targetDirection.y, targetDirection.x) + _zRotationOffsetAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        
        float rotationThisFrame = _turnSpeed * (_useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationThisFrame);
    }
}
