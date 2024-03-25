// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(GlobalGameObjectRegister))]
    [CanEditMultipleObjects]
    public class GlobalGameObjectRegisterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(GlobalGameObjectRegister.PropertyNames.globalGameObject));
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

            var component = (GlobalGameObjectRegister)target;

            if (component.globalGameObject != null)
            {
                var doesMatch = component.globalGameObject.Allows(component.gameObject);
                if (doesMatch)
                {
                    // EditorGUILayout.HelpBox("This GameObject can be added to the GlobalGameObject.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("This GameObject does not match the GlobalGameObject's component cache type.", MessageType.Error);
                }
            }
        }
    }
}
