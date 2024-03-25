using Sirenix.OdinInspector;
using UnityEngine;

public class OrbitAroundTransform : MonoBehaviour
{
    [SerializeField, Required]
    private Transform _pivotTransform;
    public float angle = 0;
    public float distance = 1;
    public float period = 1;
    public bool clockwise = true;
    
    private void OnEnable()
    {
        float range = Mathf.Deg2Rad * 30;
        float offset = Mathf.Deg2Rad * 90;  // So it starts at 12 o'clock
        angle = Random.Range(offset - range, offset + range);
        
        //HACK: fixing weird orbiting issue where the orbit pivot local position when pooling
        _pivotTransform.position = this.gameObject.transform.position;
    }
    
    private void Update()
    {
        float changeInAngle = Time.deltaTime * 2 * Mathf.PI / period;
        angle += clockwise ? -changeInAngle : changeInAngle;
        
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
        
        // // Kinda gross, but sometimes prefabs need to have their pivots on them, so we detach them here
        // if (pivotObject.transform.parent == this.transform)
        // {
        //     pivotObject.transform.SetParent(null);
        // }
        
        // Cache pivot's World position in case the pivot is parented to the transform
        Vector3 pivotLocalPosition = _pivotTransform.position;
        
        this.transform.position = _pivotTransform.position + dir * distance;
        
        //In case the pivot is parented to the transform
        _pivotTransform.position = pivotLocalPosition;
    }
}
