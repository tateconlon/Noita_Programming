// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    /// <summary>
    /// A ScriptableObject representing a global event.
    /// Base class for GameEvents with and without parameter.
    /// </summary>
    [SceneBindable("GameEvents", isCoreItem: true)]
    public abstract class GameEventBase : ScriptableObject
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            internal const string description = nameof(GameEventBase.description);
            internal const string debug = nameof(GameEventBase._debug);
        }
#endif

        internal virtual Type parameterType => null;

        #region Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion

        [SerializeField]
        private bool _debug = false;
        protected internal bool debug => _debug;

        // For preventing cyclic/recursive invocation
        protected bool currentlyBeingRaised = false;

        /// <summary>
        /// Returns the onRaise SodaEvent.
        /// This can be used to have a list of GameEvents of different types, and adding parameterless responses to all of them.
        /// </summary>
        public abstract SodaEventBase GetOnRaiseBase();
    }

    /// <summary>
    /// A ScriptableObject representing a global event with a parameter.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(GameEventBase))]
    public abstract class GameEventBase<T> : GameEventBase
    {
#if UNITY_EDITOR
        new internal static class PropertyNames
        {
            internal const string parameterDescription = nameof(GameEventBase<T>.parameterDescription);
        }
#endif

        internal override Type parameterType => typeof(T);

        [Obsolete("Please use onRaise instead.")]
        public readonly SodaEvent<T> onRaiseWithParameter = new SodaEvent<T>();
        public readonly SodaEvent<T> onRaise = new SodaEvent<T>();
        
        protected abstract UnityEvent<T> onRaiseGlobally { get; }

        #region Parameter Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string parameterDescription = "";
#pragma warning restore 0414
        #endregion

        /// <summary>
        /// Call this method to raise the event, leading to all added responses being invoked.
        /// </summary>
        public void Raise(T parameter)
        {
            if (debug)
            {
                Debug.Log("GameEvent \"" + name + "\" was raised.\n", this);
            }

            if (currentlyBeingRaised)
            {
                Debug.LogWarning("Event is already being raised, preventing recursive raise.", this);
                return;
            }

            currentlyBeingRaised = true;

            try
            {
                onRaiseGlobally.Invoke(parameter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            onRaise.Invoke(parameter);
#pragma warning disable 618
            onRaiseWithParameter.Invoke(parameter);
#pragma warning restore 618

            currentlyBeingRaised = false;
        }

        public sealed override SodaEventBase GetOnRaiseBase()
        {
            return onRaise;
        }
    }
}
