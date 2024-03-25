using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class StateMachine<TStateMachine> : MonoBehaviour where TStateMachine : StateMachine<TStateMachine>
{
    [ShowInInspector, ReadOnly]
    public State CurrentState { get; private set; }
    
    public event Action<State> OnChangeState; 
    
    [Button]
    protected void Transition(State nextState)
    {
        CurrentState?.Exit();
        
        CurrentState = nextState;
        
        // Always init each state before entering so it's guaranteed to have a non-null reference to its state machine
        CurrentState?.Init(this as TStateMachine);
        
        CurrentState?.Enter();
    }
    
    protected virtual void Update()
    {
        CurrentState?.Update();
    }
    
    protected virtual void OnDisable()
    {
        Transition(null);
    }
    
    public abstract class State
    {
        protected TStateMachine StateMachine { get; private set; }
        
        public bool IsActive { get; private set; } = false;
        public event Action<bool> OnSetIsActive; 
        
        public void Init(TStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        public virtual void Enter()
        {
            IsActive = true;
            OnSetIsActive?.Invoke(IsActive);
            StateMachine.OnChangeState?.Invoke(this);
        }
        
        public virtual void Update() { }
        
        public virtual void Exit()
        {
            IsActive = false;
            OnSetIsActive?.Invoke(IsActive);
        }
    }
}