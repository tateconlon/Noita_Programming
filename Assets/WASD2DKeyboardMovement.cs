using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WASD2DKeyboardMovement : MonoBehaviour
{
    public Transform target;
    
    public float speed;
    public Key up = Key.W;
    public Key down = Key.S;
    public Key left = Key.A;
    public Key right = Key.D;

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVector = Vector3.zero;
        if (Keyboard.current[up].isPressed)
        {
            moveVector.y += 1;
        }
        
        if (Keyboard.current[down].isPressed)
        {
            moveVector.y -= 1;
        }
        if (Keyboard.current[left].isPressed)
        {
            moveVector.x -= 1;
        }
        if (Keyboard.current[right].isPressed)
        {
            moveVector.x += 1;
        }
        
        moveVector.Normalize();
        moveVector *= speed * Time.deltaTime;

        target.position += moveVector;
    }
}
