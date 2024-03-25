using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RectangularSpawnPattern", menuName = "ScriptableObject/SpawnPatterns/RectangularSpawnPattern", order = 0)]
public class RectangularSpawnPattern : SpawnPattern
{
    [SerializeField] float tempOffset = 2.0f;  // TODO: calculate the proper offset somehow instead of hardcode?
    
    public override void GetSpawnPositions(EnemySpawner.SpawnRequest spawnRequest)
    {
        spawnRequest.SpawnPositions = new List<Vector2>(spawnRequest.Quantity);
        
        for (int i = 0; i < spawnRequest.Quantity; i++)
        {
            spawnRequest.SpawnPositions.Add(GetPointOnCameraBoundary(spawnRequest.SpawnVisibilityCam, tempOffset));
        }
    }
    
    static Vector2 GetPointOnCameraBoundary(Camera cam, float offset)
    {
        float screenHeightWorld = 2.0f * cam.orthographicSize;
        float screenWidthWorld = cam.aspect * screenHeightWorld;

        float offsetViewportHorizontal = offset / screenWidthWorld;
        float offsetViewportVertical = offset / screenHeightWorld;
        
        float viewportPointX = Random.Range(0f - offsetViewportHorizontal, 1f + offsetViewportHorizontal);
        float viewportPointY = Random.Range(0f - offsetViewportVertical, 1f + offsetViewportVertical);
        
        // Pick an axis to round down to min or up to max to be on the boundary's edge
        if (CodeHelpers.RandomBool())
        {
            if (viewportPointX < 0.5f)
            {
                viewportPointX = 0f - offsetViewportHorizontal;
            }
            else
            {
                viewportPointX = 1f + offsetViewportHorizontal;
            }
        }
        else
        {
            if (viewportPointY < 0.5f)
            {
                viewportPointY = 0f - offsetViewportVertical;
            }
            else
            {
                viewportPointY = 1f + offsetViewportVertical;
            }
        }
        
        return cam.ViewportToWorldPoint(new Vector2(viewportPointX, viewportPointY));
    }
}