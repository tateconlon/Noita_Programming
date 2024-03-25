using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TriggerEventPublisher : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> _OnTriggerEnter;
    [SerializeField] UnityEvent<Collider> _OnTriggerStay;
    [SerializeField] UnityEvent<Collider> _OnTriggerExit;

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"OnTriggerEnter: {this.gameObject}, other: {other.gameObject}" );
        _OnTriggerEnter.Invoke(other);
    }
    
    void OnTriggerStay(Collider other)
    {
        _OnTriggerStay.Invoke(other);
    }
    
    void OnTriggerExit(Collider other)
    {
        _OnTriggerExit.Invoke(other);
    }
    
    void Reset()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (col)
        {
            col.isTrigger = true;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
        }
    }
}
