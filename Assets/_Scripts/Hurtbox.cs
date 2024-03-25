using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Represents a single Hurtbox on an entity, which could optionally have custom data, e.g. a "head" hurtbox could
/// multiply all damage received by 2 and make any hits on it crit.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField, Required] public GameObject Owner;
    [SerializeField, Required] public Collider2D Collider;
    
    private void Reset()
    {
        Collider = GetComponent<Collider2D>();
    }
}
