using System.Collections.Generic;
using UnityEngine;

public class AttachedAudio : MonoBehaviour
{
    //[InfoBox("These AudioSources are '3D' and modify sound based on listener/player positions", EInfoBoxType.Warning)]
    [SerializeField] AudioSource audioSourceOne;
    [SerializeField] AudioSource audioSourceTwo;
    [SerializeField] AudioSource audioSourceThree;

    readonly Dictionary<AudioSource, PersistentAudioType> _onGoingAudio = new();

    void Awake()
    {
        _onGoingAudio.Add(audioSourceOne, PersistentAudioType.None);
        _onGoingAudio.Add(audioSourceTwo, PersistentAudioType.None);
        _onGoingAudio.Add(audioSourceThree, PersistentAudioType.None);
    }


    public void PlaySfx(SfxType sfxType)
    {
        if (!D.au.sfxData.ContainsKey(sfxType)) return;
        M.am.PlaySfxAtPoint(sfxType, transform.position, transform);
    }

    public void PlayPersistentAudio(PersistentAudioType audioType)
    {
        if (!D.au.persistentAudioData.ContainsKey(audioType))
        {
            Debug.LogWarning("SingleTrackAudio not set for PersistentAudioType." + audioType);
            return;
        }
        
        SingleTrackAudio audioTrack = D.au.persistentAudioData[audioType];
        AudioSource audioToUse = GetOngoingAudioSource(audioType);
        if (audioToUse)//it was paused, so just play
        {
            if (!audioToUse.isPlaying)
            {
                audioToUse.Play();
            }
        }
        else
        {
            audioToUse = GetNextAudioSource();
            audioToUse.PlaySingleTrack(audioTrack);
            _onGoingAudio[audioToUse] = audioType;
        }
    }
    
    public void PausePersistentAudio(PersistentAudioType audioType)
    {
        if (!D.au.persistentAudioData.ContainsKey(audioType)) return;
        
        AudioSource audioToUse = GetOngoingAudioSource(audioType);
        if (audioToUse)
        {
            audioToUse.Pause();//audio is still on going so we know to play it instead of overwriting it
        }
    }
    
    public void StopPersistentAudio(PersistentAudioType audioType)
    {
        if (!D.au.persistentAudioData.ContainsKey(audioType)) return;
        
        AudioSource audioToUse = GetOngoingAudioSource(audioType);
        if (audioToUse)
        {
            audioToUse.Stop();
            _onGoingAudio[audioToUse] = PersistentAudioType.None;
        }
    }

    AudioSource GetNextAudioSource()
    {
        if (_onGoingAudio[audioSourceOne] == PersistentAudioType.None)
        {
            return audioSourceOne;
        }
        else if (_onGoingAudio[audioSourceTwo] == PersistentAudioType.None)
        {
            return audioSourceTwo;
        }
        else if (_onGoingAudio[audioSourceThree] == PersistentAudioType.None)
        {
            return audioSourceThree;
        }
        else
        {
            Debug.LogWarning("Ran out of audio sources");
            return audioSourceOne;
        }
    }

    AudioSource GetOngoingAudioSource(PersistentAudioType audioType)
    {
        foreach (KeyValuePair<AudioSource, PersistentAudioType> pair in _onGoingAudio)
        {
            if (pair.Value == audioType)
            {
                return pair.Key;
            }
        }

        return null;
    }
}