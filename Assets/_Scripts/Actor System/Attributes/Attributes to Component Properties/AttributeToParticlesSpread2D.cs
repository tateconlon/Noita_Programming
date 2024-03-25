using UnityEngine;

public class AttributeToParticlesSpread2D : AttributeToComponentProperty<ParticleSystem>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        ParticleSystem.ShapeModule shapeModule = _targetComponent.shape;
        
        shapeModule.arc = changeValueParams.NewValue;
        shapeModule.rotation = new Vector3(0f, 0f, -0.5f * changeValueParams.NewValue);
    }
}