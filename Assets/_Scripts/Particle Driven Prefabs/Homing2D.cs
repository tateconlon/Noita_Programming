using UnityEngine;

public class Homing2D : MonoBehaviour
{
    [SerializeField] public Transform homingTarget;
    [SerializeField] public float speed = 1.0f;
    [SerializeField] public float angularSpeed = 1.0f;
    
    private void Update()
    {
        UpdateRotation();
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        
        float rotAngle = GetZangleFromTwoPosition(transform, homingTarget);
            
        float toAngle = Mathf.MoveTowardsAngle(eulerAngles.z, rotAngle, Time.deltaTime * angularSpeed);
            
        transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, toAngle);
    }

    private void UpdatePosition()
    {
        transform.position += Time.deltaTime * speed * transform.up;
    }
    
    private static float GetZangleFromTwoPosition(Transform fromTrans, Transform toTrans)
    {
        if (fromTrans == null || toTrans == null)
        {
            return 0f;
        }
        float xDistance = toTrans.position.x - fromTrans.position.x;
        float yDistance = toTrans.position.y - fromTrans.position.y;
        float angle = (Mathf.Atan2(yDistance, xDistance) * Mathf.Rad2Deg) - 90f;
        angle = GetNormalizedAngle(angle);

        return angle;
    }
    
    private static float GetNormalizedAngle(float angle)
    {
        while (angle < 0f)
        {
            angle += 360f;
        }
        while (360f < angle)
        {
            angle -= 360f;
        }
        return angle;
    }
}
