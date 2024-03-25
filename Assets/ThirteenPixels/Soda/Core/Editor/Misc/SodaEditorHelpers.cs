// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class SodaEditorHelpers
    {
        public struct GUIColor : IDisposable
        {
            private Color originalColor;

            public GUIColor(Color color)
            {
                originalColor = GUI.color;
                GUI.color = color;
            }

            public void Dispose()
            {
                GUI.color = originalColor;
            }
        }

        public const string MENU_ITEM_ROOT = "Tools/Soda/";

        #region Initialization of valueFieldActions
        private static Dictionary<Type, Func<Rect, GUIContent, object, object>> valueFieldActions;

        private static void InitializeValueFieldActions()
        {
            valueFieldActions = new Dictionary<Type, Func<Rect, GUIContent, object, object>>();

            valueFieldActions.Add(typeof(bool), (rect, label, obj) => EditorGUI.Toggle(rect, label, (bool)obj));
            valueFieldActions.Add(typeof(int), (rect, label, obj) => EditorGUI.IntField(rect, label, (int)obj));
            valueFieldActions.Add(typeof(long), (rect, label, obj) => EditorGUI.LongField(rect, label, (long)obj));
            valueFieldActions.Add(typeof(uint), (rect, label, obj) =>
            {
                var value = EditorGUI.LongField(rect, label, (uint)obj);
                if (value < uint.MinValue)
                {
                    value = uint.MinValue;
                }
                if (value > uint.MaxValue)
                {
                    value = uint.MaxValue;
                }
                return (uint)value;
            });
            valueFieldActions.Add(typeof(sbyte), (rect, label, obj) =>
            {
                return (sbyte)Mathf.Clamp(EditorGUI.LongField(rect, label, (sbyte)obj), sbyte.MinValue, sbyte.MaxValue);
            });
            valueFieldActions.Add(typeof(byte), (rect, label, obj) =>
            {
                return (byte)Mathf.Clamp(EditorGUI.LongField(rect, label, (byte)obj), byte.MinValue, byte.MaxValue);
            });
            valueFieldActions.Add(typeof(short), (rect, label, obj) =>
            {
                return (short)Mathf.Clamp(EditorGUI.LongField(rect, label, (short)obj), short.MinValue, short.MaxValue);
            });
            valueFieldActions.Add(typeof(ushort), (rect, label, obj) =>
            {
                return (ushort)Mathf.Clamp(EditorGUI.LongField(rect, label, (ushort)obj), ushort.MinValue, ushort.MaxValue);
            });
            valueFieldActions.Add(typeof(float), (rect, label, obj) => EditorGUI.FloatField(rect, label, (float)obj));
            valueFieldActions.Add(typeof(double), (rect, label, obj) => EditorGUI.DoubleField(rect, label, (double)obj));
            valueFieldActions.Add(typeof(string), (rect, label, obj) => EditorGUI.TextField(rect, label, (string)obj));
            valueFieldActions.Add(typeof(Vector2), (rect, label, obj) => EditorGUI.Vector2Field(rect, label, (Vector2)obj));
            valueFieldActions.Add(typeof(Vector3), (rect, label, obj) => EditorGUI.Vector3Field(rect, label, (Vector3)obj));
            valueFieldActions.Add(typeof(Vector4), (rect, label, obj) => EditorGUI.Vector4Field(rect, label, (Vector4)obj));
            valueFieldActions.Add(typeof(Vector2Int), (rect, label, obj) => EditorGUI.Vector2IntField(rect, label, (Vector2Int)obj));
            valueFieldActions.Add(typeof(Vector3Int), (rect, label, obj) => EditorGUI.Vector3IntField(rect, label, (Vector3Int)obj));
            valueFieldActions.Add(typeof(Color), (rect, label, obj) => EditorGUI.ColorField(rect, label, (Color)obj));
            valueFieldActions.Add(typeof(AnimationCurve), (rect, label, obj) => EditorGUI.CurveField(rect, label, (AnimationCurve)obj));
            valueFieldActions.Add(typeof(Bounds), (rect, label, obj) => EditorGUI.BoundsField(rect, label, (Bounds)obj));
            valueFieldActions.Add(typeof(Gradient), (rect, label, obj) => EditorGUI.GradientField(rect, label, (Gradient)obj));
            valueFieldActions.Add(typeof(Rect), (rect, label, obj) => EditorGUI.RectField(rect, label, (Rect)obj));
            // Add more when needed
        }
        #endregion

#if UNITY_2019_1_OR_NEWER
        private static GUIStyle inspectorSubtitleRightLabelStyle;
#endif
        private static GUIStyle _popupStyle;
        public static GUIStyle popupStyle
        {
            get
            {
                if (_popupStyle == null)
                {
                    _popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                    _popupStyle.imagePosition = ImagePosition.ImageOnly;
                }
                return _popupStyle;
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            InitializeValueFieldActions();
        }

        /// <summary>
        /// Displays a subtitle in the inspector. Used for ScriptableObjects.
        /// </summary>
        public static void DisplayInspectorSubtitle(string text, string rightSideText = null)
        {
#if UNITY_2019_1_OR_NEWER
            if (inspectorSubtitleRightLabelStyle == null)
            {
                inspectorSubtitleRightLabelStyle = new GUIStyle(GUI.skin.label);
                inspectorSubtitleRightLabelStyle.alignment = TextAnchor.MiddleRight;
            }

            GUILayout.Box(" ", GUILayout.ExpandWidth(true));
            var boxRect = GUILayoutUtility.GetLastRect();
            boxRect.xMin += 4;
            boxRect.xMax -= 4;
            GUI.Label(boxRect, text);

            if (!string.IsNullOrEmpty(rightSideText))
            {
                GUI.Label(boxRect, rightSideText, inspectorSubtitleRightLabelStyle);
            }
#else
            const int verticalPosition = 22;
            GUI.Label(new Rect(43, verticalPosition, 200, 32), text + " " + rightSideText, EditorStyles.miniLabel);
#endif
        }

        /// <summary>
        /// Displays a field for a multitude of possible object types.
        /// Comparable to EditorGUI.PropertyField, but without the need to supply a serialized property.
        /// </summary>
        /// <param name="position">The position in the UI.</param>
        /// <param name="label">The label to display in the UI.</param>
        /// <param name="value">The current value of the property.</param>
        /// <param name="type">The type of the property.</param>
        public static void DisplayValueField(Rect position, GUIContent label, ref object value, Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                value = EditorGUI.ObjectField(position, label, (UnityEngine.Object)value, type, true);
            }
            else if (type.IsEnum)
            {
                value = EditorGUI.EnumPopup(position, label, (Enum)value);
            }
            else
            {
                Func<Rect, GUIContent, object, object> func;
                if (valueFieldActions.TryGetValue(type, out func))
                {
                    value = func(position, label, value);
                }
                else
                {
                    EditorGUI.HelpBox(position, $"Cannot draw {type.Name} during play mode. Editing this value in the inspector is disabled.", MessageType.Error);
                }
            }
        }

        /// <summary>
        /// Displays a horizontal line meant as a divider between vertically stacked lines.
        /// </summary>
        public static void DisplayHorizontalLine()
        {
            DisplayHorizontalLine(null);
        }

        /// <summary>
        /// Displays a horizontal line meant as a divider between vertically stacked lines.
        /// Shows a label with the given text on the left side.
        /// </summary>
        public static void DisplayHorizontalLine(string label)
        {
            const int LINE_HEIGHT = 2;
            const int HEIGHT_WITH_LABEL = 28;
            const int HEIGHT_WITHOUT_LABEL = 10;

            var hasLabel = !string.IsNullOrEmpty(label);

            GUILayout.BeginHorizontal();
            if (hasLabel)
            {
                GUILayout.Label(label, EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));
                var rect = GUILayoutUtility.GetRect(0, HEIGHT_WITH_LABEL);
                rect.y += 15;
                rect.height = LINE_HEIGHT;
                EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.4f));
            }
            else
            {
                var rect = GUILayoutUtility.GetRect(0, HEIGHT_WITHOUT_LABEL);
                rect.y += 5;
                rect.height = LINE_HEIGHT;
                EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.4f));
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Displays a horizontal line with the label "Additional Properties".
        /// </summary>
        public static void DisplayAdditionalPropertiesLine()
        {
            DisplayHorizontalLine("Additional Properties");
        }

        public static int GetNumberOfPropertyChildren(SerializedProperty property)
        {
            property = property.Copy();
            return property.CountInProperty() - 1;
        }

        /// <summary>
        /// Creates a text file within the application's datapath.
        /// </summary>
        /// <param name="content">The content of the file.</param>
        /// <param name="filename">The name of the file.</param>
        /// <returns>The file, loaded as a UnityEngine.TextAsset.</returns>
        public static TextAsset CreateTextFile(string content, string filename)
        {
            var relativePath = TryGetActiveFolderPath();
            if (relativePath == null)
            {
                relativePath = "Assets/";
            }
            relativePath += filename;
            var fullPath = Directory.GetParent(Application.dataPath).FullName + "/" + relativePath;

            if (File.Exists(fullPath))
            {
                throw new IOException($"A file with the name {relativePath} already exists.");
            }

            try
            {
                File.WriteAllText(fullPath, content);
            }
            catch
            {
                throw new IOException("Could not write text file.");
            }

            AssetDatabase.Refresh();

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
            if (!textAsset)
            {
                throw new IOException("Could not write text file.");
            }

            return textAsset;
        }

        #region DisplayAllPropertiesExcept
        // The following methods are used to display all properties of a serialized object, but excluding some because they are already explicitly drawn somewhere else.
        // This allows the user to create classes that inherit from Soda ScriptableObject classes and adding new properties to them that will also be drawn.
        // The params keyword is not used to avoid repeated allocation of an array.

        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator,
            SerializedProperty property,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            iterator.DisplayAllPropertiesFiltered(prop =>
                prop.propertyPath != property.propertyPath,
                beforeFirstFieldCallback,
                afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator,
            SerializedProperty propertyA,
            SerializedProperty propertyB,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            iterator.DisplayAllPropertiesFiltered(prop =>
                prop.propertyPath != propertyA.propertyPath &&
                prop.propertyPath != propertyB.propertyPath,
                beforeFirstFieldCallback,
                afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator,
            SerializedProperty propertyA,
            SerializedProperty propertyB,
            SerializedProperty propertyC,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            iterator.DisplayAllPropertiesFiltered(prop =>
                prop.propertyPath != propertyA.propertyPath &&
                prop.propertyPath != propertyB.propertyPath &&
                prop.propertyPath != propertyC.propertyPath,
                beforeFirstFieldCallback,
                afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesExcept(this SerializedProperty iterator,
            SerializedProperty propertyA,
            SerializedProperty propertyB,
            SerializedProperty propertyC,
            SerializedProperty propertyD,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            iterator.DisplayAllPropertiesFiltered(prop =>
                prop.propertyPath != propertyA.propertyPath &&
                prop.propertyPath != propertyB.propertyPath &&
                prop.propertyPath != propertyC.propertyPath &&
                prop.propertyPath != propertyD.propertyPath,
                beforeFirstFieldCallback,
                afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesFiltered(this SerializedProperty iterator,
            Predicate<SerializedProperty> filter,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            var didShowField = false;

            while (iterator.NextVisible(false))
            {
                if (filter(iterator))
                {
                    if (!didShowField)
                    {
                        beforeFirstFieldCallback?.Invoke();
                    }

                    EditorGUILayout.PropertyField(iterator);

                    didShowField = true;
                }
            }

            if (didShowField && afterLastFieldCallback != null)
            {
                afterLastFieldCallback();
            }
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject,
            bool showScriptField,
            SerializedProperty property,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(property, beforeFirstFieldCallback, afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject,
            bool showScriptField,
            SerializedProperty propertyA,
            SerializedProperty propertyB,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(propertyA, propertyB, beforeFirstFieldCallback, afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject,
            bool showScriptField, SerializedProperty propertyA,
            SerializedProperty propertyB,
            SerializedProperty propertyC,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(propertyA, propertyB, propertyC, beforeFirstFieldCallback, afterLastFieldCallback);
        }

        public static void DisplayAllPropertiesExcept(this SerializedObject serializedObject,
            bool showScriptField, SerializedProperty propertyA,
            SerializedProperty propertyB,
            SerializedProperty propertyC,
            SerializedProperty propertyD,
            Action beforeFirstFieldCallback = null,
            Action afterLastFieldCallback = null)
        {
            var iterator = serializedObject.GetIterator();
            if (!showScriptField)
            {
                iterator.NextVisible(true);
            }
            iterator.DisplayAllPropertiesExcept(propertyA, propertyB, propertyC, propertyD, beforeFirstFieldCallback, afterLastFieldCallback);
        }
        #endregion

        public static string ReplaceSubtitleTypeName(string name)
        {
            switch (name)
            {
                case "Single": return "float";
                case "Int32": return "int";
                default: return name;
            }
        }

        #region Find Editor Windows
        public static EditorWindow FindFirstWindowWithTypeName(string name)
        {
            var allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            return allWindows.Where(window => window.GetType().Name == name).FirstOrDefault();
        }

        public static EditorWindow FindHierarchyWindow()
        {
            return FindFirstWindowWithTypeName("SceneHierarchyWindow");
        }

        /// <summary>
        /// Returns the project-relative path (Assets/...) that's currently open in the project view.
        /// Returns if it cannot be found.
        /// </summary>
        public static string TryGetActiveFolderPath()
        {
            var tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            object[] args = new object[] { null };
            bool found = (bool)tryGetActiveFolderPath.Invoke(null, args);
            if (found)
            {
                return (string)args[0] + "/";
            }
            return null;
        }
        #endregion
    }
}
