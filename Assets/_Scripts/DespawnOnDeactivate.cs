using System;
using Lean.Pool;
using UnityEngine;

public class DespawnOnDeactivate : MonoBehaviour
{
    private void OnDisable()
    {
        if (this.IsSceneUnloading()) return;  // Ignore cases where this GameObject is being properly destroyed
        
        if (gameObject.activeInHierarchy)
        {
            enabled = true;
            Debug.LogException(new InvalidOperationException($"Should not disable this {nameof(DespawnOnDeactivate)} Component directly. Deactivate its GameObject instead"), this);
            return;
        }
        
        LeanPool.Despawn(gameObject);
    }
}
