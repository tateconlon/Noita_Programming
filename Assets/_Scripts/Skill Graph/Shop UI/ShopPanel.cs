using System;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private int numCards = 3;
    private int currNumCards = 0;
    
    [FoldoutGroup("Binding")]
    [SerializeField] private GameObject _visiblePanel;
    [FoldoutGroup("Binding")]
    [SerializeField] private SkillShopInstance _shopActionSkillInventory;
    [FoldoutGroup("Binding")]
    [SerializeField] private SkillShopInstance _shopTriggerSkillInventory;
    [FoldoutGroup("Binding")]
    [SerializeField] private SkillInventoryInstance _skillInventory;
    [FoldoutGroup("Binding")]
    [SerializeField] private CollectionBoundPrefabs<SkillNode, SkillIcon> _shopActionSkills;
    [FoldoutGroup("Binding")]
    [SerializeField] private CollectionBoundPrefabs<SkillNode, SkillIcon> _shopTriggerSkills;
    [FoldoutGroup("Binding")]
    [SerializeField] private Button _rerollButton;
    [FoldoutGroup("Binding")]
    [SerializeField] private ScopedVariable<int> _playerGold;
    
    [FoldoutGroup("Binding")]
    [SerializeField, Required] RandomNodeGenerator _nodeGenerator;
    
    private void OnEnable()
    {
        _shopActionSkillInventory.Value.OnModified += OnShopSkillInventoryModified;
        _shopTriggerSkillInventory.Value.OnModified += OnShopSkillInventoryModified;
        _shopActionSkillInventory.Value.OnBuy += RefreshShopOnBuy;
        _rerollButton.onClick.AddListener(OnRequestReroll);

        currNumCards = numCards;

        OnShopSkillInventoryModified();
    }
    
    private void OnShopSkillInventoryModified()
    {
        _shopActionSkills.Bind(_shopActionSkillInventory.Value);
        _shopTriggerSkills.Bind(_shopTriggerSkillInventory.Value);
    }
    
    private void OnRequestReroll()
    {
        if (_playerGold < 2) return;

        _playerGold.value -= 2;
        RefreshShop(1);
    }

    public void RefreshShop(int numTimes)
    {
        if (numTimes > 0)
        {
            _nodeGenerator.RequestGenerateNodes(3, 2);
        }
    }

    private void RefreshShopOnBuy(SkillNode skillNode)
    {
        _skillInventory.Value.CopyNode(skillNode);
        currNumCards--;
        if (currNumCards > 0)
        {
            RefreshShop(1);
        }
        else
        {
            _shopActionSkillInventory.Value.Clear();
            this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        _shopActionSkillInventory.Value.OnModified -= OnShopSkillInventoryModified;
        _shopTriggerSkillInventory.Value.OnModified -= OnShopSkillInventoryModified;
        _shopActionSkillInventory.Value.OnBuy -= RefreshShopOnBuy;
        
        _rerollButton.onClick.RemoveListener(OnRequestReroll);
    }
}
