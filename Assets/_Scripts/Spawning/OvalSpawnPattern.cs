using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(fileName = "OvalSpawnPattern", menuName = "ScriptableObject/SpawnPatterns/OvalSpawnPattern", order = 0)]
public class OvalSpawnPattern : SpawnPattern
{
    [SerializeField] public ScopedVariable<Vector2> ovalCenterPos;
    
    [SerializeField] float width = 16.0f;
    [SerializeField] float height = 9.0f;
    
    public override void GetSpawnPositions(EnemySpawner.SpawnRequest spawnRequest)
    {
        spawnRequest.SpawnPositions = new List<Vector2>(spawnRequest.Quantity);
        
        float angleIncrement = 360f / spawnRequest.Quantity;
        float angleOffset = Random.Range(0f, 360f);  // Add an offset to rotate all enemies randomly around Z axis
        
        Vector2 centerPos = spawnRequest.CenterPos.HasValue ? spawnRequest.CenterPos.Value : ovalCenterPos;
        
        for (int i = 0; i < spawnRequest.Quantity; i++)
        {
            float angle = (i * angleIncrement) + angleOffset;
            
            spawnRequest.SpawnPositions.Add(GetPositionFromAngle(angle) + centerPos);
        }
    }
    
    // Source: https://answers.unity.com/questions/759542/get-coordinate-with-angle-and-distance.html
    private Vector2 GetPositionFromAngle(float angle)
    {
        float posX = width / 2.0f * Mathf.Cos(angle * Mathf.Deg2Rad);
        float posY = height / 2.0f * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector2(posX, posY);
    }
}
