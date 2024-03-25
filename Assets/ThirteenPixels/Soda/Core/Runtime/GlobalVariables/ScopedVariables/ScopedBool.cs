// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<bool> instead.")]
#endif
    [System.Serializable]
    public class ScopedBool : ScopedVariableBase<bool, GlobalBool>
    {
        public ScopedBool(bool value) : base(value)
        {
        }
    }
}
