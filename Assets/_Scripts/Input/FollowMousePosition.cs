using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMousePosition : MonoBehaviour
{
    [SerializeField, Required] private Transform _target;
    public Vector3 offset = Vector3.zero; 
    
    private void OnEnable()
    {
        Update();
    }
    
    private void Update()
    {
        if (Camera.main == null) return;
        
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        targetPos += offset;
        targetPos.z = _target.position.z;  // Don't modify Z position - will be set to Z pos of camera plane
        
        _target.position = targetPos;
    }
}