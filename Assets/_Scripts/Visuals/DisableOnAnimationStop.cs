using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class DisableOnAnimationStop : MonoBehaviour
{
    [SerializeField, Required] private Animation _animation;
    
    private void LateUpdate()
    {
        if (!_animation.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
    
    private void Reset()
    {
        _animation = GetComponent<Animation>();
    }
}