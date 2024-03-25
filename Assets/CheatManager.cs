using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField] private GlobalFloat waveTimeRemaining;
    [SerializeField] private GlobalBool _isInvincible;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (GameManager.Instance.CurrentState is GameManager.WaveCombatState state)
            {
                state.SkipStage();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _isInvincible.value = !_isInvincible.value;
        }
    }
}
