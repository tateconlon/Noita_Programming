using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class RunEndAnalytics : MonoBehaviour
{
    private bool _isApplicationQuitting = false;
    
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
    
    private void OnDestroy()
    {
        Dictionary<string, object> parameters = new();
        
        if (GameManager.Instance.Victory.IsActive)
        {
            parameters["runWon"] = true;
        }
        else if (GameManager.Instance.GameOver.IsActive)
        {
            parameters["runWon"] = false;
        }
        
        if (_isApplicationQuitting)
        {
            parameters["quitToDesktop"] = true;
        }
        else
        {
            // Otherwise, player is just quitting to menu without winning or losing the run
            parameters["quitToMenu"] = true;
        }
        
        AnalyticsService.Instance.CustomData("runEnd", parameters);
    }
}
