using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProduceTurningInput : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] GlobalBool useMouseInput;
    [SerializeField] GlobalVector2 mousePositionWorld;
    [SerializeField] GlobalVector2 playerPosition;
    [SerializeField] GlobalVector2 playerDirection;
    Vector2 _moveInput;
    
    [Header("Outputs")]
    [SerializeField] GlobalFloat turningInput;
    
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        _moveInput = callbackContext.ReadValue<Vector2>();
    }

    void Update()
    {
        if (useMouseInput.value)
        {
            // Thanks Kalle: https://stackoverflow.com/a/65795828
            Vector3 snakeToMouse = mousePositionWorld.value - playerPosition.value;
            Vector3 cross = Vector3.Cross(snakeToMouse, playerDirection.value);
            
            turningInput.value = cross.z;
        }
        else
        {
            turningInput.value = _moveInput.x;
        }
    }
}
