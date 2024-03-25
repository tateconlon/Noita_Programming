using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainParentRat : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    [SerializeField, Required] private Hitbox _hitbox;
    [SerializeField, Required] private GameObject _childRatPrefab;
    
    private const float BaseHealth = 6.0f;
    private const float HpGainPerWave = 2.0f;
    private const float BaseSpeed = 0.875f;
    private const float BaseSize = 1.0f;
    private const float BaseKnockbackMult = 0.75f;
    
    private void Awake()
    {
        _hitbox.OnHit += HealthV2.BaseEnemyHitboxHit;
        _health.OnDeath += OnDeath;
    }
    
    private void OnEnable()
    {
        _health.Init(BaseHealth , HpGainPerWave);
        _agent.speed = BaseSpeed;
        transform.localScale = BaseSize * Vector3.one;
        _knockbackReceiver.KnockbackMultiplier = BaseKnockbackMult;
        
        gameObject.AddHTag(HTags.Enemy);
        gameObject.AddHTag(HTags.DespawnOffScreen);
    }
    
    private void OnDeath(HealthV2.HpChangeParams hpChangeParams)
    {
        for (int i = 0; i < 3; i++)
        {
            LeanPool.Spawn(_childRatPrefab, transform.position, Quaternion.identity);
            
            // TODO: give child rats temporary immunity to whatever just killed the parent
        }
    }
    
    private void OnDestroy()
    {
        _hitbox.OnHit -= HealthV2.BaseEnemyHitboxHit;
        _health.OnDeath -= OnDeath;
    }
}