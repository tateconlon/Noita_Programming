using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // These AudioSources are all '2D' and do not care about listener/player positions
    [SerializeField] AudioSource globalSfxAudioSource;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource ambianceAudioSource;

    [Header("Play OneShot At Point")] 
    [SerializeField] LeanGameObjectPool oneShotAtPointPool;

    void Awake()
    {
        if (!M.am)
        {
            M.am = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);  // Optionally use this to persist across scene changes
        }
        else
        {
            // Debug.LogError($"There is already an instance of {GetType().Name}");
            gameObject.SetActive(false);  // ReSharper disable once Unity.InefficientPropertyAccess
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySfx(SfxType sfxType)
    {
        if (D.au.sfxData.ContainsKey(sfxType))
        {
            globalSfxAudioSource.PlayOneShot(D.au.sfxData[sfxType]);
        }
        else
        {
            Debug.LogWarning("no audio for sfx : " + sfxType);
        }
     
    }
    
    public void PlayGlobalSfx(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        globalSfxAudioSource.volume = volume;
        globalSfxAudioSource.pitch = pitch;
        globalSfxAudioSource.PlayOneShot(clip);
    }

    public void PlayMusic(MusicType musicType)
    {
        if (!D.au.musicData.ContainsKey(musicType))
        {
            StopMusic();
            return;
        }
        
        musicAudioSource.PlaySingleTrack(D.au.musicData[musicType]);
    }
    
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }
    
    public void FadeMusic(float duration)
    {
        musicAudioSource.DOFade(0, duration);
    }
    
    public void PlayAmbiance(AmbianceType ambianceType)
    {
        if (!D.au.ambianceData.ContainsKey(ambianceType))
        {
            StopAmbiance();
            return;
        }
        
        ambianceAudioSource.PlaySingleTrack(D.au.ambianceData[ambianceType]);
    }
    
    public void StopAmbiance()
    {
        ambianceAudioSource.Stop();
    }

    public void PlaySfxAtPoint(SfxType sfxType, Vector3 position, Transform parent = null, bool checkDistance = true)
    {
        if (!D.au.sfxData.ContainsKey(sfxType)) return;

        bool shouldPlaySfx = true;

        // TODO: reimplement main camera access later if needed
        // if (checkDistance)
        // {
        //     float distToCamera = Vector2.Distance(position, M.cam.transform.position);
        //     float audioRange = D.au.audioRangeValues[D.au.sfxData[sfxType].playerAudioRange];
        //     shouldPlaySfx = distToCamera < 2.0f * audioRange;
        // }
        
        if (shouldPlaySfx)
        {
            OneShotAtPoint oneShotAtPoint = oneShotAtPointPool.Spawn(position, Quaternion.identity, parent).GetComponent<OneShotAtPoint>();
            AudioClip playedAudioClip = oneShotAtPoint.Activate(D.au.sfxData[sfxType], oneShotAtPointPool);
            oneShotAtPointPool.Despawn(oneShotAtPoint.gameObject, playedAudioClip.length);
        }
    }
}
