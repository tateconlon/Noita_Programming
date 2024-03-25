using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class WaveCountdownUi : MonoBehaviour
{
    [SerializeField, Required] private MMF_Player _countdownFx;
    
    private void Start()
    {
        GameManager.Instance.WaveCountdown.OnSetIsActive += OnActivateWaveCountdown;
        OnActivateWaveCountdown(GameManager.Instance.WaveCountdown.IsActive);
    }
    
    private void OnActivateWaveCountdown(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
            
            _countdownFx.PlayFeedbacks();
            
            _countdownFx.RefreshCache();  // Need to do this in builds to calculate MMF_Player.TotalDuration
            GameManager.Instance.WaveCountdown.IsPlayingCountdownEffects.IncrementForDuration(_countdownFx.TotalDuration, this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.WaveCountdown.OnSetIsActive -= OnActivateWaveCountdown;
    }
}
