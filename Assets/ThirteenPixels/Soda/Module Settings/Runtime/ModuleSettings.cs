// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.ModuleSettings
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for project-wide settings objects.
    /// Create a subclass to add a single instance to the Project Settings window.
    /// Get that instance during runtime by using <see cref="ModuleSettings"/>.Get<MySettingsType>().
    /// </summary>
    public abstract class ModuleSettings : ScriptableObject
    {
        internal const string FOLDER_NAME = "13Pixels.ModuleSettings";
        internal static readonly Dictionary<Type, ModuleSettings> cache = new Dictionary<Type, ModuleSettings>();
        protected internal virtual string title => GetType().Name;


        public static T Get<T>() where T : ModuleSettings
        {
            T result;
            if (cache.TryGetValue(typeof(T), out var baseResult))
            {
                result = (T)baseResult;
            }
            else
            {
#if !UNITY_EDITOR
                result = Resources.Load<T>(FOLDER_NAME + "/" + typeof(T).FullName);
                if (!result)
                {
                    result = CreateInstance<T>();
                }
#else
                result = CreateInstance<T>();
#endif
                cache.Add(typeof(T), result);
            }

            return result;
        }
    }
}
