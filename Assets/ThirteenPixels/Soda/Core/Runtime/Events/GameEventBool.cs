// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(menuName = "Soda/GameEvent/Bool", order = 250)]
    public class GameEventBool : GameEventBase<bool>
    {
        [System.Serializable]
        private class BoolEvent : UnityEvent<bool> { }
        [SerializeField]
        private BoolEvent _onRaiseGlobally = default;
        protected override UnityEvent<bool> onRaiseGlobally => _onRaiseGlobally;
    }
}
