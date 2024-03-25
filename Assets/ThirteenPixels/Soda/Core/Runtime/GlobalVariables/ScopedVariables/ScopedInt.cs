// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<int> instead.")]
#endif
    [System.Serializable]
    public class ScopedInt : ScopedVariableBase<int, GlobalInt>
    {
        public ScopedInt(int value) : base(value)
        {
        }
    }
}
