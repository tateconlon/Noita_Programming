// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.SubAssets
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Use this attribute on a ScriptableObject's serialized field to make Soda automatically create and maintain a sub asset
    /// in each instance of that ScriptableObject class.
    /// The serialized field must be in a ScriptableObject class and of a type that derives from ScriptableObject.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SubAssetAttribute : PropertyAttribute
    {
        public bool optional = false;
    }
}
