// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Editor class for GameEvents.
    /// </summary>
    [CustomEditor(typeof(GameEventBase), editorForChildClasses: true)]
    public class GameEventEditor : Editor
    {
        protected virtual string subtitle => "GameEvent" + (((GameEventBase)target).parameterType != null ? $" ({SodaEditorHelpers.ReplaceSubtitleTypeName(((GameEventBase)target).parameterType.Name)})" : "");


        protected virtual void OnEnable()
        {
            EditorApplication.update += () => Repaint();
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= () => Repaint();
        }

        public override void OnInspectorGUI()
        {
            /// ExcludeFromDocs

            SodaEditorHelpers.DisplayInspectorSubtitle(subtitle, target.GetType().Name);

            var gameEventBaseTarget = (GameEventBase)target;

            var descriptionProperty = serializedObject.FindProperty(GameEventBase.PropertyNames.description);
            var parameterDescriptionProperty = serializedObject.FindProperty(GameEventBase<AnyType>.PropertyNames.parameterDescription);
            var onRaiseGloballyProperty = serializedObject.FindProperty(GameEvent.PropertyNames.onRaiseGlobally);
            if (onRaiseGloballyProperty == null)
            {
                onRaiseGloballyProperty = serializedObject.FindProperty("_" + GameEvent.PropertyNames.onRaiseGlobally);
            }
            var debugProperty = serializedObject.FindProperty(GameEventBase.PropertyNames.debug);

            serializedObject.Update();

            if (onRaiseGloballyProperty != null)
            {
                if (parameterDescriptionProperty != null)
                {
                    serializedObject.DisplayAllPropertiesExcept(false,
                        descriptionProperty,
                        debugProperty,
                        onRaiseGloballyProperty,
                        parameterDescriptionProperty,
                        SodaEditorHelpers.DisplayAdditionalPropertiesLine,
                        SodaEditorHelpers.DisplayHorizontalLine);
                }
                else
                {
                    serializedObject.DisplayAllPropertiesExcept(false,
                        descriptionProperty,
                        debugProperty,
                        onRaiseGloballyProperty,
                        SodaEditorHelpers.DisplayAdditionalPropertiesLine,
                        SodaEditorHelpers.DisplayHorizontalLine);
                }
            }
            else
            {
                if (parameterDescriptionProperty != null)
                {
                    serializedObject.DisplayAllPropertiesExcept(false,
                        descriptionProperty,
                        debugProperty,
                        parameterDescriptionProperty,
                        SodaEditorHelpers.DisplayAdditionalPropertiesLine,
                        SodaEditorHelpers.DisplayHorizontalLine);
                }
                else
                {
                    serializedObject.DisplayAllPropertiesExcept(false,
                        descriptionProperty,
                        debugProperty,
                        SodaEditorHelpers.DisplayAdditionalPropertiesLine,
                        SodaEditorHelpers.DisplayHorizontalLine);
                }
            }

            EditorGUILayout.PropertyField(descriptionProperty);
            if (parameterDescriptionProperty != null)
            {
                EditorGUILayout.PropertyField(parameterDescriptionProperty,
                                              new GUIContent("Parameter Description (" + SodaEditorHelpers.ReplaceSubtitleTypeName(gameEventBaseTarget.parameterType.Name) + ")"));
            }
            GUILayout.Space(16);

            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(onRaiseGloballyProperty);
            GUI.enabled = true;

            DisplayDebugCheckbox(debugProperty);
            serializedObject.ApplyModifiedProperties();

            DisplayRaiseButton();

            DisplayListeners(gameEventBaseTarget);
        }

        private void DisplayDebugCheckbox(SerializedProperty property)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.Width(16));
            GUILayout.Label("Debug (Raises are logged into the console)");
            GUILayout.EndHorizontal();
        }

        private void DisplayRaiseButton()
        {
            var gameEventTarget = target as GameEvent;
            if (!gameEventTarget)
            {
                EditorGUILayout.HelpBox("Cannot raise a parameterized GameEvent via inspector.", MessageType.Info);
            }
            else
            {
                GUI.enabled = Application.isPlaying;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Raise", GUILayout.Height(32), GUILayout.MaxWidth(320)))
                {
                    gameEventTarget.Raise();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUI.enabled = true;
            }
        }

        private void DisplayListeners(GameEventBase gameEventBaseTarget)
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Responding Objects", EditorStyles.boldLabel);
                if (targets.Length == 1)
                {
                    SodaEventDrawer.DisplayListeners(gameEventBaseTarget.GetOnRaiseBase());
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot display when multiple GameEvents are selected.", MessageType.Warning);
                }
            }
        }
    }
}
