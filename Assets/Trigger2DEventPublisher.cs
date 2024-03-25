using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
// [RequireComponent(typeof(Rigidbody2D))]
public class Trigger2DEventPublisher : MonoBehaviour
{
    [Tooltip("LayerMask to filter which layers of other Colliders will trigger events")]
    [SerializeField] LayerMask layerMask = Physics2D.AllLayers;
    [SerializeField] UnityEvent<Collider2D> _OnTriggerEnter;
    [SerializeField] UnityEvent<Collider2D> _OnTriggerStay;
    [SerializeField] UnityEvent<Collider2D> _OnTriggerExit;

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"OnTriggerEnter: {this.gameObject}, other: {other.gameObject}" );
        
        if (layerMask.Contains(other.gameObject))
        {
            _OnTriggerEnter.Invoke(other);
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (layerMask.Contains(other.gameObject))
        {
            _OnTriggerStay.Invoke(other);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (layerMask.Contains(other.gameObject))
        {
            _OnTriggerExit.Invoke(other);
        }
    }
    
    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col)
        {
            col.isTrigger = true;
        }
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D)
        {
            rb2D.isKinematic = true;
        }
    }
}
