using UnityEngine;
using UnityEngine.EventSystems;

// https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IDragHandler.html?q=idraghandler

[RequireComponent(typeof(SkillIcon))]
public class SkillIconDraggable : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private SkillIcon _skillIcon;
    
    private RectTransform _draggingPlane;

    public void Init(RectTransform draggingPlane, SkillNode draggedNode, PointerEventData eventData)
    {
        _skillIcon.Bind(draggedNode);
        
        _draggingPlane = draggingPlane;
        transform.SetAsLastSibling();  // For layering so that the draggable icon displays above other UI
        SetDraggedPosition(eventData);
        
        // Swap the current drag to instead be on this GameObject: http://answers.unity.com/answers/943274/view.html
        ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
        eventData.pointerDrag = gameObject;
        ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.beginDragHandler);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
    
    private void SetDraggedPosition(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_draggingPlane, eventData.position, 
                eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            transform.position = globalMousePos;
            transform.rotation = _draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerDrag != gameObject) return;  // Ignore end drag events where this is not being released
        
        foreach (GameObject hoveredGameObject in eventData.hovered)  // hovered objects are ordered front-back for us
        {
            if (hoveredGameObject.TryGetComponent(out IReleasedNodeReceiver releasedNodeReceiver))
            {
                releasedNodeReceiver.ReceiveReleasedNode(_skillIcon.BoundTarget);
                break;
            }
        }
        
        Destroy(gameObject);
    }
    
    private void Reset()
    {
        _skillIcon = GetComponent<SkillIcon>();
    }
}

public interface IReleasedNodeReceiver
{
    public void ReceiveReleasedNode(SkillNode releasedNode);
}