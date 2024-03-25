// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    /// <summary>
    /// A ScriptableObject representing a global event.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(GameEvent))]
    [CreateAssetMenu(menuName = "Soda/GameEvent/Simple", order = 299)]
    public class GameEvent : GameEventBase
    {
#if UNITY_EDITOR
        new internal static class PropertyNames
        {
            internal const string onRaiseGlobally = nameof(GameEvent.onRaiseGlobally);
        }
#endif

        public readonly SodaEvent onRaise = new SodaEvent();

        [SerializeField]
        private UnityEvent onRaiseGlobally = default;

        /// <summary>
        /// Call this method to raise the event, leading to all added responses being invoked.
        /// </summary>
        public void Raise()
        {
            if (debug)
            {
                Debug.Log("GameEvent \"" + name + "\" was raised.\n", this);
            }

#if UNITY_EDITOR
            EnsureOnRaiseGloballyExistence();
#endif
            if (currentlyBeingRaised)
            {
                Debug.LogWarning("Event is already being raised, preventing recursive raise.", this);
                return;
            }

            currentlyBeingRaised = true;

            try
            {
                onRaiseGlobally.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            onRaise.Invoke();

            currentlyBeingRaised = false;
        }

        public sealed override SodaEventBase GetOnRaiseBase()
        {
            return onRaise;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Initializes onRaiseGlobally if it doesn't exist. Used for test cases.
        /// </summary>
        private void EnsureOnRaiseGloballyExistence()
        {
            if (onRaiseGlobally == null)
            {
                onRaiseGlobally = new UnityEvent();
            }
        }
#endif
    }
}
