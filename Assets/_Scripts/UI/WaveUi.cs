using System;
using TMPro;
using UnityEngine;

public class WaveUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _timerText;
    
    private void Start()
    {
        GameManager.Instance.OnChangeState += OnChangeGameManagerState;
        OnChangeGameManagerState(GameManager.Instance.CurrentState);
    }
    
    private void OnChangeGameManagerState(StateMachine<GameManager>.State state)
    {
        if (state == GameManager.Instance.WaveCountdown ||
            state == GameManager.Instance.WaveCombat)
        {
            _timerText.gameObject.SetActive(true);
            
            _waveText.text = $"night {GameManager.Instance.CurWaveIndex + 1}";
        }
        else
        {
            _timerText.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (!_timerText.isActiveAndEnabled) return;
        
        TimeSpan timeRemaining = TimeSpan.FromSeconds(Mathf.Ceil(GameManager.Instance.WaveTimeRemaining));
        _timerText.text = timeRemaining.ToString("%m\\:ss");
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.OnChangeState -= OnChangeGameManagerState;
    }
}
