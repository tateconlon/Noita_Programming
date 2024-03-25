using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SkillIcon))]
public class EventsOnClickSwappableNode : MonoBehaviour
{
    [SerializeField] private SkillIcon _skillIcon;
    
    [Header("Events")]
    public UnityEvent OnSelected;
    public UnityEvent OnUnselected;
    public UnityEvent OnCanBeSwapped;
    public UnityEvent OnCannotBeSwapped;
    
    private void OnEnable()
    {
        OnUnselected.Invoke();
        OnCannotBeSwapped.Invoke();
        
        SkillIconUiEventHandler.OnClickNode += OnClickNode;
    }

    private void OnClickNode(SkillNode clickedNode, PointerEventData eventData)
    {
        if (clickedNode == _skillIcon.BoundTarget)
        {
            OnSelected.Invoke();
        }
        else
        {
            OnUnselected.Invoke();
        }
        
        if (clickedNode.CanBeSwapped(_skillIcon.BoundTarget))
        {
            OnCanBeSwapped.Invoke();
        }
        else
        {
            OnCannotBeSwapped.Invoke();
        }
    }
    
    private void OnDisable()
    {
        SkillIconUiEventHandler.OnClickNode -= OnClickNode;
    }
    
    private void Reset()
    {
        _skillIcon = GetComponent<SkillIcon>();
    }
}
