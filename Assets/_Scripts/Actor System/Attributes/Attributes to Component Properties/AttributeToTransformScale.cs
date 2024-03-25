using UnityEngine;

public class AttributeToTransformScale : AttributeToComponentProperty<Transform>
{
    protected override bool autoFindTargetAttribute => false;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _targetComponent.localScale = changeValueParams.NewValue * Vector3.one;
    }
    
    // For some reason localScale doesn't initialize correctly the first time an Actor is spawned.
    // Probably something to do with Transforms or how the pooling works, but this fix works for now.
    private void Start()
    {
        OnChangeCurValue(new AttributeComponent.ChangeValueParams(_targetAttribute));
    }
}