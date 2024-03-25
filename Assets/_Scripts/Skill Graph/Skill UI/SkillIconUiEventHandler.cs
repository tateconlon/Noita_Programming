using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SkillIcon))]
public class SkillIconUiEventHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler
{
    [SerializeField] private SkillIcon _skillIcon;

    public static event ClickDelegate OnClickNode;
    public delegate void ClickDelegate(SkillNode clickedNode, PointerEventData eventData);
    
    public static event BeginDragDelegate OnBeginDragNode;
    public delegate void BeginDragDelegate(SkillNode draggedNode, PointerEventData eventData);

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickNode?.Invoke(_skillIcon.BoundTarget, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnClickNode?.Invoke(_skillIcon.BoundTarget, eventData);  // Also trigger click when starting to drag
        OnBeginDragNode?.Invoke(_skillIcon.BoundTarget, eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Intentionally left empty. Must implement IDragHandler to implement IBeginDragHandler
    }
    
    private void Reset()
    {
        _skillIcon = GetComponent<SkillIcon>();
    }
}
