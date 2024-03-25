using UnityEngine;

public static class AnimationCurveExtensions
{
    public static float StartTime(this AnimationCurve animationCurve)
    {
        return animationCurve.keys[0].time;
    }
    
    public static float EndTime(this AnimationCurve animationCurve)
    {
        return animationCurve.keys[animationCurve.length - 1].time;
    }

    public static float Duration(this AnimationCurve animationCurve)
    {
        return animationCurve.EndTime() - animationCurve.StartTime();
    }
}
