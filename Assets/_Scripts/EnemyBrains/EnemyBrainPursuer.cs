using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainPursuer : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    [SerializeField, Required] private Hitbox _hitbox;
    
    private const float BaseHealth = 9.0f;
    private const float HpGainPerWave = 2.0f;
    private const float BaseSpeed = 0.75f;
    private const float BaseSize = 1.0f;
    private const float BaseKnockbackMult = 1.5f;
    
    private Coroutine _increaseSpeedCR;
    
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
        
        _increaseSpeedCR = StartCoroutine(IncreaseSpeedCR());
    }
    
    private IEnumerator IncreaseSpeedCR()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1.0f);
            
            _agent.speed += 0.1f;
        }
    }
    
    private void OnDisable()
    {
        StopCoroutine(_increaseSpeedCR);
    }
    
    private void OnDestroy()
    {
        _hitbox.OnHit -= HealthV2.BaseEnemyHitboxHit;
    }
}