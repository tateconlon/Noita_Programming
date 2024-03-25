using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AudioVariables", menuName = "ScriptableObject/AudioVariables", order = 0)]
public class AudioVariables : ScriptableObject
{
    [SerializeField] public AudioMixer audioMixer;
    
    public float musicBaseVolume = 0.5f;
    public float sfxBaseVolume = 1;

    [SerializeField] public AudioClip nullAudioClip;
    
    // [InfoBox("Used to randomize OneShotSFX volume, where X [0-1] is is chosen uniformly at random. " +
    //          "The value of Y [0-1] lerps between the min and max values of the OneShotSFX's Volume Distribution.")]
    // [CurveRange(0, 0, 1, 1)]
    public AnimationCurve sfxVolumeDistributionCurve;

    [Serializable] public class AudioRangeDistanceDict : UnitySerializedDictionary<PlayerAudioRangeType, float> { }
    [FormerlySerializedAs("OneShotAtPointRanges")] public AudioRangeDistanceDict audioRangeValues = new AudioRangeDistanceDict();
    
    [Serializable] public class SfxDataDict : UnitySerializedDictionary<SfxType, OneShotSfx> { }
    public SfxDataDict sfxData = new SfxDataDict();
    
    [Serializable] public class PersistentAudioDataDict : UnitySerializedDictionary<PersistentAudioType, SingleTrackAudio> { }
    public PersistentAudioDataDict persistentAudioData = new PersistentAudioDataDict();

    [Serializable] public class MusicDataDict : UnitySerializedDictionary<MusicType, SingleTrackAudio> { }
    public MusicDataDict musicData = new MusicDataDict();
    
    [Serializable] public class AmbianceDataDict : UnitySerializedDictionary<AmbianceType, SingleTrackAudio> { }
    public AmbianceDataDict ambianceData = new AmbianceDataDict();

    void OnValidate()
    {
        CheckDictionariesMissingTypes();  // Pretty brute-force to do it here, could use a button instead
    }
    
    void CheckDictionariesMissingTypes()
    {
        foreach (SfxType sfxType in Enum.GetValues(typeof(SfxType)))
        {
            if (!sfxData.ContainsKey(sfxType) && sfxType != SfxType.None)
            {
                Debug.LogWarning("SFXTypeData missing SFXType." + sfxType);
            }
        }
        
        foreach (PersistentAudioType persistentAudioType in Enum.GetValues(typeof(PersistentAudioType)))
        {
            if (!persistentAudioData.ContainsKey(persistentAudioType) && persistentAudioType != PersistentAudioType.None)
            {
                Debug.LogWarning("SFXTypeData missing SFXType." + persistentAudioType);
            }
        }
        
        foreach (MusicType musicType in Enum.GetValues(typeof(MusicType)))
        {
            if (!musicData.ContainsKey(musicType) && musicType != MusicType.None)
            {
                Debug.LogWarning("MusicTypeData missing MusicType." + musicType);
            }
        }
        
        foreach (AmbianceType ambianceType in Enum.GetValues(typeof(AmbianceType)))
        {
            if (!ambianceData.ContainsKey(ambianceType) && ambianceType != AmbianceType.None)
            {
                Debug.LogWarning("AmbianceTypeData missing AmbianceType." + ambianceType);
            }
        }
    }
}

[Serializable]
public class OneShotSfx  // Should always be played using AudioSource.PlayOneShot()
{
    [Tooltip("This range setting is only used when playing through a player's AudioSources")]
    public PlayerAudioRangeType playerAudioRange = PlayerAudioRangeType.Medium;
    
    [Tooltip("Note that the final volume value used will be clamped between 0 and 1")]
    /*[MinMaxSlider(0f, 2f)]*/ public Vector2 volumeDistribution = Vector2.one;
    
    public AudioClip[] audioClips;
}

[Serializable]
public class SingleTrackAudio  // Should always be played using AudioSource.Play()
{
    public PlayerAudioRangeType maxRange = PlayerAudioRangeType.Medium;
    
    [Range(0f, 1f)] public float trackVolume = 1f;
    
    public AudioClip audioTrack;
}

public enum SfxType
{
    None,
}

public enum PersistentAudioType
{
    None,
}

public enum MusicType
{
    None,
}

public enum AmbianceType
{
    None,
}

public enum PlayerAudioRangeType
{
    None, Short, Medium, Long, Max,
}