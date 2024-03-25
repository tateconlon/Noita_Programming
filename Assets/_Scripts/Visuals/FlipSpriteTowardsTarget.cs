using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlipSpriteTowardsTarget : MonoBehaviour
{
    [Required]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Required]
    [SerializeField] private ScopedVariable<Vector2> _target;
    
    [InfoBox("This script assumes that Sprites are facing to the right by default, use this flag otherwise.")]
    [SerializeField] private bool _spriteFacesLeftByDefault = false;
    
    private void LateUpdate()
    {
        bool flipX = _target.value.x < transform.position.x;
        
        if (_spriteFacesLeftByDefault)
        {
            flipX = !flipX;
        }
        
        _spriteRenderer.flipX = flipX;
    }
    
    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
