using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SlotConnector : MonoBehaviour
{
    public static Color gold = new Color(0.95f, 0.7094675f, 0, 1f);
    public static Color dark = new Color(0, 0, 0, 0.5058824f);

    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Image connectorImg;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private ShopItemUI leftSlot;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private ShopItemUI rightSlot;

    // Update is called once per frame
    void Update()
    {
        if(leftSlot.BoundTarget == null || rightSlot.BoundTarget == null)
        {
            connectorImg.color = dark;
            return;
        }
        
        connectorImg.color = gold;
    }
}
