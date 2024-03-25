// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;

    /// <summary>
    /// Base class for GlobalVariables.
    /// Each GlobalVariable represents a single global variable stored in an injectable ScriptableObject.
    /// </summary>
    public abstract class GlobalVariableBase<T> : GlobalVariableBase, ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        new internal static class PropertyNames
        {
            internal const string originalValue = nameof(GlobalVariableBase<T>.originalValue);
        }
#endif

        public override sealed Type valueType => typeof(T);  // ASSETMOD: made public

        [SerializeField]
        [DisplayInsteadInPlaymode(nameof(value), tooltip = "The value this object currently has.")]
        private T originalValue = default;
        private T _value;

        /// <summary>
        /// The value this GlobalVariable represents.
        /// </summary>
        public virtual T value
        {
            get
            {
                return _value;
            }
            set
            {
                if (Equals(_value, value)) return;

                _value = value;
                onChange.Invoke(value);
            }
        }
        
        // ASSETMOD: for NodeCanvas compatability
        ///<summary>Same as .value. Used for binding in NodeCanvas.</summary>
        public T GetValue() { return value; }
        ///<summary>Same as .value. Used for binding in NodeCanvas.</summary>
        public void SetValue(T newValue) { this.value = newValue; }

        private SodaEvent<T> _onChange;

        /// <summary>
        /// This event is invoked when the value changes.
        /// </summary>
        public SodaEvent<T> onChange
        {
            get
            {
                if (_onChange == null)
                {
                    _onChange = new SodaEvent<T>(() => value);
                }
                return _onChange;
            }
        }

        internal override sealed SodaEventBase GetOnChangeEvent()
        {
            return onChange;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (!shouldDoPostDeserialization) return;
#endif

            _value = originalValue;
            OnAfterDeserialize();
        }

        protected virtual void OnAfterDeserialize()
        {

        }

        // ASSETMOD
        public override void ResetValue()
        {
            value = originalValue;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /// <summary>
        /// Copies the value from another GlobalVariable via UnityEvent.
        /// The passed GlobalVariable has to be of the same type as this one.
        /// Intended for use via UnityEvent.
        /// </summary>
        public void CopyValueFrom(GlobalVariableBase other)
        {
            var otherGlobalVariable = other as GlobalVariableBase<T>;
            if (otherGlobalVariable != null)
            {
                value = otherGlobalVariable.value;
            }
            else
            {
                Debug.LogError("Can only copy the value from another GlobalVariable of the same type.");
            }
        }
        
        public static implicit operator T(GlobalVariableBase<T> @this) => @this.value;  // ASSETMOD
        
        public override object valueObject => value;  // ASSETMOD
    }

    /// <summary>
    /// Base class for GlobalVariables.
    /// This non-generic base class allows other classes to use GlobalVariables regardless of their generic type, which used for the savestate system.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(GlobalVariableBase))]
    [SceneBindable("GlobalVariables", isCoreItem: true)]
    public abstract class GlobalVariableBase : ScriptableObject
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            internal const string description = nameof(GlobalVariableBase.description);
        }

        internal bool shouldDoPostDeserialization = true;
#endif

        public abstract Type valueType { get; }  // ASSETMOD: made public

        #region Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion


        protected virtual void Awake()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        public abstract object valueObject { get; }  // ASSETMOD

        /// <summary>
        /// Loads the value for this GlobalVariable by using the provided ISavesateReader and key.
        /// Override this in a GlobalVariable class to allow it to do so.
        /// </summary>
        public virtual void LoadValue(ISavestateReader reader, string key)
        {
            throw new NotSupportedException("The GlobalVariable class \"" + GetType() + "\" does not support value loading.");
        }

        /// <summary>
        /// Saves the value of this GlobalVariable by using the provided ISavesateWriter and key.
        /// Override this in a GlobalVariable class to allow it to do so.
        /// </summary>
        public virtual void SaveValue(ISavestateWriter writer, string key)
        {
            throw new NotSupportedException("The GlobalVariable class \"" + GetType() + "\" does not support value saving.");
        }

        public abstract void ResetValue();  // ASSETMOD

#if UNITY_EDITOR
#line hidden
        public static implicit operator bool(GlobalVariableBase globalVariable)
        {
            Debug.LogWarning("You are implicitly converting a GlobalVariable to a bool. Please use an explicit != null instead to avoid confusion with reading the value property.", globalVariable);
            return globalVariable != null;
        }
#line default
#endif

        internal abstract SodaEventBase GetOnChangeEvent();
    }
}
