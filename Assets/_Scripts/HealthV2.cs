using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class HealthV2 : MonoBehaviour
{
    [SerializeField] private float _maxHp = 1.0f;
    [SerializeField] private float _hp = 1.0f;
    
    public float MaxHp => _maxHp;
    public float Hp => _hp;
    public bool IsDead => _hp <= 0f;
    [ShowInInspector]
    public readonly CounterBool IsInvincible = new(false);
    
    public event Action<HpChangeParams> OnHpChanged;
    public static event Action<HpChangeParams> OnAnyHpChanged;
    
    public event Action<HpChangeParams> OnDeath;
    public static event Action<HpChangeParams> OnAnyDeath;
    
    private void OnEnable()
    {
        _hp = _maxHp;
    }
    
    public void Init(float hp)
    {
        _maxHp = hp;
        _hp = hp;
    }
    
    public void Init(float baseHp, float hpGainPerWave)
    {
        //time 2 for balance
        float curHp = baseHp + Mathf.Floor(hpGainPerWave * 2 * GameManager.Instance.CurWaveIndex);
        
        Init(curHp);
    }
    
    public void ChangeHp(float hpDelta, GameObject attacker)
    {
        if (IsDead) return;
        if (IsInvincible.Value) return;
        
        float dmgMult = 0f;
        if (gameObject.HasHTag(HTags.Enemy_Elite) && RelicsManager.instance.relics.Any(x => x is EliteDamageUp))
        {
            EliteDamageUp relic = RelicsManager.instance.relics.Find(x => x is EliteDamageUp) as EliteDamageUp;
            dmgMult += relic.multAdd;
        }
        
        if (gameObject.HasHTag(HTags.Enemy) && RelicsManager.instance.relics.Any(x => x is EnemyDmgUp))
        {
            EnemyDmgUp relic = RelicsManager.instance.relics.Find(x => x is EnemyDmgUp) as EnemyDmgUp;
            dmgMult += relic.multAdd;
        }
        
        hpDelta *= 1 + dmgMult;

        float prevHp = _hp;
        _hp = Mathf.Clamp(_hp + hpDelta, 0f, _maxHp);
        
        HpChangeParams hpChangeParams = new(this, attacker, prevHp, hpDelta);
        
        OnHpChanged?.Invoke(hpChangeParams);
        OnAnyHpChanged?.Invoke(hpChangeParams);
        
        if (IsDead)
        {
            OnDeath?.Invoke(hpChangeParams);
            OnAnyDeath?.Invoke(hpChangeParams);
        }
    }
    
    private void OnDisable()
    {
        IsInvincible.Clear();
    }
    
    public readonly struct HpChangeParams
    {
        public readonly HealthV2 Victim;
        public readonly GameObject Attacker;
        public readonly float PrevHp;
        public readonly float HpDeltaUnclamped;
        
        public HpChangeParams(HealthV2 victim, GameObject attacker, float prevHp, float hpDeltaUnclamped)
        {
            Victim = victim;
            Attacker = attacker;
            PrevHp = prevHp;
            HpDeltaUnclamped = hpDeltaUnclamped;
        }
    }
    
    // Replace with custom hitbox response behavior in enemy brains as needed
    public static void BaseEnemyHitboxHit(Hitbox hitbox, HashSet<Hurtbox> hurtboxes)
    {
        foreach (Hurtbox hurtbox in hurtboxes)
        {
            if (hurtbox.Owner.HasHTag(HTags.Player) && hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
            {
                healthV2.ChangeHp(-0.5f, hitbox.Owner);
            }
        }
    }
}