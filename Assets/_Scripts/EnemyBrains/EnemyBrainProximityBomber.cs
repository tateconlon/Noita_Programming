using System;
using System.Collections;
using Lean.Pool;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainProximityBomber : StateMachine<EnemyBrainProximityBomber>
{
    [Header("Bindings")]
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private NavMeshAgent _agent;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    [SerializeField, Required] private Hitbox _hitbox;
    [SerializeField, Required] private MMF_Player _telegraphDetonateFx;
    [SerializeField, Required] private GameObject _bombPrefab;
    
    [Header("States")]
    [SerializeField] private PursueState _pursue;
    [SerializeField] private TelegraphDetonateState _telegraph;
    [SerializeField] private DetonateState _detonate;
    
    private const float BaseHealth = 6.0f;
    private const float HpGainPerWave = 2.0f;
    private const float BaseSpeed = 0.75f;
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
        
        Transition(_pursue);
    }
    
    private void OnDeath(HealthV2.HpChangeParams hpChangeParams)
    {
        if (CurrentState != _detonate)
        {
            Transition(_detonate);
        }
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
    public class PursueState : State
    {
        private const float BaseDetonateRange = 2.5f;
        
        public override void Enter()
        {
            StateMachine._agent.enabled = true;
            
            base.Enter();
        }
        
        public override void Update()
        {
            if (HTagExtensions.IsTaggedObjectNearby(HTags.Player, StateMachine.transform.position, BaseDetonateRange))
            {
                StateMachine.Transition(StateMachine._telegraph);
            }
        }
    }
    
    [Serializable]
    public class TelegraphDetonateState : State
    {
        private Coroutine _telegraphCR = null;
        
        public override void Enter()
        {
            base.Enter();
            
            _telegraphCR = StateMachine.StartCoroutine(TelegraphCR());
        }
        
        private IEnumerator TelegraphCR()
        {
            yield return StateMachine._telegraphDetonateFx.PlayFeedbacksCoroutine(StateMachine.transform.position);
            
            StateMachine.Transition(StateMachine._detonate);
        }
        
        public override void Exit()
        {
            StateMachine.StopCoroutine(_telegraphCR);
            StateMachine._telegraphDetonateFx.StopFeedbacks();
            
            base.Exit();
        }
    }
    
    [Serializable]
    public class DetonateState : State
    {
        public override void Enter()
        {
            base.Enter();
            
            StateMachine._agent.enabled = false;
            LeanPool.Spawn(StateMachine._bombPrefab, StateMachine.transform.position, Quaternion.identity);
            StateMachine._health.ChangeHp(-StateMachine._health.Hp, StateMachine.gameObject);
        }
    }
}