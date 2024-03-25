using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    bool inCollider;

    public UnityEvent interactEvent;

    public UnityEvent inRangeEvent;
    public UnityEvent outOfRangeEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (inCollider)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactEvent.Invoke();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            inCollider = true;
            inRangeEvent?.Invoke();
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            inCollider = false;
            outOfRangeEvent?.Invoke();
        }
    }
}
