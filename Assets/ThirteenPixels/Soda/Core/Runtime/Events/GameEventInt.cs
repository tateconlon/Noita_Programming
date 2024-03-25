// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(menuName = "Soda/GameEvent/Int", order = 250)]
    public class GameEventInt : GameEventBase<int>
    {
        [System.Serializable]
        private class IntEvent : UnityEvent<int> { }
        [SerializeField]
        private IntEvent _onRaiseGlobally = default;
        protected override UnityEvent<int> onRaiseGlobally => _onRaiseGlobally;
    }
}
