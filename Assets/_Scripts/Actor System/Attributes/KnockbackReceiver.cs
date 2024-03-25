using Sirenix.OdinInspector;
using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField, Required] private Rigidbody2D _rb2d;
    [SerializeField] public float KnockbackMultiplier = 1.0f;
    
    public void ApplyKnockback(Vector2 knockbackDirection, float magnitude)
    {
        //Changing knockback direction to always be away from player because enemies rocketing into feels unfair in the chaos
        knockbackDirection = _rb2d.transform.position - PlayerControllerV2.instance.transform.position;
        _rb2d.AddForce(KnockbackMultiplier * magnitude * 0.5f * knockbackDirection.normalized, ForceMode2D.Impulse);
    }
}
