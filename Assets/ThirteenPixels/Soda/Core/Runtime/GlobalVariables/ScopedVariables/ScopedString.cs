// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<string> instead.")]
#endif
    [System.Serializable]
    public class ScopedString : ScopedVariableBase<string, GlobalString>
    {
        public ScopedString(string value) : base(value)
        {
        }
    }
}
