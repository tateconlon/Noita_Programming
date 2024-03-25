// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class RuntimeSetElementTest
    {
        private RuntimeSetGameObject runtimeSet;
        private RuntimeSetElement runtimeSetElement;

        [SetUp]
        public void SetUp()
        {
            runtimeSet = ScriptableObject.CreateInstance<RuntimeSetGameObject>();

            runtimeSetElement = new GameObject("Test GO").AddComponent<RuntimeSetElement>();
        }

        private static void Enable<T>(T target) where T : Behaviour
        {
            target.enabled = true;
            var onEnable = typeof(T).GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            onEnable.Invoke(target, new object[0]);
        }

        private static void Disable<T>(T target) where T : Behaviour
        {
            target.enabled = false;
            var onDisable = typeof(T).GetMethod("OnDisable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            onDisable.Invoke(target, new object[0]);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(runtimeSetElement.gameObject);

            Object.DestroyImmediate(runtimeSet);
        }

        [Test]
        public void NewComponentWithoutTargetDisablesItself()
        {
            // We could check for the error with LogAssert.Expect, but it still ends up in the console,
            // which is a worrying sight when running tests.
            Debug.unityLogger.logEnabled = false;

            Enable(runtimeSetElement);
            Assert.IsFalse(runtimeSetElement.isActiveAndEnabled);

            Debug.unityLogger.logEnabled = true;
        }

        [Test]
        public void NewComponentWithTargetRegistersToTarget()
        {
            runtimeSetElement.runtimeSet = runtimeSet;

            Assert.IsTrue(runtimeSetElement.isActiveAndEnabled);
            Assert.AreEqual(1, runtimeSet.elementCount);
            Assert.IsTrue(runtimeSet.Contains(runtimeSetElement.gameObject));
        }

        [Test]
        public void DisablingUnregisters()
        {
            runtimeSetElement.runtimeSet = runtimeSet;
            Enable(runtimeSetElement);

            Disable(runtimeSetElement);

            Assert.IsFalse(runtimeSetElement.isActiveAndEnabled);
            Assert.AreEqual(0, runtimeSet.elementCount);
            Assert.IsFalse(runtimeSet.Contains(runtimeSetElement.gameObject));
        }
    }
}
