// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a boolean.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/Bool", order = 10)]
    public class GlobalBool : GlobalVariableBase<bool>
    {
        /// <summary>
        /// Changes the value to the opposite one.
        /// Intended for use via UnityEvent.
        /// </summary>
        public void Toggle()
        {
            value = !value;
        }
        
        public override void LoadValue(ISavestateReader reader, string key)
        {
            /// ExcludeFromDocs
            value = reader.GetBool(key);
        }

        public override void SaveValue(ISavestateWriter writer, string key)
        {
            /// ExcludeFromDocs
            writer.SetBool(key, value);
        }
    }
}
