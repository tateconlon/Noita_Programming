using UnityEngine.AI;

public class AttributeToAgentSpeed : AttributeToComponentProperty<NavMeshAgent>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _targetComponent.speed = changeValueParams.NewValue;
    }
}