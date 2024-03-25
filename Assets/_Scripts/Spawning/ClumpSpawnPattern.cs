using UnityEngine;

[CreateAssetMenu(fileName = "GridSpawnPattern", menuName = "ScriptableObject/SpawnPatterns/GridSpawnPattern", order = 0)]
public class ClumpSpawnPattern : SpawnPattern
{
    public SpawnPattern centerPosSpawnPattern;
    
    public override void GetSpawnPositions(EnemySpawner.SpawnRequest spawnRequest)
    {
        InitCenterPos(spawnRequest);
        
        int i = 1;
        int layer = 1;
        
        float enemyRadius = 0.5f;

        while (i < spawnRequest.Quantity)
        {
            float layerRadius = layer * 2.0f * enemyRadius;
            float layerCircumference = 2.0f * Mathf.PI * layerRadius;
            
            float enemiesInLayer = layerCircumference / (2.0f * enemyRadius);
            float angleIncrement = 360f / enemiesInLayer;
            
            int enemiesToPlaceInLayer = Mathf.FloorToInt(enemiesInLayer);
            
            while (enemiesToPlaceInLayer > 0 && i < spawnRequest.Quantity)
            {
                float angle = i * angleIncrement;
                Vector2 offsetPos = GetPositionFromAngle(angle, layerRadius);
                
                spawnRequest.SpawnPositions.Add(offsetPos + spawnRequest.SpawnPositions[0]);
                
                enemiesToPlaceInLayer -= 1;
                i += 1;
            }
            
            layer += 1;
        }
    }
    
    private void InitCenterPos(EnemySpawner.SpawnRequest spawnRequest)
    {
        if (spawnRequest.CenterPos.HasValue)
        {
            spawnRequest.SpawnPositions.Add(spawnRequest.CenterPos.Value);
        }
        else
        {
            // Kinda hacky way to fill the first spawn position (the center of the clump) using another spawn pattern
            int quantity = spawnRequest.Quantity;
            spawnRequest.Quantity = 1;
            centerPosSpawnPattern.GetSpawnPositions(spawnRequest);
            spawnRequest.Quantity = quantity;
        }
    }
    
    Vector2 GetPositionFromAngle(float angle, float radius)
    {
        float posX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float posY = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector2(posX, posY);
    }
}
