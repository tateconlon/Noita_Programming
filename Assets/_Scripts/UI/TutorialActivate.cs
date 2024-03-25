using System;
using UnityEngine;

public class TutorialActivate : MonoBehaviour
{
    [SerializeField] private int waveIndexToActivate;
    [SerializeField] private bool onWaveInit;
    void Start()
    {
        if (onWaveInit)
        {
            GameManager.Instance.WaveCountdown.OnSetIsActive += OnActivateTutorial;
        }
        else
        {
            GameManager.Instance.Shop.OnSetIsActive += OnActivateTutorial;
        }
        //Only turn on the gameobject so we can register the events
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (onWaveInit)
        {
            GameManager.Instance.WaveCountdown.IsPlayingCountdownEffects.Decrement();
        }
    }

    private void OnDestroy()
    {
        if (onWaveInit)
        {
            GameManager.Instance.WaveCountdown.OnSetIsActive -= OnActivateTutorial;
        }
        else
        {
            GameManager.Instance.Shop.OnSetIsActive -= OnActivateTutorial;
        }
    }

    private void OnActivateTutorial(bool isActive)
    {
        if (isActive)
        {
            if (GameManager.Instance.CurWaveIndex == waveIndexToActivate)
            {
                gameObject.SetActive(true);
                if (onWaveInit)
                {
                    GameManager.Instance.WaveCountdown.IsPlayingCountdownEffects.Increment();
                }
            }
            
        }
    }
}