using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof (TextMeshProUGUI))]
public class ShopRollTitleText : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField] private string titleText;
    [SerializeField] private string titleText2;
    void OnEnable()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        
        ShopItemUI.ChooseNewSpell += OnChooseNewSpellItem;
        text.text = titleText;
    }

    private void OnDisable()
    {
        ShopItemUI.ChooseNewSpell -= OnChooseNewSpellItem;
    }

    void OnChooseNewSpellItem(ShopItemUI _)
    {
        text.text = titleText2;
    }
}