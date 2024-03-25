using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderUnityEvents : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] public UnityEvent<Collider, GameObject> TriggerEnterEvent;
    [SerializeField] public UnityEvent<Collider, GameObject> TriggerStayEvent;
    [SerializeField] public UnityEvent<Collider, GameObject> TriggerExitEvent;
    
    [Header("Collision")]
    [SerializeField] public UnityEvent<Collision, GameObject> CollisionEnterEvent;
    [SerializeField] public UnityEvent<Collision, GameObject> CollisionStayEvent;
    [SerializeField] public UnityEvent<Collision, GameObject> CollisionExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterEvent.Invoke(other, gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStayEvent.Invoke(other, gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExitEvent.Invoke(other, gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionEnterEvent.Invoke(collision, gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        CollisionStayEvent.Invoke(collision, gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        CollisionExitEvent.Invoke(collision, gameObject);
    }
}
