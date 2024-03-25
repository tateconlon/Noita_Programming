using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Limitless Unity Development/SimpleBloom")]

public class SimpleBloom : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    [Space]
    public ColorParameter bloomColor = new ColorParameter(Color.white);
    public ClampedFloatParameter bloomAmount = new ClampedFloatParameter(1.5f, 0, 2);
    public ClampedFloatParameter bloomThreshold = new ClampedFloatParameter(0.1f, 0, 1);
    [Space]
    public ClampedIntParameter BlurPassAmount = new ClampedIntParameter(3,1,4);
    public ClampedFloatParameter blurAmount = new ClampedFloatParameter(.7f, 0f, 1);
    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;

}
