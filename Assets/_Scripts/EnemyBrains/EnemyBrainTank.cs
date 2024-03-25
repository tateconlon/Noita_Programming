using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainTank : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    [SerializeField, Required] private Hitbox _hitbox;
    
    private const float BaseHealth = 9.0f;
    private const float HpGainPerWave = 2f;
    private const float BaseSpeed = 0.75f;
    private const float BaseSize = 1.25f;
    private const float BaseKnockbackMult = 0.5f;
    
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
        
        gameObject.AddHTag(HTags.Enemy_Elite);
        gameObject.AddHTag(HTags.DespawnOffScreen);
    }
    
    private void OnDestroy()
    {
        _hitbox.OnHit -= HealthV2.BaseEnemyHitboxHit;
    }
}