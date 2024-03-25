// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// The type T might refer to a single component that all GameObjects have to have in order to be able to be registered.
    /// Alternatively, it could refer to a struct that contains multiple references to required components.
    /// </summary>
    public abstract class RuntimeSetBase<T> : RuntimeSetBase, IEnumerable<T>
    {
        internal override sealed Type itemType => typeof(T);

        private readonly Dictionary<GameObject, T> dict = new Dictionary<GameObject, T>();
        private readonly Dictionary<T, GameObject> reverseDict = new Dictionary<T, GameObject>();
        private readonly List<T> list = new List<T>();

        /// <summary>
        /// The number of elements currently registered to this RuntimeSet.
        /// </summary>
        public sealed override int elementCount
        {
            get { return dict.Count; }
        }

        /// <summary>
        /// Gets the element with the specified index.
        /// </summary>
        public T this[int index]
        {
#line hidden
            get => list[index];
#line default
        }


        /// <summary>
        /// Adds a GameObject to the RuntimeSet.
        /// Only succeeds when an "element" could be created by applying the RuntimeSet's rules.
        /// </summary>
        /// <param name="gameObject">The GameObject to add to the RuntimeSet.</param>
        /// <returns>True if the GameObject could be added to the RuntimeSet. False if the GameObject was already added or no proper element could be created from it.</returns>
        public sealed override bool Add(GameObject gameObject)
        {
            if (gameObject == null) return false;
            
            var success = false;
            if (TryCreateElement(gameObject, out T element))
            {
                success = AddToCollections(gameObject, element);
                if (success)
                {
                    OnAddElement(element);
                    onElementCountChange.Invoke(elementCount);
                }
            }

            return success;
        }

        /// <summary>
        /// Adds a GameObject to the RuntimeSet at a specific index.
        /// Throws an exception if done while iterating over the set.
        /// </summary>
        /// <param name="index">The index at which to insert the element.</param>
        /// <param name="gameObject">The GameObject to add to the RuntimeSet.</param>
        /// <returns>True if the GameObject could be added to the RuntimeSet. False if the GameObject was already added or no proper element could be created from it.</returns>
        public sealed override bool Insert(int index, GameObject gameObject)
        {
            if (gameObject == null) return false;
            
            var success = false;
            if (TryCreateElement(gameObject, out T element))
            {
                success = AddToCollections(gameObject, element, index);
                if (success)
                {
                    OnAddElement(element);
                    onElementCountChange.Invoke(elementCount);
                }
            }

            return success;
        }

        /// <summary>
        /// Returns whether or not the given GameObject matches the specifications of this RuntimeSet, which means that it can be added.
        /// </summary>
        public override bool Allows(GameObject gameObject)
        {
            return TryCreateElement(gameObject, out var result);
        }

        /// <summary>
        /// Creates an "element" for the RuntimeSet if possible.
        /// An element could be a component reference or a struct comtaining multiple references.
        /// </summary>
        /// <param name="gameObject">The GameObject to create the element from.</param>
        /// <param name="element">The element that was created.</param>
        /// <returns>Whether or not the element could be created. If, for example, the required component was missing on the given GameObject, this is false.</returns>
        protected virtual bool TryCreateElement(GameObject gameObject, out T element)
        {
            return ComponentCache.TryCreateViaReflection(gameObject,
                                                         out element,
                                                         () => new Exception("Trying to initialize a component cache with a cache type that is neither a component nor a struct. Please use a component type or a struct, or override " + GetType() + "." + nameof(TryCreateElement) + " to allow this."));
        }

        /// <summary>
        /// Called whenever an element is successfully added.
        /// Override this to initialize the element to be in the set.
        /// For example, you can make the RuntimeSet monitor changes in its element.
        /// </summary>
        /// <param name="element">The element that was added.</param>
        protected virtual void OnAddElement(T element)
        {

        }

        /// <summary>
        /// Removes a GameObject from the RuntimeSet.
        /// </summary>
        /// <param name="gameObject">The GameObject to remove.</param>
        public sealed override void Remove(GameObject gameObject)
        {
            if (dict.TryGetValue(gameObject, out T element))
            {
                RemoveFromDictionaries(element, gameObject);
                list.Remove(element);

                OnRemoveElement(element);
                onElementCountChange.Invoke(elementCount);
            }
        }
        
        /// <summary>
        /// Removes a GameObject from the RuntimeSet.
        /// </summary>
        /// <param name="index">The index at which the GameObject should be removed.</param>
        public sealed override void RemoveAt(int index)
        {
#line hidden
            var element = list[index];
#line default
            var gameObject = reverseDict[element];

            RemoveFromDictionaries(element, gameObject);
            list.RemoveAt(index);

            OnRemoveElement(element);
            onElementCountChange.Invoke(elementCount);
        }

        private void RemoveFromDictionaries(T element, GameObject gameObject)
        {
            dict.Remove(gameObject);
            reverseDict.Remove(element);
        }

        /// <summary>
        /// Called whenever an element is successfully removed.
        /// Override this to return the element to the state it was before being added to this RuntimeSet.
        /// For example, you can revoke any monitoring of the element done by the RuntimeSet.
        /// </summary>
        /// <param name="element">The element that was removed.</param>
        protected virtual void OnRemoveElement(T element)
        {

        }

        private bool AddToCollections(GameObject gameObject, T element, int index = -1)
        {
            if (index > elementCount) throw new ArgumentOutOfRangeException("RuntimeSet.Insert index was out of range.");

            if (!dict.ContainsKey(gameObject))
            {
                dict.Add(gameObject, element);
                reverseDict.Add(element, gameObject);
                if (index >= 0)
                {
                    list.Insert(index, element);
                }
                else
                {
                    list.Add(element);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether a specific GameObject is part of this set.
        /// </summary>
        public sealed override bool Contains(GameObject gameObject)
        {
            return dict.ContainsKey(gameObject);
        }

        /// <summary>
        /// Returns the GameObject with the specified index.
        /// </summary>
        public sealed override GameObject GetGameObjectAt(int index)
        {
#line hidden
            var element = list[index];
#line default
            return reverseDict[element];
        }

        /// <summary>
        /// Returns the first element (which was probably added first) or the default value if the RuntimeSet is empty.
        /// </summary>
        public T GetFirstOrDefaultElement()
        {
            return elementCount > 0 ? this[0] : default;
        }

        /// <summary>
        /// Returns the last element (which is probably the last that was added) or the default value if the RuntimeSet is empty.
        /// </summary>
        public T GetLastOrDefaultElement()
        {
            return elementCount > 0 ? this[elementCount - 1] : default;
        }

        /// <summary>
        /// Invokes the given action on every element in this RuntimeSet.
        /// Adding or removing elements to or from this set is okay.
        /// Elements added during the iteration are not taken into account for it.
        /// </summary>
        public void ForEach(Action<T> action)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// Returns a collection of all GameObjects in this RuntimeSet that updates if the RuntimeSet contents change.
        /// </summary>
        internal sealed override IEnumerable<GameObject> GetAllGameObjects()
        {
            return dict.Keys;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return reverseDict.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// </summary>
    [HelpURL(SodaDocumentation.URL_RUNTIME + nameof(RuntimeSetBase))]
    [SceneBindable("RuntimeSets", isCoreItem: true)]
    public abstract class RuntimeSetBase : ScriptableObject
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            internal const string description = nameof(RuntimeSetBase.description);
        }
#endif

        internal abstract Type itemType { get; }

        #region Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion

        /// <summary>
        /// The number of elements currently registered to this RuntimeSet.
        /// </summary>
        public abstract int elementCount { get; }

        private SodaEvent<int> _onElementCountChange;
        /// <summary>
        /// This event is invoked when an item is added or removed.
        /// </summary>
        public SodaEvent<int> onElementCountChange
        {
            get
            {
                if (_onElementCountChange == null)
                {
                    _onElementCountChange = new SodaEvent<int>(() => elementCount);
                }
                return _onElementCountChange;
            }
        }

#if UNITY_EDITOR
        internal SodaEventBase GetOnElementCountChangeEvent()
        {
            return onElementCountChange;
        }
#endif


        protected virtual void Awake()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        /// <summary>
        /// Adds a GameObject to the RuntimeSet.
        /// Only succeeds when an "element" could be created by applying the RuntimeSet's rules.
        /// </summary>
        /// <param name="gameObject">The GameObject to add to the RuntimeSet.</param>
        /// <returns>True if the GameObject could be added to the RuntimeSet. False if the GameObject was already added or no proper element could be created from it.</returns>
        public abstract bool Add(GameObject gameObject);

        /// <summary>
        /// Adds a GameObject to the RuntimeSet at a specific index.
        /// Throws an exception if done while iterating over the set.
        /// </summary>
        /// <param name="index">The index at which to insert the element.</param>
        /// <param name="gameObject">The GameObject to add to the RuntimeSet.</param>
        /// <returns>True if the GameObject could be added to the RuntimeSet. False if the GameObject was already added or no proper element could be created from it.</returns>
        public abstract bool Insert(int index, GameObject gameObject);

        /// <summary>
        /// Returns whether or not the given GameObject matches the specifications of this RuntimeSet, which means that it can be added.
        /// </summary>
        public abstract bool Allows(GameObject gameObject);

        /// <summary>
        /// Removes a GameObject from the RuntimeSet.
        /// </summary>
        /// <param name="gameObject">The GameObject to remove.</param>
        public abstract void Remove(GameObject gameObject);

        /// <summary>
        /// Removes a GameObject from the RuntimeSet.
        /// </summary>
        /// <param name="index">The index at which the GameObject should be removed.</param>
        public abstract void RemoveAt(int index);

        /// <summary>
        /// Checks whether a specific GameObject is part of this set.
        /// </summary>
        public abstract bool Contains(GameObject gameObject);

        /// <summary>
        /// Returns the GameObject with the specified index.
        /// </summary>
        public abstract GameObject GetGameObjectAt(int index);

        /// <summary>
        /// Returns the first GameObject (which was probably added first) or null if the RuntimeSet is empty.
        /// </summary>
        public GameObject GetFirstOrDefault()
        {
            return elementCount > 0 ? GetGameObjectAt(0) : null;
        }

        /// <summary>
        /// Returns the last GameObject (which is probably the last that was added) or null if the RuntimeSet is empty.
        /// </summary>
        public GameObject GetLastOrDefault()
        {
            return elementCount > 0 ? GetGameObjectAt(elementCount - 1) : null;
        }

        /// <summary>
        /// Returns a collection of all GameObjects in this RuntimeSet.
        /// Used for the inspector window to display a list of all items.
        /// </summary>
        internal abstract IEnumerable<GameObject> GetAllGameObjects();
    }
}
