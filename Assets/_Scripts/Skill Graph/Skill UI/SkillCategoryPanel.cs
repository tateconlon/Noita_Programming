using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCategoryPanel : MonoBehaviour, IBindable<SkillCategory>
{
    [Header("Bindings")]
    [SerializeField] private Image _panel;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    
    public SkillCategory BoundTarget { get; private set; }
    
    [Button]
    public void Bind(SkillCategory category)
    {
        BoundTarget = category;
        
        _panel.color = category.backgroundColor;
        _icon.sprite = category.icon;
        _name.text = category.displayName;
    }
}
