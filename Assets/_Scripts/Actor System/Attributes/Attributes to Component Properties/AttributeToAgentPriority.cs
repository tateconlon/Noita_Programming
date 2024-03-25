using UnityEngine;
using UnityEngine.AI;

public class AttributeToAgentPriority : AttributeToComponentProperty<NavMeshAgent>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        int avoidance = Mathf.RoundToInt(100.0f - changeValueParams.NewValue);
        
        _targetComponent.avoidancePriority = Mathf.Clamp(avoidance, 0, 99);
    }
}