using System;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] AudioVariables audioVariables;
    [SerializeField] BuildVariables buildVariables;
    [SerializeField] DebugVariables debugVariables;
    [SerializeField] GameplayVariables gameplayVariables;
    [SerializeField] VisualVariables visualVariables;
    
    // One-off helper objects for allocation-free methods
    [NonSerialized] public MaterialPropertyBlock PropertyBlock;
    [NonSerialized] public RaycastHit2D[] SingleRaycastHit2D = new RaycastHit2D[1];
    
    [NonSerialized] public readonly UnityEvent TimerUpdater = new();

    void Awake()
    {
        if (!M.dm)
        {
            M.dm = this;
            DontDestroyOnLoad(gameObject);  // Optionally use this to persist across scene changes
        }
        else
        {
            // Debug.LogError($"There is already an instance of {GetType().Name}");
            gameObject.SetActive(false);  // ReSharper disable once Unity.InefficientPropertyAccess
            Destroy(gameObject);
            return;
        }

        D.au = audioVariables;
        D.bv = buildVariables;
        D.gv = gameplayVariables;
        D.vv = visualVariables;
        D.dv = debugVariables;
        
        PropertyBlock = new MaterialPropertyBlock();
        
        InitializeScriptableObjects();
    }

    // Do any work needed to initialize data in ScriptableObjects here since their Awake callbacks are weird
    void InitializeScriptableObjects()
    {
        
    }

    void Update()
    {
        TimerUpdater.Invoke();
    }
}