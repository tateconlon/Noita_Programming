using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class ChooseSpellAnalytics : MonoBehaviour
{
    private void OnEnable()
    {
        ShopItemUI.ChooseNewSpell += OnChooseNewSpellItem;
    }
    
    private void OnChooseNewSpellItem(ShopItemUI shopItemUI)
    {
        Dictionary<string, object> parameters = new();
        
        parameters["spellName"] = shopItemUI.BoundTarget.name;
        parameters["wave"] = GameManager.Instance.CurWaveIndex;
        
        AnalyticsService.Instance.CustomData("chooseSpell", parameters);
    }
    
    private void OnDisable()
    {
        ShopItemUI.ChooseNewSpell -= OnChooseNewSpellItem;
    }
}