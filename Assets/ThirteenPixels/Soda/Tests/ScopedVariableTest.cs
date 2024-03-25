// Copyright © Sascha Graeff/13Pixels.

#if UNITY_2020_1_OR_NEWER
#define NEW_SCOPED_VARIABLES
#endif

namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;

    public class ScopedVariableTest
    {
        private GlobalInt globalInt;
#if NEW_SCOPED_VARIABLES
        private ScopedVariable<int> scopedInt;
#else
        private ScopedInt scopedInt;
#endif

        [SetUp]
        public void SetUp()
        {
#if NEW_SCOPED_VARIABLES
            scopedInt = new ScopedVariable<int>(0);
#else
            scopedInt = new ScopedInt(0);
#endif
            globalInt = ScriptableObject.CreateInstance<GlobalInt>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(globalInt);
        }

        [Test]
        public void Constructor()
        {
#if NEW_SCOPED_VARIABLES
            scopedInt = new ScopedVariable<int>(11);
#else
            scopedInt = new ScopedInt(11);
#endif
            Assert.AreEqual(11, scopedInt.value);
        }

        [Test]
        public void ValueChange()
        {
            scopedInt.value = 42;
            Assert.AreEqual(42, scopedInt.value);
        }

        [Test]
        public void ScopeChange()
        {
            globalInt.value = 42;
            scopedInt.AssignGlobalVariable(globalInt);
            Assert.AreEqual(42, scopedInt.value);
            Assert.AreEqual(42, globalInt.value);

            scopedInt.AssignLocalValue(1337);
            Assert.AreEqual(1337, scopedInt.value);
            Assert.AreEqual(42, globalInt.value);
        }

        [Test]
        public void OnChangeEvent()
        {
            globalInt.value = 42;
            scopedInt.AssignGlobalVariable(globalInt);
            var result = 0;

            scopedInt.onChangeValue.AddResponseAndInvoke(i => result = i);
            Assert.AreEqual(42, result);

            globalInt.value = 1337;
            Assert.AreEqual(1337, result);

            scopedInt.AssignLocalValue(420);
            Assert.AreEqual(420, result);

            globalInt.value = 123456;
            Assert.AreEqual(420, result);
        }

        [Test]
        public void SettingOldValueDoesntTriggerEvent()
        {
            var count = 0;
            scopedInt.onChangeValue.AddResponse(i => count++);

            scopedInt.value = 42;
            scopedInt.value = 42;

            Assert.AreEqual(1, count);
        }

        [Test]
        public void NullValues()
        {
            var globalString = ScriptableObject.CreateInstance<GlobalString>();
#if NEW_SCOPED_VARIABLES
            var scopedString = new ScopedVariable<string>("not null");
#else
            var scopedString = new ScopedString("not null");
#endif

            globalString.value = "totally not null";

            // With a local value
            Assert.DoesNotThrow(() =>
            {
                scopedString.value = null;
            });
            Assert.AreEqual(null, scopedString.value);

            Assert.DoesNotThrow(() =>
            {
                scopedString.value = "not null again";
            });
            Assert.AreEqual("not null again", scopedString.value);

            // As a GlobalVariable proxy
            scopedString.AssignGlobalVariable(globalString);
            Assert.DoesNotThrow(() =>
            {
                scopedString.value = null;
            });
            Assert.AreEqual(null, scopedString.value);

            Assert.DoesNotThrow(() =>
            {
                scopedString.value = "not null again";
            });
            Assert.AreEqual("not null again", scopedString.value);

            Object.DestroyImmediate(globalString);
        }

#if NEW_SCOPED_VARIABLES
        private class SerializationTest : ScriptableObject
        {
#pragma warning disable
            [SerializeField]
            private ScopedVariable<int> number = default;
#pragma warning restore
        }

        [Test]
        public void ScopedVariablesAreSerializable()
        {
            var scriptableObject = ScriptableObject.CreateInstance<SerializationTest>();

            try
            {
                var serializedObject = new UnityEditor.SerializedObject(scriptableObject);
                var property = serializedObject.FindProperty("number");

                Assert.IsNotNull(property);
            }
            finally
            {
                Object.DestroyImmediate(scriptableObject);
            }
        }
#endif
    }
}
