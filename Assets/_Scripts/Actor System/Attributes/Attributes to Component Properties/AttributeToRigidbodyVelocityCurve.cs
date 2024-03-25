using Sirenix.OdinInspector;
using UnityEngine;

public class AttributeToRigidbodyVelocityCurve : AttributeToComponentProperty<Rigidbody2D>
{
    [SerializeField] private ParticleSystem.MinMaxCurve _velocityCurve = 
        new(1.0f, AnimationCurve.Constant(0f, 1f, 1f));
    
    [SerializeField, Required] private Timer _curveTimer;
    
    protected override bool autoFindTargetAttribute => true;
    
    private float _curveLerpFactor;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        _curveLerpFactor = Random.value;  // Pick a NEW random lerp factor EVERY time this enabled
    }

    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        FixedUpdate();
    }
    
    private void FixedUpdate()
    {
        float curValueCurved = _targetAttribute.curValue * _velocityCurve.Evaluate(_curveTimer.TimeElapsedNormalized, _curveLerpFactor);
        
        _targetComponent.velocity = curValueCurved * _targetComponent.transform.up;
    }
}