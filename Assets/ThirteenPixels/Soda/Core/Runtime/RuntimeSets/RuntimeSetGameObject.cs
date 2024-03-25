// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// Stores only the GameObject itself for some limited, but optimized usage cases - mainly to check whether a GameObject is in it or to destroy all elements at once.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/RuntimeSet/GameObject", order = 10)]
    public class RuntimeSetGameObject : RuntimeSetBase, IEnumerable<GameObject>
    {
        internal override sealed Type itemType => typeof(GameObject);

        private readonly List<GameObject> list = new List<GameObject>();
        private readonly HashSet<GameObject> set = new HashSet<GameObject>();

        /// <summary>
        /// The number of elements currently registered to this RuntimeSet.
        /// </summary>
        public override int elementCount
        {
            get { return set.Count; }
        }

        public GameObject this[int index]
        {
#line hidden
            get
            {
                return list[index];
            }
#line default
        }
        

        public override bool Add(GameObject gameObject)
        {
            if (set.Add(gameObject))
            {
                list.Add(gameObject);
                onElementCountChange.Invoke(elementCount);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Insert(int index, GameObject gameObject)
        {
            if (set.Add(gameObject))
            {
                list.Insert(index, gameObject);
                onElementCountChange.Invoke(elementCount);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not the given GameObject matches the specifications of this RuntimeSet, which means that it can be added.
        /// </summary>
        public override bool Allows(GameObject gameObject)
        {
            return true;
        }

        public override bool Contains(GameObject go)
        {
            return set.Contains(go);
        }

        /// <summary>
        /// Returns the GameObject with the specified index.
        /// Same as using the index accessor (myRuntimeSet[index]).
        /// </summary>
        public sealed override GameObject GetGameObjectAt(int index)
        {
#line hidden
            return this[index];
#line default
        }

        internal override IEnumerable<GameObject> GetAllGameObjects()
        {
            return list;
        }

        public override void Remove(GameObject go)
        {
            if (set.Remove(go))
            {
                list.Remove(go);
                onElementCountChange.Invoke(elementCount);
            }
        }

        public override void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        /// <summary>
        /// Invokes the given action on every element in this RuntimeSet.
        /// Adding or removing elements to or from this set is okay.
        /// Elements added during the iteration are not taken into account for it.
        /// </summary>
        public void ForEach(Action<GameObject> action)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                action(list[i]);
            }
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            return list.AsReadOnly().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<GameObject>)this).GetEnumerator();
        }
    }
}
