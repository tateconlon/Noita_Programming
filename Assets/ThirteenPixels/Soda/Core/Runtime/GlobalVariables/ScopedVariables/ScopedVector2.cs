// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<Vector2> instead.")]
#endif
    [System.Serializable]
    public class ScopedVector2 : ScopedVariableBase<Vector2, GlobalVector2>
    {
        public ScopedVector2(Vector2 value) : base(value)
        {
        }
    }
}
