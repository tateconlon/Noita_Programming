// Copyright © Sascha Graeff/13Pixels.
#if UNITY_2019_3_OR_NEWER
#define UNITY_NEW_SKIN
#endif

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// PropertyDrawer for serialized fields for GlobalVariable types.
    /// </summary>
    [CustomPropertyDrawer(typeof(GlobalVariableBase), true)]
    public class GlobalVariablePropertyDrawer : PropertyDrawer
    {
        private SerializedObject serializedTargetObject;
        private static GUIStyle expandButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft) { alignment = TextAnchor.MiddleRight };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /// ExcludeFromDocs
            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            var originalIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            const int foldoutWidth = 17;
            var actualFoldoutWidth = foldoutWidth;
#if UNITY_NEW_SKIN
            // Make the foldout button slightly wider so it goes under the (now round) property field.
            actualFoldoutWidth += 2;
#endif
            var foldoutRect = new Rect(position.x,
                                       position.y,
                                       actualFoldoutWidth,
                                       EditorGUIUtility.singleLineHeight);
            var referenceRect = new Rect(position.x + foldoutWidth,
                                         position.y,
                                         position.width - foldoutWidth,
                                         EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(position.x,
                                     position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                                     position.width - 18,
                                     EditorGUIUtility.singleLineHeight);

            var targetObject = property.objectReferenceValue;

            if (targetObject)
            {
                var foldoutLabel = property.isExpanded ? "-" : "+";
#if UNITY_NEW_SKIN
                foldoutLabel += " ";
#endif
                if (GUI.Button(foldoutRect, foldoutLabel, expandButtonStyle))
                {
                    property.isExpanded = !property.isExpanded;
                }

                EditorGUI.PropertyField(referenceRect, property, GUIContent.none);

                if (property.isExpanded)
                {
                    if (serializedTargetObject == null || serializedTargetObject.targetObject != targetObject)
                    {
                        serializedTargetObject = new SerializedObject(targetObject);
                    }

                    var valueProperty = serializedTargetObject.FindProperty(GlobalVariableBase<AnyType>.PropertyNames.originalValue);
                    var hasSerializableValue = valueProperty != null;

                    if (hasSerializableValue)
                    {
                        DrawFoldoutValueField(valueRect, valueProperty);
                    }
                    else
                    {
                        EditorGUI.HelpBox(valueRect, "Non-serializable value.", MessageType.None);
                    }
                }
            }
            else
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property, GUIContent.none);
                property.isExpanded = false;
            }

            EditorGUI.indentLevel = originalIndentLevel;

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private void DrawFoldoutValueField(Rect valueRect, SerializedProperty valueProperty)
        {
            serializedTargetObject.Update();

            const int prefixLabelWidth = 50;
            var prefixLabelRect = valueRect;
            prefixLabelRect.width = prefixLabelWidth;
            EditorGUI.PrefixLabel(prefixLabelRect, new GUIContent("Value"));

            valueRect.xMin += prefixLabelWidth;

            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

            serializedTargetObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}
