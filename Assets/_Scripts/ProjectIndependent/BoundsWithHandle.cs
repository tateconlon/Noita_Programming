using UnityEngine;

//NOTE: DO NOT UPDATE THE BOUNDS USING THE HANDLE. IT IS BROKEN.
//ONLY USE THE INSPECTOR FIELDS AND USE HANDLE FOR VISUALIZATION
public class BoundsWithHandle : MonoBehaviour
{
    [SerializeField] private Bounds value;

    [Header("Parameters")]
    [SerializeField] public Color _handleColor = Color.red;
    [SerializeField] public Color _wireframeColor = Color.white;

    public Vector3 GetCenter()
    {
        return value.center + transform.position;
    }
    
    public Vector3 GetSize()
    {
        return new Vector3(value.size.x * transform.localScale.x, value.size.y * transform.localScale.y, value.size.z * transform.localScale.z);
    }

    public Bounds GetBounds()
    {
        Bounds retVal = new Bounds();
        
        //Make the BoundsWithHandle transform & translate relative to the transform
        //Note: I don't think you can rotate Bounds.
        retVal.center = GetCenter();
        retVal.size = GetSize();
        
        return retVal;
    }

    public void SetLocalCenter(Vector3 center)
    {
        value.center = center;
    }
    
    public void SetLocalSize(Vector3 size)
    {
        value.size = size;
    }
}
