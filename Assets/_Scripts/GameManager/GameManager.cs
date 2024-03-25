using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : StateMachine<GameManager>
{
    public static GameManager Instance;
    
    [Header("Bindings")]
    [SerializeField, Required] private Stage _stage;
    
    [Header("States")]
    [SerializeField] public InitNextWaveState InitNextWave;
    [SerializeField] public WaveCountdownState WaveCountdown;
    [SerializeField] public WaveCombatState WaveCombat;
    [SerializeField] public GameOverState GameOver;
    [SerializeField] public WaveEndState WaveEnd;
    [SerializeField] public ShopState Shop;
    [SerializeField] public TestingState Testing;
    [SerializeField] public VictoryState Victory;
    
    public Stage CurStage => _stage;
    public int CurWaveIndex { get; private set; } = 0;
    public Wave CurWave { get; private set; }
    public float WaveTimeRemaining { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        CurWaveIndex = -1;
        
        Transition(InitNextWave);
    }
    
    private void OnEnable()
    {
        PlayerControllerV2.OnPlayerDied += OnPlayerDied;
    }
    
    private void OnPlayerDied(HealthV2.HpChangeParams hpChangeParams)
    {
        if (CurrentState.GetType() != typeof(VictoryState))
        {
            Transition(GameOver);
        }
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        PlayerControllerV2.OnPlayerDied -= OnPlayerDied;
    }
    
    [Serializable]
    public class InitNextWaveState : State
    {
        public override void Enter()
        {
            StateMachine.CurWaveIndex++;
            StateMachine.CurWave = StateMachine.CurStage.GetWave(StateMachine.CurWaveIndex);
            StateMachine.WaveTimeRemaining = StateMachine.CurWave.duration;
            
            base.Enter();
            
            StateMachine.Transition(StateMachine.WaveCountdown);
        }
    }
    
    [Serializable]
    public class WaveCountdownState : State
    {
        public readonly CounterBool IsPlayingCountdownEffects = new(false);
        
        public override void Enter()
        {
            IsPlayingCountdownEffects.Clear();
            
            base.Enter();
        }
        
        public override void Update()
        {
            if (!IsPlayingCountdownEffects.Value)
            {
                StateMachine.Transition(StateMachine.WaveCombat);
            }
        }
    }
    
    [Serializable]
    public class WaveCombatState : State
    {
        //Variables are inited in InitWaveState
        public void SkipStage()
        {
            StateMachine.Transition(StateMachine.WaveEnd);
        }

        public override void Update()
        {
            StateMachine.WaveTimeRemaining -= Time.deltaTime;
            
            if (StateMachine.WaveTimeRemaining <= 0f)
            {
                StateMachine.Transition(StateMachine.WaveEnd);
            }
        }

        public void IncreaseWaveTime(float time)
        {
            StateMachine.WaveTimeRemaining += time;
        }
    }
    
    [Serializable]
    public class GameOverState : State
    {
        
    }
    
    [Serializable]
    public class WaveEndState : State
    {
        // TODO: play some kind of cinematic when the wave is over
        
        public override void Enter()
        {
            foreach (GameObject enemy in new List<GameObject>(HTags.Enemy.GameObjects))
            {
                enemy.SetActive(false);
            }
            
            foreach (GameObject pickup in new List<GameObject>(HTags.Pickup.GameObjects))
            {
                pickup.SetActive(false);
            }
            
            base.Enter();
            
            if (StateMachine.CurWaveIndex + 1 < StateMachine.CurStage.NumWaves)
            {
                StateMachine.Transition(StateMachine.Shop);
            }
            else
            {
                StateMachine.Transition(StateMachine.Victory);
            }
        }
    }
    
    [Serializable]
    public class ShopState : State
    {
        public bool hasChosen = false;
        public override void Enter()
        {
            ShopExitButton.ExitShopButtonClick += OnExitShotButtonClicked;
            hasChosen = false;
            
            base.Enter();
        }
        
        private void OnExitShotButtonClicked()
        {
            StateMachine.Transition(StateMachine.Testing);
        }
        
        public override void Exit()
        {
            ShopExitButton.ExitShopButtonClick -= OnExitShotButtonClicked;
            
            base.Exit();
        }
    }
    
    [Serializable]
    public class TestingState : State
    {
        public override void Enter()
        {
            RoomTeleporter.TeleportEvent += OnTeleport;
            
            base.Enter();  // TODO Finnbarr: teleporters and dummies should activate in response to entering this state
        }
        
        private void OnTeleport()
        {
            StateMachine.Transition(StateMachine.InitNextWave);
        }
        
        public override void Exit()
        {
            RoomTeleporter.TeleportEvent -= OnTeleport;
            
            base.Exit();
        }
    }
    
    [Serializable]
    public class VictoryState : State
    {
        
    }
}
