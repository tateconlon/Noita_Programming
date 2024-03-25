using UnityEngine;
using UnityEngine.EventSystems;

public class BindSkillNodeOnClick : MonoBehaviour
{
    [SerializeField] private SkillInfoPanel _skillInfoPanel;

    private void OnEnable()
    {
        SkillIconUiEventHandler.OnClickNode += OnClickNode;
    }

    private void OnClickNode(SkillNode clickedNode, PointerEventData eventData)
    {
        _skillInfoPanel.Bind(clickedNode);
    }
    
    private void OnDisable()
    {
        SkillIconUiEventHandler.OnClickNode -= OnClickNode;
    }
}
