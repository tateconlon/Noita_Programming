// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Editor class for the GameEventListener component.
    /// </summary>
    [CustomEditor(typeof(GameEventListener))]
    public class GameEventListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            /// ExcludeFromDocs

            serializedObject.Update();
            var listener = (GameEventListener)target;

            GUILayout.Space(6);

            var gameEventProperty = serializedObject.FindProperty(GameEventListener.PropertyNames.gameEvent);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gameEventProperty, new GUIContent("Game Event"));
            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                listener.gameEvent = (GameEvent)gameEventProperty.objectReferenceValue;
            }

            bool stashedEnable = GUI.enabled;
            
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(GameEventListener.PropertyNames.listenWhileDisabled));
            GUI.enabled = stashedEnable;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(GameEventListener.PropertyNames.deactivateAfterRaise));

            EditorGUILayout.PropertyField(serializedObject.FindProperty(GameEventListener.PropertyNames.response));

            var buttonRect = EditorGUILayout.GetControlRect(GUILayout.Height(0), GUILayout.Width(120));
            buttonRect.height = 14;
            buttonRect.y -= 20;
            buttonRect.x += 1;

            GUI.enabled = Application.isPlaying;
            if (GUI.Button(buttonRect, "Invoke Response", EditorStyles.miniButton))
            {
                listener.OnEventRaised();
            }
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
