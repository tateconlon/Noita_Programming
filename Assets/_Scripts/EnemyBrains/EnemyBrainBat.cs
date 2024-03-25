using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainBat : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    [SerializeField, Required] private Hitbox _hitbox;
    
    private const float BaseHealth = 1.0f;
    private const float HpGainPerWave = 1.0f;
    private const float BaseSpeed = 1.125f;
    private const float BaseSize = 0.5f;
    private const float BaseKnockbackMult = 2.0f;
    
    private void Awake()
    {
        _hitbox.OnHit += HealthV2.BaseEnemyHitboxHit;
    }
    
    private void OnEnable()
    {
        _health.Init(BaseHealth , HpGainPerWave);
        _agent.speed = BaseSpeed;
        transform.localScale = BaseSize * Vector3.one;
        _knockbackReceiver.KnockbackMultiplier = BaseKnockbackMult;
        
        gameObject.AddHTag(HTags.Enemy);
    }
    
    private void OnDestroy()
    {
        _hitbox.OnHit -= HealthV2.BaseEnemyHitboxHit;
    }
}