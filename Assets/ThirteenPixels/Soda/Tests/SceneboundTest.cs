// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using System.Collections;

    public class SceneboundTest
    {
        /// <summary>
        /// Tests whether the Scenebound system causes an exception when changing selection in an empty scene.
        /// </summary>
        [UnityTest]
        public IEnumerator ChangeselectionInEmptyScene()
        {
            var someEvent = ScriptableObject.CreateInstance<GameEvent>();
            yield return null;
            Selection.activeObject = someEvent;
            yield return null;
            Object.DestroyImmediate(someEvent);
        }
    }
}
