using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("_UI/HUD/HUD Player Inventory Binder")]
public class HUDPlayerInventoryBinder : MonoBehaviour
{
    [Required] public SpellItemList playerInventory;

    [ChildGameObjectsOnly]
    public List<PlayerInventorySlot> slots;

    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private PlayerInventorySlot slotPrefab;
    void OnEnable()
    {
        playerInventory.OnModified += Bind;
        // foreach (HUDSpellSlot slot in slots)
        // {
        //     slot.OnChange += SpellChanged;
        // }
        Bind();
    }
    
    void OnDisable()
    {
        playerInventory.OnModified -= Bind;
        // foreach (HUDSpellSlot slot in slots)
        // {
        //     slot.OnChange -= SpellChanged;
        // }
    }

    void Bind()
    {
        //If playerInventory got bigger, add more slots. It's fine if we have less slots than inventory
        while (slots.Count < playerInventory.maxSize)
        {
            PlayerInventorySlot newSlot = Instantiate(slotPrefab, transform);
            newSlot.name = $"Spell Slot ({slots.Count})";
            newSlot.transform.SetSiblingIndex(999); //last sibling
            // newSlot.OnChange += SpellChanged;
            slots.Add(newSlot);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < playerInventory.maxSize)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Bind(playerInventory[i]);
                slots[i].list = playerInventory;
                slots[i].listIndex = i;
            }
            else
            {
                slots[i].gameObject.SetActive(false);
                
                //TODO TATE: Is this where we'd implement dropping?
                slots[i].Bind(null);
            }
        }
    }

    // Code copied from DraggableSpell    
     // void SpellChanged(HUDSpellSlot slot, SpellItem spell)
     // {
     //
     //     int i = slots.FindIndex(x => x.gameObject == slot.gameObject);
     //
     //     playerInventory[i] = spell;
     //     Bind();
     // }
}
