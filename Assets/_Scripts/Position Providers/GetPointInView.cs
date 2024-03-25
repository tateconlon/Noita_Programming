using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

public class GetPointInView : PositionProvider
{
    [SerializeField, Tooltip("How much space a valid spawn position needs to be able to spawn the prefab")]
    private float _validSpawnFreeSpaceRadius = 1.0f;
    
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField, MinMaxSlider(0f, 100f)]
    private Vector2 _distanceRange = new(10f, 25f);
    
    [SerializeField, MinMaxSlider(0f, 1f)]
    private Vector2 _cameraRayRangeX = new(0.45f, 0.55f);
    
    [SerializeField, MinMaxSlider(0f, 1f)]
    private Vector2 _cameraRayRangeY = new(0.45f, 0.55f);
    
    [SerializeField]
    private int _maxAttempts = 16;
    
    protected override PositionRotation GetPositionRotationInternal()
    {
        for (int i = 0; i < _maxAttempts; i++)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(Random.Range(_cameraRayRangeX.x, _cameraRayRangeX.y), 
                Random.Range(_cameraRayRangeY.x, _cameraRayRangeY.y)));
        
            if (Physics.SphereCast(ray, _validSpawnFreeSpaceRadius, _distanceRange.x)) continue;  // Too close

            Vector3 spawnPoint;
        
            if (Physics.SphereCast(ray.GetPoint(_distanceRange.x), _validSpawnFreeSpaceRadius,
                    ray.direction, out RaycastHit hitInfo, _distanceRange.y, _layerMask))
            {
                spawnPoint = hitInfo.point;

                spawnPoint += hitInfo.normal.normalized * _validSpawnFreeSpaceRadius;  // Add offset to give enough space
            }
            else
            {
                spawnPoint = ray.GetPoint(Random.Range(_distanceRange.x, _distanceRange.y));
                
                // Move the spawnPoint down to be grounded so it's reachable
                if (Physics.SphereCast(spawnPoint, _validSpawnFreeSpaceRadius,
                        Vector3.down, out RaycastHit groundingHitInfo, _distanceRange.y, _layerMask))
                {
                    spawnPoint = groundingHitInfo.point;
                }
                else
                {
                    continue;  // Couldn't find ground underneath, so this point is probably out over open air
                }
            }

            Vector3 directionToCamera = Camera.main.transform.position - spawnPoint;
            directionToCamera.y = 0f;  // Zero out Y coordinate so prefab will only rotate around the Y axis
            Quaternion rotation = Quaternion.LookRotation(directionToCamera);

            return new PositionRotation(spawnPoint, rotation);
        }
        
        throw new InvalidDataException($"Could not cast to valid point in {_maxAttempts} attempts");
    }
}
