using System;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControllerV2 : MonoBehaviour
{
    public static PlayerControllerV2 instance;

    [Header("Bindings")]
    [SerializeField, Required] public HealthV2 _health;
    [FormerlySerializedAs("stats")]
    public StatsModHolder statsMod;
    [SerializeField, Required] private GameObject _hurtSafetyCircle;
    
    private const float BaseHealth = 3.0f;
    
    public static event Action<PlayerControllerV2> OnChange;
    public static event Action<HealthV2.HpChangeParams> OnPlayerDied;

    public WandV2 wand;

    private void Awake()
    {
        instance = this;
        OnChange?.Invoke(this);
    }
    
    private void OnEnable()
    {
        _health.Init(BaseHealth);
        
        gameObject.AddHTag(HTags.Player);
        
        _health.OnHpChanged += OnHpChanged;
        _health.OnDeath += OnDeath;
    }
    
    private void OnHpChanged(HealthV2.HpChangeParams hpChangeParams)
    {
        if (!_health.IsDead && hpChangeParams.HpDeltaUnclamped < 0f)
        {
            LeanPool.Spawn(_hurtSafetyCircle, transform.position, Quaternion.identity);
            _health.IsInvincible.IncrementForDuration(1.0f, this);
        }
    }
    
    private void OnDeath(HealthV2.HpChangeParams hpChangeParams)
    {
        OnPlayerDied?.Invoke(hpChangeParams);
    }
    
    private void OnDisable()
    {
        _health.OnHpChanged -= OnHpChanged;
        _health.OnDeath -= OnDeath;
    }
}