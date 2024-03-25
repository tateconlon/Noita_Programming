using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IBindable<SkillNode>
{
    [Header("Bindings")] 
    [SerializeField] private Image _bg;
    [SerializeField] private Image _icon;
    //[SerializeField] private Image _tierPanel;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _numChildrenValue;
    [SerializeField] private TMP_Text _tierText;

    [Header("Temp")]
    [SerializeField] private float _emptyFadeAmount = 0.5f;
    [SerializeField] private Color _attackColor = Color.blue;
    [SerializeField] private Color _triggerColor = Color.red;
    
    public SkillNode BoundTarget { get; private set; }
    
    public void Bind(SkillNode target)
    {
        BoundTarget = target;

        _bg.color = target is ActionNode ? _attackColor : _triggerColor;
        //_icon.color = target is ActionNode ? _attackColor : _triggerColor;
        
        _tierText.enabled = !target.IsEmpty && target.Definition.canRollInShop;
        
        if (target.IsEmpty)
        {
            _icon.sprite = null;
            _icon.color = target is ActionNode ? _attackColor : _triggerColor;
            _icon.color = Color.Lerp(_icon.color, Color.black, _emptyFadeAmount);
            //_tierPanel.color = Color.clear;
            _label.text = target is ActionNode ? "add creature" : "add wand";;
        }
        else
        {
            _icon.sprite = target.Definition.icon;
            _icon.color = Color.white;
            // _tierPanel.color = target.Tier.backgroundColor;  // Could set tier panel color to color based on level
            _label.text = target.Definition.displayName;
            _tierText.text = target.Definition.tier.ToString();
        }
        
        _numChildrenValue.text = target.maxNumChildren.ToString();
    }

    public void TryBuy()
    {
        SkillShop ss = (SkillShop)BoundTarget.graph;
        ss.TryBuySkill(BoundTarget);
    }
}
