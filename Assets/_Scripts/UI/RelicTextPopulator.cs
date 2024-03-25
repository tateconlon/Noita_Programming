using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class RelicTextPopulator : MonoBehaviour
{
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    void OnShopEnter(bool isActive)
    {
        if (isActive)
        {
            Refresh();
        }
    }
    
    void Refresh()
    {
        List<Relic> relic = RelicsManager.instance.relics;
        foreach (TextMeshProUGUI text in _texts)
        {
            Destroy(text.gameObject);
        }
        _texts.Clear();
        foreach (Relic r in relic)
        {
            TextMeshProUGUI text = Instantiate(Resources.Load<TextMeshProUGUI>("RelicText"), transform);
            text.text = "- " + r.GiveName();
            _texts.Add(text);
        }
    }

    void Awake()
    {
        GameManager.Instance.Shop.OnSetIsActive += OnShopEnter;
    }

    private void OnDestroy()
    {
        GameManager.Instance.Shop.OnSetIsActive -= OnShopEnter;
    }
}