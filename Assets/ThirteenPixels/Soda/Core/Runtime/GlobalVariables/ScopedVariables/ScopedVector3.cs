// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<Vector3> instead.")]
#endif
    [System.Serializable]
    public class ScopedVector3 : ScopedVariableBase<Vector3, GlobalVector3>
    {
        public ScopedVector3(Vector3 value) : base(value)
        {
        }
    }
}
