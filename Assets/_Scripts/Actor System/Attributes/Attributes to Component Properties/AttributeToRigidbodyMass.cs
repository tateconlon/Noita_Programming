using UnityEngine;

public class AttributeToRigidbodyMass : AttributeToComponentProperty<Rigidbody2D>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _targetComponent.mass = changeValueParams.NewValue;
    }
}