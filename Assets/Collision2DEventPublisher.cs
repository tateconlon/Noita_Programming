using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collision2DEventPublisher : MonoBehaviour
{
    [SerializeField] UnityEvent<Collision2D> _OnCollisionEnter;
    [SerializeField] UnityEvent<Collision2D> _OnCollisionStay;
    [SerializeField] UnityEvent<Collision2D> _OnCollisionExit;

    void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log($"OnTriggerEnter: {this.gameObject}, other: {other.gameObject}" );
        _OnCollisionEnter.Invoke(other);
    }
    
    void OnCollisionStay2D(Collision2D other)
    {
        _OnCollisionStay.Invoke(other);
    }
    
    void OnCollisionExit2D(Collision2D other)
    {
        _OnCollisionExit.Invoke(other);
    }
    
    void Reset()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col)
        {
            col.isTrigger = false;
        }
    }
}
