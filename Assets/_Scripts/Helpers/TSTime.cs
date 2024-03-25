using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
// gameplayTimeScale
//     used in normal play (e.g. slowing down at end of round),
// not just to debug
// gameplayDebugTimeScale
//     Slows down physics and gameplay at same rate to debug physics
//     Doesn't affect UI & framerate so UI and testing buttons are smooth
// applicationTimeScale - Best Debug Environment
//     ONLY used to debug by simulating lower frames
//     option to also set Application.targetFrameRate?

//TODO: This class is kind of jank, not really tested at all. The behaviour is odd.
//Trusting this class is ill-advised!
public class TSTime : MonoBehaviour
{
    [FormerlySerializedAs("gameplaySlider")] public TextMeshProUGUI gameplayText;
    [FormerlySerializedAs("debugSlider")] public TextMeshProUGUI debugText;
    [FormerlySerializedAs("framerateSlider")] public TextMeshProUGUI framerateText;

    public Slider gameplaySlider;
    public Slider debugSlider;
    public Slider framerateSlider;
    
    public bool syncPhysicsAndFrameRate;


    public float gameplayTimeScale = 1f;
    public float debugGameplayTimeScale = 1f;
    public float applicationTimeScale = 1f;

    float initFixedDeltaTime;
    void Awake()
    {
        initFixedDeltaTime = Time.fixedDeltaTime;
    }
    
    public void SetGameplayTimeScale(float gts)
    {
        gameplayTimeScale = gts;
        gameplayText.text = $"Gameplay: {gts}";
        gameplaySlider.value = gts;
        SetTimeScales();
    }

    public void SetDebugSlowFactor(float dsf)
    {
        debugGameplayTimeScale = dsf;
        debugText.text = $"Debug Physics: {dsf}";
        debugSlider.value = dsf;
        SetTimeScales();
    }

    public void SetApplicationTimeScale(float ats)
    {
        applicationTimeScale = ats;
        int frameRate = Mathf.FloorToInt(60f * applicationTimeScale);
        framerateText.text = $"Frame rate: {frameRate}";
        Application.targetFrameRate = frameRate;
        
        framerateSlider.value = ats;
    }

    public void MoveFramerateSlider(float val)
    {
        SetApplicationTimeScale(val);
        if (syncPhysicsAndFrameRate)
        {
            SetDebugSlowFactor(val);
        }
    }
    
    public void MoveDebugSlider(float val)
    {
        SetDebugSlowFactor(val);
        if (syncPhysicsAndFrameRate)
        {
            SetApplicationTimeScale(val);
        }
        else
        {
            SetGameplayTimeScale(val);
        }
    }
    

    void SetTimeScales()
    {
        Time.timeScale = gameplayTimeScale * debugGameplayTimeScale; 
        Time.fixedDeltaTime = initFixedDeltaTime * debugGameplayTimeScale;
    }
    
    public void ToggleSyncDebugAndFrameRate(bool isChecked)
    {
        syncPhysicsAndFrameRate = isChecked;
    }
}
