using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using TMPro;
using UnityEngine;

public class ShopRollerV2 : MonoBehaviour
{
    public List<SpellItemV2> pool = new();
    public int shopTier = 0;
    public GlobalInt gold;

    public bool hasRolled = false;
    
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private TextMeshProUGUI tierText;

    public static ShopRollerV2 Instance;
    public static event Action<ShopRoll> OnRoll;
    public static event Action OnInit;
    public static event Action OnGoldChange;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.Shop.OnSetIsActive += OnActivateShop;
        OnActivateShop(GameManager.Instance.Shop.IsActive);
    }

    private void OnDestroy()
    {
        GameManager.Instance.Shop.OnSetIsActive -= OnActivateShop;
    }

    private void OnActivateShop(bool isActive)
    {
        gameObject.SetActive(isActive);
        tierText.text = $"Tier {shopTier}\nNext Tier in {3 - GameManager.Instance.CurWaveIndex % 3} waves";
        shopTier = GameManager.Instance.CurWaveIndex / 3 + 1;
        if (isActive)
        {
            hasRolled = false;
            Roll();
        }
    }

    public void ChangeGold(int amount)
    {
        if(amount == 0) return;
        gold.value += amount;
        gold.value = Mathf.Max(0, gold.value);
        OnGoldChange?.Invoke();
    }

    public void Roll(int goldCost = 0)
    {
        if (goldCost > gold)
        {
            return;
        }
        gold.value -= goldCost;
        OnGoldChange?.Invoke();
        
        ShopRoll shopRoll = new()
        {
            rewards = new(),
        };

        //Can roll times than available slots, doesn't matter so we roll 20 times since
        //there will never be more than 20 slots
        for (int i = 0; i < 20; i++)    
        {
            //equal weight for all spells available in pool - Super Auto Pets style
            SpellItemV2 rolledSpell = pool.WeightedPick(spell => spell.uiInfo.tier <= shopTier ? 1 : 0);
            shopRoll.rewards.Add(rolledSpell);
        }
            
        OnRoll?.Invoke(shopRoll);
    }
    
    //Intermediate class so we can have a list of lists in the inspector
    [Serializable]
    public class ShopRoll
    {
        [SerializeField] public List<SpellItemV2> rewards;
    }
}