using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpPerLevelAnimationCurve", menuName = "ScriptableObject/ExpPerLevelProviders/ExpPerLevelAnimationCurve", order = 0)]
public class ExpPerLevelAnimationCurve : ExpPerLevelProvider
{
    [InfoBox("For levels below the first key time, the value of the first key will be returned. " +
             "For levels above the last key time, the value of the last key will be linearly extrapolated forward.")]
    [SerializeField] AnimationCurve animationCurve;
    
    public override float GetExpForLevel(int level)
    {
        if (level < animationCurve.StartTime())
        {
            return animationCurve[0].value;
        }

        if (level > animationCurve.EndTime())
        {
            float endTime = animationCurve.EndTime();
            float deltaPerLevel = animationCurve.Evaluate(endTime) - animationCurve.Evaluate(endTime - 1);

            float levelsToExtrapolate = level - endTime;

            return animationCurve.Evaluate(endTime) + (levelsToExtrapolate * deltaPerLevel);
        }
        
        return animationCurve.Evaluate(level);
    }
}
