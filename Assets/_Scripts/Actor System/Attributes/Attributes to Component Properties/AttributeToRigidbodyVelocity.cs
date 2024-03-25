using UnityEngine;

public class AttributeToRigidbodyVelocity : AttributeToComponentProperty<Rigidbody2D>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _targetComponent.velocity = changeValueParams.NewValue * _targetComponent.transform.up;
    }
}
