using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Inspired by <a href="https://docs.unity3d.com/ScriptReference/CharacterController.html">Unity's CharacterController</a>
/// </summary>
public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Required] private Rigidbody2D _rb2d;
    
    /// <summary>
    /// The sum of all requested motion for this FixedUpdate
    /// </summary>
    [NonSerialized, ShowInInspector, ReadOnly]
    private Vector2 _motionSum;
    
    private void OnEnable()
    {
        _motionSum = Vector2.zero;
    }
    
    public void Move(Vector2 motion)
    {
        _motionSum += motion;
    }
    
    private void FixedUpdate()
    {
        _rb2d.MovePosition(_rb2d.position + _motionSum);
        
        _motionSum = Vector2.zero;
    }
    
    private void Reset()
    {
        _rb2d = GetComponentInParent<Rigidbody2D>();
    }
}
