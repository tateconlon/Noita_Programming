using Lean.Pool;
using UnityEngine;

public class OneShotAtPoint : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    LeanGameObjectPool _parentPool;
    
    public AudioClip Activate(OneShotSfx oneShotSfx, LeanGameObjectPool parentPool)
    {
        _parentPool = parentPool;
        
        if (!D.au.audioRangeValues.ContainsKey(oneShotSfx.playerAudioRange))
        {
            Debug.LogWarning("Range not set for PlayerAudioRangeType." + oneShotSfx.playerAudioRange);
            return null;
        }

        audioSource.maxDistance = D.au.audioRangeValues[oneShotSfx.playerAudioRange];
        
        return audioSource.PlayOneShot(oneShotSfx);
    }

    void OnDestroy()
    {
        if (_parentPool != null && _parentPool.DeactivatedChild != transform.parent)
        {
            _parentPool.Detach(gameObject);  // Detach so pool doesn't keep track of destroyed objects
        }
    }
}