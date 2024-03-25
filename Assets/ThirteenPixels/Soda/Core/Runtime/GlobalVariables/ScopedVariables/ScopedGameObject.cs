// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

#if UNITY_2020_1_OR_NEWER
    [System.Obsolete("Please use ScopedVariable<GameObject> instead.")]
#endif
    [System.Serializable]
    public class ScopedGameObject : ScopedVariableBase<GameObject, GlobalGameObject>
    {
        public ScopedGameObject(GameObject value) : base(value)
        {
        }
    }
}
