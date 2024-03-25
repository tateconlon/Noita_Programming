// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using Object = UnityEngine.Object;

    public static class SodaDragAndDrop
    {
        private const float MIN_DRAG_PIXELS = 7.2f;
        private static Object dragObject;
        private static Vector2 clickPosition;

        public static void MakeDragSource(Rect rect, Object objectToDrag)
        {
            var currentEvent = Event.current;
            if (rect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    clickPosition = currentEvent.mousePosition;
                    dragObject = objectToDrag;
                    currentEvent.Use();
                }
                else if (dragObject == objectToDrag &&
                         currentEvent.type == EventType.MouseDrag &&
                         Vector2.Distance(clickPosition, currentEvent.mousePosition) >= MIN_DRAG_PIXELS)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new Object[] { objectToDrag };
                    // PrepareStartDrag is supposed to clear paths, but it didn't really.
                    DragAndDrop.paths = System.Array.Empty<string>();
                    DragAndDrop.StartDrag(objectToDrag.name);
                    currentEvent.Use();
                }
            }
        }

        public static void DrawDragButton(Rect rect, GUIContent content, Object objectToDrag)
        {
            GUI.Box(rect, content, EditorStyles.toolbarButton);
            MakeDragSource(rect, objectToDrag);
        }

        public static void DrawDragButtonLayout(GUIContent content, Object objectToDrag, params GUILayoutOption[] options)
        {
            var rect = GUILayoutUtility.GetRect(content, GUIStyle.none, options);
            DrawDragButton(rect, content, objectToDrag);
        }

        public static void MakeDragDestination(Rect rect, Func<Object, bool> predicate, Action<Object> callback)
        {
            var currentEvent = Event.current;

            if (rect.Contains(currentEvent.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length == 1 && predicate(DragAndDrop.objectReferences[0]))
                {
                    switch (currentEvent.type)
                    {
                        case EventType.DragUpdated:
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                            break;
                        case EventType.DragPerform:
                            try
                            {
                                callback(DragAndDrop.objectReferences[0]);
                            }
                            finally
                            {
                                DragAndDrop.AcceptDrag();
                            }
                            break;
                    }
                }
            }
        }

        public static void MakePathDragDestination(Rect rect, Func<string, bool> predicate, Action<string> callback)
        {
            var currentEvent = Event.current;

            if (rect.Contains(currentEvent.mousePosition))
            {
                if (DragAndDrop.paths.Length == 1 && predicate(DragAndDrop.paths[0]))
                {
                    switch (currentEvent.type)
                    {
                        case EventType.DragUpdated:
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                            break;
                        case EventType.DragPerform:
                            try
                            {
                                callback(DragAndDrop.paths[0]);
                            }
                            finally
                            {
                                DragAndDrop.AcceptDrag();
                            }
                            break;
                    }
                }
            }
        }
    }
}
