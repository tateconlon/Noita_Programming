using UnityEngine;
using UnityEngine.EventSystems;

public class NodeDrag : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private UiNodeBase _skillNode;

    private void Awake()
    {
        _skillNode = GetComponentInParent<UiNodeBase>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _skillNode.transform.localPosition = _skillNode.graph.scrollRect.content.InverseTransformPoint(eventData.position) - offset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 pointer = _skillNode.graph.scrollRect.content.InverseTransformPoint(eventData.position);
        Vector2 pos = _skillNode.transform.localPosition;
        offset = pointer - pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _skillNode.transform.localPosition = _skillNode.graph.scrollRect.content.InverseTransformPoint(eventData.position) - offset;
        Vector2 pos = _skillNode.transform.localPosition;
        pos.y = -pos.y;
        _skillNode.node.position = pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        _skillNode.graph.nodeContextMenu.selectedNode = _skillNode.node;
        _skillNode.graph.nodeContextMenu.OpenAt(eventData.position);
    }
}