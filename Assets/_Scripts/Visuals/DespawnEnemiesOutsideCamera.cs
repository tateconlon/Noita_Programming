using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DespawnEnemiesOutsideCamera : MonoBehaviour
{
    [SerializeField, Required] private Camera _camera;
    [SerializeField, Required] private BoxCollider2D _boxCollider;
    
    public static event Action<GameObject, Camera> OnWillDespawnEnemy; 
    
    private void Update()
    {
        float camHeight = 2.0f * _camera.orthographicSize;
        float camWidth = camHeight * _camera.aspect;
        
        if (!Mathf.Approximately(_boxCollider.size.y, camHeight) || 
            !Mathf.Approximately(_boxCollider.size.x, camWidth))
        {
            _boxCollider.size = new Vector2(camWidth, camHeight);
        }
    }
    
    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (!otherCollider.TryGetComponent(out Hurtbox hurtbox)) return;
        
        GameObject other = hurtbox.Owner;
        
        if (other.TryGetComponent(out HealthV2 healthV2) && !healthV2.IsDead && 
            other.HasHTag(HTags.Enemy) && other.HasHTag(HTags.DespawnOffScreen))
        {
            OnWillDespawnEnemy?.Invoke(other, _camera);
            other.SetActive(false);
        }
    }
}