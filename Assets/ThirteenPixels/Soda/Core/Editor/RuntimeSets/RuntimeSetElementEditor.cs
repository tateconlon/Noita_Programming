// Copyright © Sascha Graeff/13Pixels.

using UnityEngine;

namespace ThirteenPixels.Soda.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(RuntimeSetElement))]
    [CanEditMultipleObjects]
    public class RuntimeSetElementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            bool stashedEnable = GUI.enabled;
            
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(RuntimeSetElement.PropertyNames.AddWhileDisabled));
            GUI.enabled = stashedEnable;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(RuntimeSetElement.PropertyNames.runtimeSet));
            serializedObject.ApplyModifiedProperties();

            DisplaySetMatchingInformation();
        }

        private void DisplaySetMatchingInformation()
        {
            if (targets.Length > 1)
            {
                EditorGUILayout.HelpBox("Multiple GameObjects cannot be validated at the same time.", MessageType.Warning);
                return;
            }

            var component = (RuntimeSetElement)target;

            if (component.runtimeSet)
            {
                var doesMatch = component.runtimeSet.Allows(component.gameObject);
                if (doesMatch)
                {

                    // EditorGUILayout.HelpBox("This GameObject can be added to the RuntimeSet.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("This GameObject does not match the RuntimeSet's element type.", MessageType.Error);
                }
            }
        }
    }
}
