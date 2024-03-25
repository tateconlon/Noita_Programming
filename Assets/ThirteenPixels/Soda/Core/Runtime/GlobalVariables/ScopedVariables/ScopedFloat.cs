// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<float> instead.")]
#endif
    [System.Serializable]
    public class ScopedFloat : ScopedVariableBase<float, GlobalFloat>
    {
        public ScopedFloat(float value) : base(value)
        {
        }
    }
}
