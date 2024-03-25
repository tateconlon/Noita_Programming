// Copyright © Sascha Graeff/13Pixels.

using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A MonoBehaviour used to add a GameObject to a RuntimeSet.
    /// The element is only part of the RuntimeSet while it's active.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(RuntimeSetElement))]
    [AddComponentMenu("Soda/RuntimeSet Element")]
    public class RuntimeSetElement : MonoBehaviour
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            internal const string runtimeSet = nameof(_runtimeSet);
            internal const string AddWhileDisabled = nameof(AddWhileDisabled);
        }
#endif
        [SerializeField] bool AddWhileDisabled = false;  // Shouldn't be able to modify at runtime

        [Tooltip("The RuntimeSet to add this GameObject to.")]
        [SerializeField]
        [DisplayInsteadInPlaymode(nameof(runtimeSet))]
        private RuntimeSetBase _runtimeSet;
        /// <summary>
        /// The RuntimeSet to register with.
        /// This can be changed at runtime - the component will unregister from the previous RuntimeSet and register with the new one.
        /// </summary>
        public RuntimeSetBase runtimeSet
        {
            get
            {
                return _runtimeSet;
            }
            set
            {
                if (value == _runtimeSet) return;

                if (_runtimeSet && (this.enabled || AddWhileDisabled))
                {
                    _runtimeSet.Remove(gameObject);
                }

                _runtimeSet = value;

                if (_runtimeSet && (this.enabled || AddWhileDisabled))
                {
                    var couldAdd = _runtimeSet.Add(gameObject);
                    if (!couldAdd)
                    {
                        enabled = false;
                        Debug.Log($"Couldn't add {name} to runtimeSet {runtimeSet.name}", this);
                    }
                }
            }
        }

        //Awake doesn't run on inactive gameobjects. Add a RuntimeSetElementManager component in scene
        //which will add the disabled RuntimeSetElement to the runtimeset.
        void Awake()
        {
            if (runtimeSet && AddWhileDisabled)
            {
                var couldAdd = runtimeSet.Add(gameObject);
                if (!couldAdd)
                {
                    Debug.Log($"Couldn't add {name} to runtimeSet {runtimeSet.name}", this);
                }
            }
        }

        private void OnEnable()
        {
            if(!runtimeSet)
            {
                Debug.LogError("This RuntimeSetElement does not have a RuntimeSet assigned.", this);
                enabled = false;
                return;
            }
            
            if (!AddWhileDisabled)
            {
                var couldAdd = runtimeSet.Add(gameObject);
                if (!couldAdd)
                {
                    enabled = false;
                    Debug.Log($"Couldn't add {name} to runtimeSet {runtimeSet.name}", this);
                }
            }
        }

        private void OnDisable()
        {
            if (runtimeSet && !AddWhileDisabled)
            {
                runtimeSet.Remove(gameObject);
            }
        }

        public void AddToRuntimeSet()
        {
            if (AddWhileDisabled)
            {
                var couldAdd = runtimeSet.Add(gameObject);
                if (!couldAdd)
                {
                    Debug.Log($"Couldn't add {name} to runtimeSet {runtimeSet.name}", this);
                }
            }
        }

        public void InspectorSetAddWhileDisabled(bool val)
        {
            if (Application.isPlaying)
            {
                //Debug.LogError("Can't change RuntimeSetElement.AddWhileDisabled at Runtime. Only for inspector purposes!", this);
                return;
            }

            AddWhileDisabled = val;
        }

        void OnDestroy()
        {
            runtimeSet.Remove(gameObject);
        }
    }

}
