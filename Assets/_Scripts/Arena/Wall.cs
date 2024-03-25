using Sirenix.OdinInspector;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] int wallIndex = 0;

    // Hardcoded wall values from source code:
    //
    // gw = 480, gh = 270
    //
    // x1 = 48, y1 = 27
    // x2 = 432, y2 = 243
    //
    // Wall0 min/max = (-40, -40)/(48, 310)
    // Wall1 min/max = (432, -40)/(520, 310)
    // Wall2 min/max = (48, -40)/(432, 27)
    // Wall3 min/max = (48, 243)/(432, 310)
    
    [Button]
    void CalculateDimensions()
    {
        Bounds bounds = new();
        
        switch (wallIndex)
        {
            case 0:
                bounds.SetMinMax(new Vector2(-40, -40), new Vector2(48, 310));
                break;
            case 1:
                bounds.SetMinMax(new Vector2(432, -40), new Vector2(520, 310));
                break;
            case 2:
                bounds.SetMinMax(new Vector2(48, -40), new Vector2(432, 27));
                break;
            case 3:
                bounds.SetMinMax(new Vector2(48, 243), new Vector2(432, 310));
                break;
            default:
                Debug.LogError($"Unsupported {nameof(wallIndex)}: {wallIndex}");
                return;
        }
        
        transform.position = bounds.center;
        transform.localScale = bounds.size;
    }
}
