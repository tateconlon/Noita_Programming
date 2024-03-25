public class AttributeToTimerDuration : AttributeToComponentProperty<Timer>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _targetComponent.Duration = changeValueParams.NewValue;
    }
}