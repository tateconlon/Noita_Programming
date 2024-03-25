using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoPanel : MonoBehaviour, IBindable<SkillNode>
{
    [Header("Bindings")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _tierPanel;
    [SerializeField] private TMP_Text _skillName;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _statText;
    [SerializeField] private TMP_Text _numChildrenValue;
    [SerializeField] private SkillCategoryPanel _categoryPanel;
    [SerializeField] private CollectionBoundPrefabs<SkillCategory, SkillCategoryPanel> _compatibleCategories;
    
    public SkillNode BoundTarget { get; private set; }
    
    [Button]
    public void Bind(SkillNode skillNode)
    {
        if (skillNode.IsEmpty)
        {
            Debug.Assert(skillNode.graph is SkillTree && skillNode.HasParent);
            
            skillNode = skillNode.ParentBase;  // Bind to the parent instead of an empty node
        }
        
        BoundTarget = skillNode;
        
        _icon.sprite = skillNode.Definition.icon;
        // _tierPanel.color = skillNode.Tier.backgroundColor;  // Could set tier panel color to color based on level
        _skillName.text = skillNode.Definition.displayName;
        _description.text = skillNode.Definition.description;
        _statText.text = skillNode.StatsToString();
        _numChildrenValue.text = skillNode.maxNumChildren.ToString();
        _categoryPanel.Bind(skillNode.Definition.category);
        _compatibleCategories.Bind(skillNode.Definition.category.rightSideCompatible);
    }
}
