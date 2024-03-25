// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class GlobalGameObjectRegisterTest
    {
        private GlobalGameObject globalGameObject;
        private GlobalGameObjectRegister globalGameObjectRegister;

        [SetUp]
        public void SetUp()
        {
            globalGameObject = ScriptableObject.CreateInstance<GlobalGameObject>();

            globalGameObjectRegister = new GameObject("Test GO").AddComponent<GlobalGameObjectRegister>();
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
            Object.DestroyImmediate(globalGameObjectRegister.gameObject);

            Object.DestroyImmediate(globalGameObject);
        }

        [Test]
        public void NewComponentWithoutTargetDisablesItself()
        {
            // We could check for the error with LogAssert.Expect, but it still ends up in the console,
            // which is a worrying sight when running tests.
            Debug.unityLogger.logEnabled = false;

            Enable(globalGameObjectRegister);
            Assert.IsFalse(globalGameObjectRegister.isActiveAndEnabled);

            Debug.unityLogger.logEnabled = true;
        }

        [Test]
        public void NewComponentWithTargetRegistersToTarget()
        {
            globalGameObjectRegister.globalGameObject = globalGameObject;

            Assert.IsTrue(globalGameObjectRegister.isActiveAndEnabled);
            Assert.AreEqual(globalGameObjectRegister.gameObject, globalGameObject.value);
        }

        [Test]
        public void DisablingUnregisters()
        {
            globalGameObjectRegister.globalGameObject = globalGameObject;
            Enable(globalGameObjectRegister);

            Disable(globalGameObjectRegister);

            Assert.IsFalse(globalGameObjectRegister.isActiveAndEnabled);
            Assert.AreEqual(null, globalGameObject.value);
        }

        [Test]
        public void ComponentBecomesDisabledWhenTargetChangesValue()
        {
            globalGameObjectRegister.globalGameObject = globalGameObject;
            Enable(globalGameObjectRegister);

            globalGameObject.value = null;

            Assert.IsFalse(globalGameObjectRegister.isActiveAndEnabled);
        }
    }
}
