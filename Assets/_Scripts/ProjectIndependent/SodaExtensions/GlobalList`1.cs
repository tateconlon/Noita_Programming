// Copyright © Sascha Graeff/13Pixels.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a List.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/List", order = 200)]
    public class GlobalList<T> : GlobalVariableBase<ObservableList<T>>
    {
        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            value = new ObservableList<T>();
        }
    }
}
