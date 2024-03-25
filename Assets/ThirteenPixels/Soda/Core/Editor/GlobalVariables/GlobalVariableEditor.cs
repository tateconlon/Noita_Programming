// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Editor class for GlobalVariables.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GlobalVariableBase), editorForChildClasses: true)]
    public class GlobalVariableEditor : Editor
    {
        protected virtual string subtitle => $"Global Variable ({SodaEditorHelpers.ReplaceSubtitleTypeName(((GlobalVariableBase)target).valueType.Name)})";


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

            var originalValueProperty = serializedObject.FindProperty(GlobalVariableBase<AnyType>.PropertyNames.originalValue);
            var descriptionProperty = serializedObject.FindProperty(GlobalVariableBase.PropertyNames.description);

            serializedObject.Update();

            if (originalValueProperty != null)
            {
                EditorGUILayout.PropertyField(originalValueProperty);
                serializedObject.DisplayAllPropertiesExcept(false,
                    originalValueProperty,
                    descriptionProperty,
                    SodaEditorHelpers.DisplayAdditionalPropertiesLine,
                    SodaEditorHelpers.DisplayHorizontalLine);
            }
            else
            {
                EditorGUILayout.HelpBox("The type of the value this GlobalVariable is representing does not seem to be serializable. It cannot be displayed or edited in the inspector.", MessageType.Info);
                serializedObject.DisplayAllPropertiesExcept(false,
                    descriptionProperty,
                    SodaEditorHelpers.DisplayAdditionalPropertiesLine,
                    SodaEditorHelpers.DisplayHorizontalLine);
            }

            EditorGUILayout.PropertyField(descriptionProperty);

            SetTargetDeserialization(false);
            serializedObject.ApplyModifiedProperties();
            SetTargetDeserialization(true);

            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Objects responding to onChange event");
                if (targets.Length == 1)
                {
                    var globalVariableTarget = (GlobalVariableBase)target;
                    SodaEventDrawer.DisplayListeners(globalVariableTarget.GetOnChangeEvent());
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot display when multiple GlobalVariables are selected.", MessageType.Warning);
                }
            }
        }

        /// <summary>
        /// Enables or disables the targets' OnAfterDeserialize behavior.
        /// This is used to prevent ApplyModifiedProperties from triggering it.
        /// Otherwise, changing the description or another serialized field during play mode would reset the value to the original serialized value.
        /// </summary>
        private void SetTargetDeserialization(bool deserialize)
        {
            foreach (var target in serializedObject.targetObjects)
            {
                ((GlobalVariableBase)target).shouldDoPostDeserialization = deserialize;
            }
        }
    }
}
