using System.IO;
using UnityEngine;

[RequireComponent(typeof(BoundsWithHandle))]
public class GetPointInBounds : PositionProvider
{
    [SerializeField]
    private BoundsWithHandle _bounds;

    [SerializeField]
    private LayerMask _layerMask;
    
    [SerializeField]
    private int _maxAttempts = 16;
    
    protected override PositionRotation GetPositionRotationInternal()
    {
        for (int i = 0; i < _maxAttempts; i++)
        {
            Bounds bounds = _bounds.GetBounds();
            Vector3 startPoint = bounds.RandomPoint();
            startPoint.y = bounds.max.y;
            
            if (Physics.Raycast(startPoint, Vector3.down, out RaycastHit hitInfo, bounds.size.y, _layerMask))
            {
                return new PositionRotation(hitInfo.point, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            }
        }
        
        throw new InvalidDataException($"Could not raycast to valid point in {_maxAttempts} attempts");
    }
}
