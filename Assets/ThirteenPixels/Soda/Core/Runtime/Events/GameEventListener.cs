// Copyright © Sascha Graeff/13Pixels.

using Sirenix.OdinInspector;

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// A MonoBehaviour listening to a specific GameEvent.
    /// Whenever the GameEvent is raised, the response UnityEvent is invoked.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(GameEventListener))]
    [AddComponentMenu("Soda/GameEvent Listener")]
    public class GameEventListener : MonoBehaviour
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            internal const string gameEvent = nameof(_gameEvent);
            internal const string deactivateAfterRaise = nameof(GameEventListener.deactivateAfterRaise);
            internal const string response = nameof(_response);
            internal const string listenWhileDisabled = nameof(listenWhileDisabled);
            internal const string shouldDisableCheckbox = nameof(shouldDisableCheckbox);
        }
#endif
        
        [DisableIf(nameof(ShouldDisableCheckbox))]
        [SerializeField] bool listenWhileDisabled = false;  // Shouldn't be able to modify at runtime

        bool ShouldDisableCheckbox => Application.isPlaying;


        [Tooltip("The event to react upon.")]
        [SerializeField]
        private GameEventBase _gameEvent;
        /// <summary>
        /// The GameEvent to listen to.
        /// This can be changed at runtime - the response will be removed from the old GameEvent and added to the new one.
        /// </summary>
        public GameEventBase gameEvent
        {
            get => _gameEvent;
            set
            {
                if (_gameEvent == value) return;

                if ((this.enabled || listenWhileDisabled) && _gameEvent)
                {
                    _gameEvent.GetOnRaiseBase().RemoveResponse(OnEventRaised);
                }

                _gameEvent = value;

                if ((this.enabled || listenWhileDisabled) && _gameEvent)
                {
                    _gameEvent.GetOnRaiseBase().AddResponse(OnEventRaised);
                }
            }
        }

        [Tooltip("Deactivate this component after its GameEvent was raised.")]
        public bool deactivateAfterRaise = false;

        [Space]
        [Tooltip("Response to invoke when the event is raised.")]
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("response")]
        private UnityEvent _response = default;
        /// <summary>
        /// The response to invoke when the referenced GameEvent is being raised.
        /// </summary>
        public UnityEvent response => _response;

        void Awake()
        {
            if (gameEvent && listenWhileDisabled)
            {
                gameEvent.GetOnRaiseBase().AddResponse(OnEventRaised);
            }
        }

        private void OnEnable()
        {
            if (gameEvent && !listenWhileDisabled)
            {
                gameEvent.GetOnRaiseBase().AddResponse(OnEventRaised);
            }
        }

        private void OnDisable()
        {
            if (gameEvent && !listenWhileDisabled)
            {
                gameEvent.GetOnRaiseBase().RemoveResponse(OnEventRaised);
            }
        }

        void OnDestroy()
        {
            if (gameEvent && listenWhileDisabled)
            {
                gameEvent.GetOnRaiseBase().RemoveResponse(OnEventRaised);
            }
        }

        internal void OnEventRaised()
        {
            response.Invoke();

            if (deactivateAfterRaise)
            {
                this.enabled = false;
            }
        }
    }
}
