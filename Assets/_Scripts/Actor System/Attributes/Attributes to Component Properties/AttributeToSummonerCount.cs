using UnityEngine;

public class AttributeToSummonerCount : AttributeToComponentProperty<SpellLauncher>
{
    protected override bool autoFindTargetAttribute => true;
    
    protected override void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _targetComponent.count = Mathf.RoundToInt(changeValueParams.NewValue);
    }
}
