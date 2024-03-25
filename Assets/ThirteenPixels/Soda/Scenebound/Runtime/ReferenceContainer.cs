// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Scenebound
{
    using UnityEngine;
    using System.Collections.Generic;

    internal class ReferenceContainer : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        internal List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();

        private void OnDestroy()
        {
            foreach (var scriptableObject in scriptableObjects)
            {
                Destroy(scriptableObject);
            }
        }
    }
}
