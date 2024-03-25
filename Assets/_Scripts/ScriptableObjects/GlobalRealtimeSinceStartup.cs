using ThirteenPixels.Soda;
using UnityEngine;

/// <summary>
/// Used to access Time.realtimeSinceStartup as a GlobalFloat
/// </summary>
[CreateAssetMenu(menuName = "Soda/GlobalVariable/Realtime Since Startup", order = 300)]
public class GlobalRealtimeSinceStartup : GlobalFloat
{
    public override float value
    {
        get => base.value + Time.realtimeSinceStartup;
        set => base.value = value;
    }
}
