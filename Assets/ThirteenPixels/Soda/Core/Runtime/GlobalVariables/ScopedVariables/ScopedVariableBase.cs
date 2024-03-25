﻿// Copyright © Sascha Graeff/13Pixels.

// This ScopedVariable implementation is obsolete for Unity 2020.1 and higher.
// Please see ScopedVariable.cs for the new implementation, which is mostly identical.
// To not have an uncomfortable amount of #if directives in one file, we now have two of them with one of them completely disabled depending on editor version.

#if !UNITY_2020_1_OR_NEWER
namespace ThirteenPixels.Soda
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Base class for ScopedVariable types.
    /// A ScopedVariable is a type that allows to create a field that either contains a local value
    /// or a GlobalVariable that represents a value of the same type.
    /// </summary>
    /// <typeparam name="T">The type of the value being represented.</typeparam> 
    /// <typeparam name="GVT">The type of GlobalVariable that can be referenced to also represent a value of type T.</typeparam> 
    public abstract class ScopedVariableBase<T, GVT> : ScopedVariableBase where GVT : GlobalVariableBase<T>
    {
        [SerializeField]
        private GVT globalVariable = default;
        [SerializeField]
        private T localValue = default;
        [SerializeField]
        private bool useLocal = false;

        /// <summary>
        /// The value represented by this ScopedVariable.
        /// Depending on the ScopedVariable's state, this refers to the local value or the value of the referenced GlobalVariable.
        /// </summary>
        public T value
        {
            get { return useLocal ? localValue : (globalVariable != null ? globalVariable.value : default(T)); }
            set
            {
                if (useLocal)
                {
                    if (!Equals(localValue, value))
                    {
                        localValue = value;
                        InvokeOnChangeEvent(value);
                    }
                }
                else if (globalVariable != null)
                {
                    globalVariable.value = value;
                }
                else
                {
                    throw new NullReferenceException("Trying to set a value to a GlobalVariable object, but there is none referenced.");
                }
            }
        }

        /// <summary>
        /// Creates a new ScopedVariable that is set to "GlobalVariable" mode.
        /// </summary>
        public ScopedVariableBase()
        {
        }

        /// <summary>
        /// Creates a new ScopedVariable with a default local value.
        /// Sets the ScopedVariable to local value mode.
        /// </summary>
        public ScopedVariableBase(T value)
        {
            localValue = value;
            useLocal = true;
        }

#region OnChangeValue event
        private SodaEvent<T> _onChangeValue;
        /// <summary>
        /// This event is invoked when the value changes, be it the local value, the value of the referenced GlobalVariable
        /// or when switching between local value and GlobalVariable.
        /// </summary>
        public SodaEvent<T> onChangeValue
        {
            get
            {
                if (_onChangeValue == null)
                {
                    _onChangeValue = new SodaEvent<T>(() => value, UpdateGlobalVariableMonitoring);
                }
                return _onChangeValue;
            }
        }

        internal override void UpdateGlobalVariableMonitoring()
        {
            if (globalVariable != null)
            {
                if (hasOnChangeResponses && !useLocal)
                {
                    globalVariable.onChange.AddResponse(InvokeOnChangeEvent);
                }
                else
                {
                    globalVariable.onChange.RemoveResponse(InvokeOnChangeEvent);
                }
            }
        }

        private void InvokeOnChangeEvent(T value)
        {
            onChangeValue.Invoke(value);
        }

        internal override void InvokeOnChangeEvent()
        {
            InvokeOnChangeEvent(value);
        }

        private bool hasOnChangeResponses
        {
            get { return onChangeValue.responseCount > 0; }
        }
#endregion

        /// <summary>
        /// Assign a GlobalVariable to be referenced by this ScopedVariable.
        /// Switches the ScopedVariable to "GlobalVariable" mode.
        /// </summary>
        public void AssignGlobalVariable(GVT globalVariable)
        {
            this.globalVariable = globalVariable;
            useLocal = false;

            UpdateGlobalVariableMonitoring();
            InvokeOnChangeEvent();
        }

        /// <summary>
        /// Assign a local value to this ScopedVariable.
        /// Switches the ScopedVariable to "local value" mode and nullifies any reference to a GlobalVariable.
        /// </summary>
        public void AssignLocalValue(T value)
        {
            localValue = value;
            useLocal = true;

            UpdateGlobalVariableMonitoring();

            globalVariable = null;

            InvokeOnChangeEvent();
        }

        internal override SodaEventBase GetOnChangeValueEvent()
        {
            return onChangeValue;
        }
    }

    /// <summary>
    /// Base class for ScopedVariable types.
    /// A ScopedVariable is a type that allows to create a field that either contains a local value
    /// or a GlobalVariable that represents a value of the same type.
    /// </summary>
    public abstract class ScopedVariableBase
    {
        internal abstract void InvokeOnChangeEvent();
        internal abstract void UpdateGlobalVariableMonitoring();
        internal abstract SodaEventBase GetOnChangeValueEvent();
    }
}
#endif