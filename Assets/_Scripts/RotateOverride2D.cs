using UnityEngine;

public class RotateOverride2D : MonoBehaviour
{
    [SerializeField] bool rotateUp;
    public void LateUpdate()
    {
        this.transform.right = this.transform.right - Vector3.forward * this.transform.right.z; //Align to camera plane by making sure the right vector is aligned with the x-y plane by zero-ing out the z component)
        if (rotateUp)
        {
            this.transform.rotation = Quaternion.identity;
        }
    }
}