// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Draws a property or field as a replacement for the serialized field tagged with the <see cref="DisplayInsteadInPlaymodeAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisplayInsteadInPlaymodeAttribute))]
    public class DisplayInsteadInPlaymodeDrawer : PropertyDrawer
    {
#if UNITY_2022_2_OR_NEWER
        private const int SAVE_BUTTON_WIDTH = 20;
        private const int SAVE_BUTTON_PADDING = 3;
#endif
        private const string TOOLTIP = "Changes to this value are not being saved when exiting play mode.";
        private static readonly Regex LABEL_FORMATTING_REGEX = new Regex("((?<=[a-z])[A-Z0-9])|_");

        private static Texture2D playmodeIcon;
        private static Texture2D playmodeIconOff;
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            playmodeIcon = Resources.Load<Texture2D>("Icon-Playmode-On");
            playmodeIconOff = Resources.Load<Texture2D>("Icon-Playmode-Off");
        }

        private PropertyInfo replacementProperty;
        private FieldInfo replacementField;
        private string formattedInspectorPropertyName;

        private void InitializeIfNeeded(object target)
        {
            var diipAttribute = (DisplayInsteadInPlaymodeAttribute)attribute;

            if (diipAttribute.replacementName != null && replacementProperty == null)
            {
                var bindingFlags = BindingFlags.SetProperty |
                    BindingFlags.GetProperty |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic;
                replacementProperty = target.GetType().GetProperty(diipAttribute.replacementName, bindingFlags);

                if (replacementProperty != null)
                {
                    formattedInspectorPropertyName = FormatUnityInspectorPropertyLabel(replacementProperty.Name);
                }
                else
                {
                    replacementField = target.GetType().GetField(diipAttribute.replacementName, bindingFlags);

                    if (replacementField != null)
                    {
                        formattedInspectorPropertyName = FormatUnityInspectorPropertyLabel(replacementField.Name);
                    }
                    else
                    {
                        Debug.LogError("Could not find replacement property or field to draw.");
                    }
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /// ExcludeFromDocs

            var serializedObject = property.serializedObject;
            var target = serializedObject.targetObject;

            var diipAttribute = (DisplayInsteadInPlaymodeAttribute)attribute;

            InitializeIfNeeded(target);

            if (Application.isPlaying)
            {
                if (replacementProperty != null || replacementField != null)
                {
                    property.isExpanded = false;

                    var playmodeLabel = new GUIContent(label);

                    var targetIsScriptableObject = target is ScriptableObject;

                    if (targetIsScriptableObject)
                    {
                        if (label != GUIContent.none)
                        {
                            playmodeLabel.image = playmodeIcon;
                            // Tooltips are, by design, not displayed in play mode.
                            /*
                             * var originalTooltip = !string.IsNullOrEmpty(label.tooltip) ? label.tooltip + "\n\n" : "";
                             * playmodeLabel.tooltip = originalTooltip + tooltip;
                             */
                        }

#if UNITY_2022_2_OR_NEWER
                        ResizePositionForSaveValueButton(ref position);
#endif
                    }

                    if (label != GUIContent.none)
                    {
                        playmodeLabel.text = formattedInspectorPropertyName;
                    }

                    object value;
                    if (replacementProperty != null)
                    {
                        value = replacementProperty.GetValue(target, null);
                        SodaEditorHelpers.DisplayValueField(position, playmodeLabel, ref value, replacementProperty.PropertyType);
                        replacementProperty.SetValue(target, value, null);
                    }
                    else
                    {
                        value = replacementField.GetValue(target);
                        SodaEditorHelpers.DisplayValueField(position, playmodeLabel, ref value, replacementField.FieldType);
                        replacementField.SetValue(target, value);
                    }

#if UNITY_2022_2_OR_NEWER
                    if (targetIsScriptableObject)
                    {
                        DrawSaveValueButton(position, () =>
                        {
                            property.boxedValue = value;
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
#endif
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label, true);
                    GUI.enabled = true;
                }
            }
            else
            {
                var editmodeLabel = new GUIContent(label);
                if (target is ScriptableObject && label != GUIContent.none)
                {
                    editmodeLabel.text = formattedInspectorPropertyName;
                    editmodeLabel.image = playmodeIconOff;
                    var attributeTooltip = !string.IsNullOrEmpty(diipAttribute.tooltip) ? diipAttribute.tooltip + "\n\n" : string.Empty;
                    editmodeLabel.tooltip = attributeTooltip + TOOLTIP;
                }
                EditorGUI.PropertyField(position, property, editmodeLabel, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }

#if UNITY_2022_2_OR_NEWER
        private void ResizePositionForSaveValueButton(ref Rect position)
        {
            position.width -= SAVE_BUTTON_WIDTH + SAVE_BUTTON_PADDING;
        }

        private void DrawSaveValueButton(Rect position, GenericMenu.MenuFunction onClick)
        {
            var buttonRect = new Rect(position.xMax + SAVE_BUTTON_PADDING, position.y, SAVE_BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(buttonRect, string.Empty, SodaEditorHelpers.popupStyle))
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Save value"), false, onClick);

                menu.ShowAsContext();
            }
        }
#endif

        private static string FormatUnityInspectorPropertyLabel(string propertyName)
        {
            var result = LABEL_FORMATTING_REGEX.Replace(propertyName, match => " " + (match.Value == "_" ? string.Empty : match.Value));
            result = result.Trim();
            result = result[0].ToString().ToUpper() + result.Substring(1);
            return result;
        }
    }
}
