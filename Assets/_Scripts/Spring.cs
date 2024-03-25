using Sirenix.OdinInspector;
using UnityEngine;

// Inspired by https://github.com/a327ex/blog/issues/60
public abstract class Spring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] bool useUnscaledTime = false;
    [SerializeField] float initialValue = 1;
    [SerializeField] float stiffness = 100;
    [SerializeField] float damping = 10;
    [SerializeField] private float maxChange = 0.3f;
    
    [ShowInInspector, ReadOnly]
    public float Value
    {
        get => _value;
        private set
        {
            float prev = Value;
            _value = value;
            OnValueChanged(prev, Value);
        }
    }
    
    private float _value;

    [Header("Debug")]
    [SerializeField] float debugPullForce = 1.0f;
    bool ShouldDisableDebugButtons => !Application.isPlaying;
    
    float _velocity;
    
    const float DisableWithinMargin = 0.00001f;

    protected virtual void Awake()
    {
        Clear();
    }

    private void Update()
    {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.smoothDeltaTime;
        deltaTime = Mathf.Min(deltaTime, 0.01666667f);  // Clamp so spring won't have huge changes when FPS is low
        
        float acceleration = -stiffness * (Value - initialValue) - damping * _velocity;
        _velocity += acceleration * deltaTime;
        Value += _velocity * deltaTime;
        
        if (Mathf.Abs(Value - initialValue) < DisableWithinMargin)
        {
            Clear();  // Stop updating if spring has mostly stopped moving
        }
    }
    
    protected abstract void OnValueChanged(float prev, float cur);

    public void Pull(float force)
    {
        float newValue = Value + force;
        Value = Mathf.Clamp(newValue, initialValue - maxChange, initialValue + maxChange);
        // Value += force;
        // Value = Mathf.Clamp(Value, initialValue - maxChange, initialValue + maxChange);

        enabled = true;
    }

    public void Pull(float force, float newStiffness, float newDamping)
    {
        stiffness = newStiffness;
        damping = newDamping;
        
        Pull(force);
    }

    public void Clear()
    {
        Value = initialValue;
        _velocity = 0;

        enabled = false;
    }

    [Button, DisableIf(nameof(ShouldDisableDebugButtons))]
    void DebugPull()
    {
        Pull(debugPullForce);
    }
}
