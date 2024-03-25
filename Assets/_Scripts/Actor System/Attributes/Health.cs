using System;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

[RequireComponent(typeof(AttributeComponent))]
public class Health : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    [SerializeField, Required] private AttributeComponent _curHealth;
    [SerializeField, Required] private AttributeComponent _maxHealth;
    
    [Header("Tags")]
    [SerializeField] private TagHashSet _removeTagsOnDeath;

    [Header("XP on Death (should be moved out)")]
    [SerializeField] private ScopedVariable<float> _xp;
    [SerializeField] private float _xpOnDeath;
    
    [ShowInInspector]
    public event Action<HealthChangedParams> OnHealthChanged;
    public static event Action<HealthChangedParams> OnAnyHealthChanged;
    
    [ShowInInspector]
    public event Action<HealthChangedParams> OnDeath;
    public static event Action<HealthChangedParams> OnAnyDeath;
    
    private void OnEnable()
    {
        _curHealth.OnChangeCurValue += OnChangeHealth;
    }
    
    private void OnChangeHealth(AttributeComponent.ChangeValueParams changeValueParams)
    {
        HealthChangedParams healthChangedParams = new(_actor, changeValueParams.Effect.Owner,
            changeValueParams.PrevValue, changeValueParams.NewValue, changeValueParams.DeltaUnclamped,
            _maxHealth.curValue);
        
        OnHealthChanged?.Invoke(healthChangedParams);
        OnAnyHealthChanged?.Invoke(healthChangedParams);
        
        // Check prevHealth too so we don't trigger OnDeath more than once
        if (healthChangedParams.PrevHealth > 0f && healthChangedParams.CurHealth <= 0f)
        {
            OnDeath?.Invoke(healthChangedParams);
            OnAnyDeath?.Invoke(healthChangedParams);
            
            foreach (Tag tagToRemove in _removeTagsOnDeath)
            {
               // _actor.Tags.Remove(tagToRemove);
            }
            
            // Should really be moved elsewhere
            if (_xpOnDeath > 0 && _xp != null)
            {
                _xp.value += _xpOnDeath;
            }
            
            // NOTE: doesn't disable GameObject or anything
        }
    }
    
    private void OnDisable()
    {
        _curHealth.OnChangeCurValue -= OnChangeHealth;
    }

    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
        _curHealth = GetComponent<AttributeComponent>();
    }
    
    public readonly struct HealthChangedParams
    {
        public readonly Actor Victim;
        public readonly Actor Attacker;
        public readonly float PrevHealth;
        public readonly float CurHealth;
        public readonly float HealthDeltaUnclamped;
        public readonly float MaxHealth;
        
        public HealthChangedParams(Actor victim, Actor attacker, float prevHealth, float curHealth, float healthDeltaUnclamped, float maxHealth)
        {
            Victim = victim;
            Attacker = attacker;
            PrevHealth = prevHealth;
            CurHealth = curHealth;
            HealthDeltaUnclamped = healthDeltaUnclamped;
            MaxHealth = maxHealth;
        }
    }
}