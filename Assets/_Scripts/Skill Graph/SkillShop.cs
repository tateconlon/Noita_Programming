using System;
using ThirteenPixels.Soda;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "Skill Shop", menuName = "ScriptableObject/Skills/Skill Shop")]
public class SkillShop : SkillInventory
{
    [SerializeField] private ScopedVariable<int> _shopCurrency;
    public event Action<SkillNode> OnBuy;
    
    public bool CanBuySkill(SkillNode skillNode)
    {
        return _shopCurrency.value >= skillNode.Definition.Cost;
    }

    public bool TryBuySkill(SkillNode skillNode)
    {
        if (!CanBuySkill(skillNode)) return false;

        _shopCurrency.value -= skillNode.Definition.Cost;
        OnBuy?.Invoke(skillNode);
        return true;
    }
    
    public void SellSkill(SkillNode skillNode)
    {
        _shopCurrency.value += skillNode.Definition.Cost;
    }
}
