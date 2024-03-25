using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MMF_Player))]
public class PlayEffectsOnFireWand : MonoBehaviour
{
    [SerializeField, Required] private MMF_Player _effects;
    
    private void OnEnable()
    {
        FireWandOnInput.OnWandFired += OnWandFired;
    }
    
    private void OnWandFired()
    {
        _effects.PlayFeedbacks();
    }
    
    private void OnDisable()
    {
        FireWandOnInput.OnWandFired -= OnWandFired;
    }
    
    private void Reset()
    {
        _effects = GetComponent<MMF_Player>();
    }
}