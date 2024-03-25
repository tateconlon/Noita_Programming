// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using System;

    /// <summary>
    /// Declares a ScriptableObject type as "scene bindable".
    /// This allows the Soda Scenebound editor to create instances of the class and its subclasses and bind them to the scene rather than the Assets folder.
    /// This does not put any restrictions on the type, so it can still also be instantiated into the Assets folder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class SceneBindableAttribute : Attribute
    {
        public string categoryName { get; }
        public bool isCoreItem { get; }

        public SceneBindableAttribute(string categoryName) : this(categoryName, false)
        {
        }

        internal SceneBindableAttribute(string categoryName, bool isCoreItem)
        {
            this.categoryName = categoryName;
            this.isCoreItem = isCoreItem;
        }
    }
}
