using UnityEngine;
using UnityEngine.EventSystems;

// https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IDragHandler.html?q=idraghandler

public class SkillIconDraggableSpawner : MonoBehaviour
{
    [SerializeField] private SkillIconDraggable _draggablePrefab;
    [SerializeField] private RectTransform _draggingPlane;

    private void OnEnable()
    {
        SkillIconUiEventHandler.OnBeginDragNode += OnDragNodeBegin;
    }

    private void OnDragNodeBegin(SkillNode draggedNode, PointerEventData eventData)
    {
        SkillIconDraggable draggable = Instantiate(_draggablePrefab, _draggingPlane);
        draggable.Init(_draggingPlane, draggedNode, eventData);
        
        // Immediately cancel the drag if node is empty or not movable
        if (draggedNode.IsEmpty || !draggedNode.canBeMoved)
        {
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
        }
    }
    
    private void OnDisable()
    {
        SkillIconUiEventHandler.OnBeginDragNode -= OnDragNodeBegin;
    }
}
