using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        
        // TODO: once we're spreading the demo publicly, add a user consent flow before calling this
        AnalyticsService.Instance.StartDataCollection();
    }
}
