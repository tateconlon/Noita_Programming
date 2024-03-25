// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<Color> instead.")]
#endif
    [System.Serializable]
    public class ScopedColor : ScopedVariableBase<Color, GlobalColor>
    {
        public ScopedColor(Color value) : base(value)
        {
        }
    }
}
