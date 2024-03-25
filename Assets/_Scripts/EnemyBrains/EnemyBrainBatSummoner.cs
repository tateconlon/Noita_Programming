using System;
using System.Collections;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainBatSummoner : StateMachine<EnemyBrainBatSummoner>
{
    [Header("Bindings")]
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    [SerializeField, Required] private Hitbox _hitbox;
    [SerializeField, Required] private GameObject _batPrefab;
    
    [Header("States")]
    [SerializeField] private ApproachState _approach;
    [SerializeField] private AimState _aim;
    [SerializeField] private ShootState _shoot;
    
    private const float BaseHealth = 8.0f;
    private const float HpGainPerWave = 2.5f;
    private const float BaseSpeed = 0.5f;
    private const float BaseSize = 1.0f;
    private const float BaseKnockbackMult = 1.0f;
    
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
        
        _health.OnDeath += OnDeath;
        
        Transition(_approach);
    }
    
    private void OnDeath(HealthV2.HpChangeParams hpChangeParams)
    {
        Transition(null);
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        _health.OnDeath -= OnDeath;
    }
    
    private void OnDestroy()
    {
        _hitbox.OnHit -= HealthV2.BaseEnemyHitboxHit;
    }
    
    [Serializable]
    public class ApproachState : State
    {
        private const float BaseShootRange = 6.0f;
        
        public override void Enter()
        {
            StateMachine._agent.enabled = true;
            
            base.Enter();
        }
        
        public override void Update()
        {
            if (HTagExtensions.IsTaggedObjectNearby(HTags.Player, StateMachine.transform.position, BaseShootRange))
            {
                StateMachine.Transition(StateMachine._aim);
            }
        }
    }
    
    [Serializable]
    public class AimState : State
    {
        private Coroutine _aimCr = null;
        
        public override void Enter()
        {
            StateMachine._agent.enabled = false;
            
            _aimCr = StateMachine.StartCoroutine(AimCR());
            
            base.Enter();
        }
        
        private IEnumerator AimCR()
        {
            yield return new WaitForSeconds(2.0f);
            
            StateMachine.Transition(StateMachine._shoot);
        }
        
        public override void Exit()
        {
            StateMachine.StopCoroutine(_aimCr);
            
            base.Exit();
        }
    }
    
    [Serializable]
    public class ShootState : State
    {
        public override void Enter()
        {
            base.Enter();
            
            LeanPool.Spawn(StateMachine._batPrefab, StateMachine.transform.position, Quaternion.identity);
            StateMachine.Transition(StateMachine._approach);
        }
    }
}