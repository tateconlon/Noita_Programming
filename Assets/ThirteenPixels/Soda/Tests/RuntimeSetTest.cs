// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Tests
{
    using UnityEngine;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class RuntimeSetTest
    {
        private class RuntimeSetVisibleObjects : RuntimeSetBase<RuntimeSetVisibleObjects.Element>
        {
            public struct Element
            {
                public readonly GameObject gameObject;
                public readonly MeshRenderer renderer;
                public readonly MeshFilter filter;

                public Element(GameObject gameObject, MeshRenderer renderer, MeshFilter filter)
                {
                    this.gameObject = gameObject;
                    this.renderer = renderer;
                    this.filter = filter;
                }
            }

            // We use the default reflection-based implementation here for testing.
            /*
            protected override bool TryCreateElement(GameObject gameObject, out Element element)
            {
                element = new Element(gameObject.GetComponent<MeshRenderer>(),
                                      gameObject.GetComponent<MeshFilter>());
                return element.renderer && element.filter;
            }
            */
        }

        private RuntimeSetGameObject runtimeSetGameObject;
        private RuntimeSetVisibleObjects runtimeSetVisibleObjects;
        private List<GameObject> objects;

        [SetUp]
        public void SetUp()
        {
            runtimeSetGameObject = ScriptableObject.CreateInstance<RuntimeSetGameObject>();
            runtimeSetVisibleObjects = ScriptableObject.CreateInstance<RuntimeSetVisibleObjects>();

            objects = new List<GameObject>();
            objects.Add(new GameObject("Test GameObject 1"));
            objects.Add(new GameObject("Test GameObject 2"));
            objects.Add(new GameObject("Test GameObject 3"));

            foreach (var obj in objects)
            {
                runtimeSetGameObject.Add(obj);
            }
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(runtimeSetGameObject);
            Object.DestroyImmediate(runtimeSetVisibleObjects);

            foreach (var obj in objects)
            {
                Object.DestroyImmediate(obj);
            }
            objects.Clear();
        }

        [Test]
        public void AddCorrectGameObjects()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();
                
                var success = runtimeSetVisibleObjects.Add(obj);
                Assert.IsTrue(success);
            }

            Assert.AreEqual(3, runtimeSetVisibleObjects.elementCount);

            var bits = 0;
            runtimeSetVisibleObjects.ForEach(item =>
            {
                var index = objects.IndexOf(item.gameObject);
                Assert.GreaterOrEqual(index, 0);
                bits |= 1 << index;
            });
            Assert.AreEqual(7, bits);
        }

        [Test]
        public void AddWrongGameObjects()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();

                var success = runtimeSetVisibleObjects.Add(obj);
                Assert.IsFalse(success);
            }

            Assert.AreEqual(0, runtimeSetVisibleObjects.elementCount);
        }

        [Test]
        public void RemoveGameObjects()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSetVisibleObjects.Add(obj);
            }

            var count = 3;
            foreach (var obj in objects)
            {
                runtimeSetVisibleObjects.Remove(obj);
                count--;
                Assert.AreEqual(count, runtimeSetVisibleObjects.elementCount);
            }
        }

        [Test]
        public void OnElementCountChangeEvent()
        {
            var count = 0;
            runtimeSetVisibleObjects.onElementCountChange.AddResponse(newCount => count = newCount);


            var i = 0;
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSetVisibleObjects.Add(obj);
                i++;
                Assert.AreEqual(i, count);
            }

            foreach (var obj in objects)
            {
                runtimeSetVisibleObjects.Remove(obj);
                i--;
                Assert.AreEqual(i, count);
            }
        }

        [Test]
        public void CanIterate()
        {
            var index = 1;

            foreach (var item in runtimeSetGameObject)
            {
                Assert.AreEqual("Test GameObject " + index, item.name);
                index++;
            }

            Assert.AreEqual(4, index);
        }

        [Test]
        public void CanModifyDuringForEach()
        {
            Assert.AreEqual(3, runtimeSetGameObject.elementCount);

            GameObject objectTakenOut = null;

            runtimeSetGameObject.ForEach(item =>
            {
                if (item.name == "Test GameObject 1")
                {
                    runtimeSetGameObject.Remove(item);
                    objectTakenOut = item;
                }
            });

            Assert.AreEqual(2, runtimeSetGameObject.elementCount);
            Assert.IsNotNull(objectTakenOut);

            runtimeSetGameObject.ForEach(item =>
            {
                if (item.name == "Test GameObject 2")
                {
                    runtimeSetGameObject.Add(objectTakenOut);
                }
            });

            Assert.AreEqual(3, runtimeSetGameObject.elementCount);
        }

        [Test]
        public void FirstElementMatchesFirstGameObject()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSetVisibleObjects.Add(obj);
            }

            Assert.AreSame(runtimeSetVisibleObjects.GetFirstOrDefault(), runtimeSetVisibleObjects.GetFirstOrDefaultElement().gameObject);
        }

        [Test]
        public void LastlementMatchesLastGameObject()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSetVisibleObjects.Add(obj);
            }

            Assert.AreSame(runtimeSetVisibleObjects.GetLastOrDefault(), runtimeSetVisibleObjects.GetLastOrDefaultElement().gameObject);
        }

        [Test]
        public void IterationMatchesIndex()
        {
            foreach (var obj in objects)
            {
                obj.AddComponent<MeshRenderer>();
                obj.AddComponent<MeshFilter>();

                runtimeSetVisibleObjects.Add(obj);
            }

            {
                var index = 0;
                foreach (var element in runtimeSetVisibleObjects)
                {
                    Assert.AreEqual(element, runtimeSetVisibleObjects[index]);
                    index++;
                }
            }

            {
                var index = 0;
                foreach (var gameObject in objects)
                {
                    Assert.AreSame(gameObject, objects[index]);
                    index++;
                }
            }
        }
    }
}
