// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// This attribute is used to display a replacement property during editor play mode.
    /// This is used to prevent editing of serialized values in ScriptableObjects in play mode, which would be persistent.
    /// </summary>
    public class DisplayInsteadInPlaymodeAttribute : PropertyAttribute
    {
        public string replacementName { private set; get; }
        /// <summary>
        /// A tooltip to display alongside the general tooltip for this attribute.
        /// Use only on ScriptableObjects, use [Tooltip] in MonoBehaviours.
        /// </summary>
        public string tooltip;
        
        /// <param name="replacementName">
        /// The name of the property or field to display instead of the original serialized field.
        /// Use nameof() to avoid strings.
        /// </param>
        public DisplayInsteadInPlaymodeAttribute(string replacementName = null)
        {
            this.replacementName = replacementName;
        }
    }
}
