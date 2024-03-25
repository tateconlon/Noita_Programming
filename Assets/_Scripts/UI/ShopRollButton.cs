using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRollButton : MonoBehaviour
{
    [SerializeField] private Button buttonImage;
    [SerializeField] private TextMeshProUGUI goldText;
    public void Awake()
    {
        ShopRollerV2.OnGoldChange += OnGoldChange;
    }

    public void OnDestroy()
    {
        ShopRollerV2.OnGoldChange -= OnGoldChange;
    }

    public void OnClick()
    {
        if (!ShopRollerV2.Instance.hasRolled)
        {
            ShopRollerV2.Instance.Roll(5);
        }
    }
    
    private void OnGoldChange()
    {
        goldText.text = $"Gold: {ShopRollerV2.Instance.gold.value.ToString()}";
        if (ShopRollerV2.Instance.gold >= 5)
        {
            buttonImage.interactable = true;
        }
        else
        {
            buttonImage.interactable = false;
        }
    }
}