using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCameraMovement : MonoBehaviour
{
    public Camera cam;
    public float sensX = 5;
    public float sensY = 5;
    
    void Update()
    {
        cam.transform.position += new Vector3(
            Time.smoothDeltaTime * sensX * Input.GetAxis("Horizontal"),
            Time.smoothDeltaTime * sensY * Input.GetAxis("Vertical"));

    }
}
