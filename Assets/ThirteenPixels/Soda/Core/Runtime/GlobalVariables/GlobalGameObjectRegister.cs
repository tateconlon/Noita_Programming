// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Add this component to a GameObject and assign a GlobalGameObject value to assign the GameObject to it.
    /// This component must be active and enabled.
    /// The GameObject's component setup must match any requirements imposed by the referenced GlobalGameObject.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(GlobalGameObjectRegister))]
    [AddComponentMenu("Soda/GlobalGameObject Register")]
    public class GlobalGameObjectRegister : MonoBehaviour
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            internal const string globalGameObject = nameof(_globalGameObject);
        }
#endif

        [SerializeField]
        [DisplayInsteadInPlaymode(nameof(globalGameObject))]
        private GlobalGameObject _globalGameObject = default;

        /// <summary>
        /// The GlobalGameObject to register with.
        /// This can be changed at runtime - the component will unregister from the previous GlobalGameObject and register with the new one.
        /// </summary>
        public GlobalGameObject globalGameObject
        {
            get
            {
                return _globalGameObject;
            }
            set
            {
                if (value == _globalGameObject) return;

                if (_globalGameObject != null && enabled)
                {
                    _globalGameObject.onChange.RemoveResponse(CheckValue);
                    _globalGameObject.value = null;
                }

                _globalGameObject = value;

                if (_globalGameObject != null && enabled)
                {
                    var couldSetValue = _globalGameObject.TrySetValue(gameObject);
                    if (couldSetValue)
                    {
                        _globalGameObject.onChange.AddResponse(CheckValue);
                    }
                    else
                    {
                        enabled = false;
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (globalGameObject == null)
            {
                Debug.LogError("GlobalGameObjectRegister doesn't have a GlobalGameObject assigned.", gameObject);
                enabled = false;
                return;
            }

            var couldSetValue = globalGameObject.TrySetValue(gameObject);

            if (!couldSetValue)
            {
                Debug.LogError("GlobalGameObjectRegister couldn't register the GameObject due to its components not matching the requirements.", gameObject);
                enabled = false;
                return;
            }

            globalGameObject.onChange.AddResponse(CheckValue);
        }

        private void OnDisable()
        {
            if (globalGameObject == null) return;

            globalGameObject.onChange.RemoveResponse(CheckValue);
            if (globalGameObject.value == gameObject)
            {
                globalGameObject.value = null;
            }
        }

        private void CheckValue(GameObject newGO)
        {
            if (newGO != gameObject)
            {
                enabled = false;
            }
        }
    }
}
