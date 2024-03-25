// Copyright © Sascha Graeff/13Pixels.
#if UNITY_2019_3_OR_NEWER
#define UNITY_NEW_SKIN
#endif

namespace ThirteenPixels.Soda.Editor
{
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ScopedVariableBase), true)]
    public class ScopedVariablePropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent[] options = new GUIContent[] { new GUIContent("Global Variable"), new GUIContent("Local Value") };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /// ExcludeFromDocs

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var modeRectY = position.y;
#if UNITY_NEW_SKIN
            modeRectY += 1;
            var valueRectOffset = 17;
#else
            modeRectY += 4;
            var valueRectOffset = 22;
#endif
            var modeRect = new Rect(position.x + 1, modeRectY, 20, EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(position.x + valueRectOffset, position.y, position.width - valueRectOffset, EditorGUIUtility.singleLineHeight);

            var useLocalChanged = DrawUseLocalSwitch(property, modeRect, out var useLocal);
            bool valueChanged;

            if (useLocal)
            {
                valueChanged = DrawLocalValueField(property, valueRect);
            }
            else
            {
                valueChanged = DrawReferenceProperty(property, valueRect);
            }

            if (useLocalChanged || valueChanged)
            {
                property.serializedObject.ApplyModifiedProperties();

                if (Application.isPlaying)
                {
                    UpdateObject(property);
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Draws the UI for switching between local value and referenced GlobalVariable.
        /// </summary>
        /// <param name="useLocal">The state of the property (true if a local value is used) after drawing this.</param>
        /// <returns>true if the property has changed.</returns>
        private static bool DrawUseLocalSwitch(SerializedProperty property, Rect rect, out bool useLocal)
        {
            EditorGUI.BeginChangeCheck();

            var useLocalProperty = property.FindPropertyRelative("useLocal");
            useLocal = useLocalProperty.boolValue;
            useLocal = EditorGUI.Popup(rect, GUIContent.none, useLocal ? 1 : 0, options, SodaEditorHelpers.popupStyle) == 1;
            useLocalProperty.boolValue = useLocal;

            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// Draws the property field for changing the local value.
        /// </summary>
        /// <returns>true if the local value has changed.</returns>
        private static bool DrawLocalValueField(SerializedProperty property, Rect rect)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 100;
            var localValueProperty = property.FindPropertyRelative("localValue");
            if (localValueProperty != null)
            {
                if (localValueProperty.hasChildren)
                {
                    rect.xMin += 10;
                }
                EditorGUI.PropertyField(rect, localValueProperty, new GUIContent("Value"), true);
            }
            else
            {
                EditorGUI.HelpBox(rect, "Non-serializable value.", MessageType.None);
            }
            EditorGUIUtility.labelWidth = 0;

            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// Draws the property field for picking a GlobalVariable.
        /// </summary>
        /// <returns>true if the reference changed, but NOT if the referenced GlobalVariable's value changed through the GlobalVariable property drawer.</returns>
        private static bool DrawReferenceProperty(SerializedProperty property, Rect rect)
        {
            var globalVariableProperty = property.FindPropertyRelative("globalVariable");

            var previousValue = globalVariableProperty.objectReferenceValue;
            EditorGUI.PropertyField(rect, globalVariableProperty, GUIContent.none);
            var newValue = globalVariableProperty.objectReferenceValue;

            return newValue != previousValue;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineCount = 1;

            var useLocalProperty = property.FindPropertyRelative("useLocal");
            var useLocal = useLocalProperty.boolValue;
            if (useLocal)
            {
                var localValueProperty = property.FindPropertyRelative("localValue");
                if (localValueProperty != null && localValueProperty.isExpanded)
                {
                    lineCount = SodaEditorHelpers.GetNumberOfPropertyChildren(localValueProperty) + 1;
                }
            }
            else
            {
                var glovalVariableProperty = property.FindPropertyRelative("globalVariable");
                if (glovalVariableProperty.objectReferenceValue && glovalVariableProperty.isExpanded)
                {
                    lineCount = 2;
                }
            }

            return lineCount * EditorGUIUtility.singleLineHeight + 
                (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        /// <summary>
        /// Updates the runtime object, including a proper update of the monitoring state and a notification for all objects monitoring it.
        /// </summary>
        private static void UpdateObject(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetObjectType = targetObject.GetType();

            FieldInfo field = FindFieldInTypeHierarchy(property.propertyPath, ref targetObjectType);

            // Happens once when changing a thing at runtime.
            // But the method will be called again, so it works.
            if (field == null) return;

            var targetScopedVariable = (ScopedVariableBase)field.GetValue(targetObject);
            targetScopedVariable.InvokeOnChangeEvent();
            targetScopedVariable.UpdateGlobalVariableMonitoring();
        }

        /// <summary>
        /// Finds the field with the given name in the given type, or any of its supertypes, up until UnityEngine.Object.
        /// </summary>
        /// <param name="fieldName">The name of the field to search for.</param>
        /// <param name="type">The type to start searching at.</param>
        /// <returns>The FieldInfo with the given name, null if none was found.</returns>
        private static FieldInfo FindFieldInTypeHierarchy(string fieldName, ref System.Type type)
        {
            FieldInfo field;
            do
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
            while (field == null && type.BaseType != null && (type = type.BaseType) != typeof(Object));

            return field;
        }
    }
}
