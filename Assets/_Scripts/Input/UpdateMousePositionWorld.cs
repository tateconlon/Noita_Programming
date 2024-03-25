using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpdateMousePositionWorld : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GlobalVector2 mousePositionWorld;
    
    void Update()
    {
        mousePositionWorld.value = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
