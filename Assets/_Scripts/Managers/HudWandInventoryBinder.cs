using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[AddComponentMenu("_UI/HUD/HUD Wand Inventory Binder")]
public class HudWandInventoryBinder : MonoBehaviour
{
    private Wand curWand;

    [SerializeField] private List<CastingBlockDepthUI> castingBlockDepthUIList;

    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private CastingBlockDepthUI castingBlockDepthUIPrefab;

    
    void OnEnable()
    {
        Wand.OnChange += Bind;
        //
        // for (int i = 0; i < slots.Count; i++)
        // {
        //     slots[i].OnChange += SpellChanged;
        //     
        //     if(i < slots.Count - 1) slots[i].nextSlot = slots[i + 1];
        // }

        //We've subscribed to the change event, so we can wait if it's null right now.
        //Wand is a singleton so it will eventually subscribe itself
        //This resolves the Awake/OnEnable singleton assignment race condition
        if (Wand.equippedWand != null)  
        {
            Bind(Wand.equippedWand);
        }
    }
    
    void OnDisable()
    {
        Wand.OnChange -= Bind;
        // foreach (HUDSpellSlot slot in slots)
        // {
        //     slot.OnChange -= SpellChanged;
        // }
    }

    void Bind(Wand w)
    {
        curWand = w;

        //temp until Tate figures out how this should flow
        foreach (CastingBlockDepthUI castingBlockDepthUI in castingBlockDepthUIList)
        {
            Destroy(castingBlockDepthUI.gameObject);
        }
        castingBlockDepthUIList.Clear();
        
        if(w == null) return;

        List<CastingBlock> currDepthCBs = new();
        currDepthCBs.Add(w._rootCastingBlock);    //We add this since "clicking" triggers the first casting block
        
        CastingBlockDepthUI currCBDepthUI = Instantiate(castingBlockDepthUIPrefab, this.transform);
        currCBDepthUI.Bind(currDepthCBs);
        castingBlockDepthUIList.Add(currCBDepthUI);

        //Every spell in the list
        //TODO TATE: Removeas this check?
        int i = 0;
        while(i < 100)
        {
            i++;
            currDepthCBs = new();
            //Getting the next casting blocks from our current CB Level
            foreach (CastingBlock castingBlock in currCBDepthUI.BoundTarget)
            {
                foreach (SpellItem castingBlockSpell in castingBlock.getSpells())
                {
                    if (castingBlockSpell == null) continue;
                    
                    if (castingBlockSpell.hasChildren)
                    {
                        currDepthCBs.Add(castingBlockSpell.triggeredCastingBlock);
                    }
                }
            }

            if (currDepthCBs.Count == 0) break;
                
            currCBDepthUI = Instantiate(castingBlockDepthUIPrefab, this.transform);
            currCBDepthUI.Bind(currDepthCBs);
            castingBlockDepthUIList.Add(currCBDepthUI);
        }
    }

    // void SpellChanged(HUDSpellSlot slot, SpellItem spell)
    // {
    //
    //     int slotIndex = slots.FindIndex(x => x.gameObject == slot.gameObject);
    //     
    //     //If adding a spell, insert in slot and shift all spells to the right.
    //     //Shift until there's a hole. eg. [x][y][null][z] -> [insert][x][y][z]
    //     //If shfiting causes an overflow, put the last spell in the inventory
    //     if (spell != null)
    //     {
    //         //Find next empty slot after the insertion point
    //         int emptySlotIndex = slotIndex;
    //         for (; emptySlotIndex < curWand.Spells.Length; emptySlotIndex++)
    //         {
    //             if (curWand.Spells[emptySlotIndex] == null) break;
    //         }
    //     
    //         //If we couldn't find an empty slot, put whatever is in the last slot in the inventory
    //         //Then set the empty slot to the last in the wand, since it's now empty
    //         if (emptySlotIndex == curWand.Spells.Length)
    //         {
    //             emptySlotIndex = curWand.Spells.Length - 1;
    //             for (int x = 0; x < playerInventory.maxSize; x++)
    //             {
    //                 if (playerInventory[x] == null)
    //                 {
    //                     playerInventory[x] = curWand.Spells[^1]; //^1 is negative index (last)
    //                     break;
    //                 }
    //             }
    //         }
    //     
    //         //Shift all current spells to the right, starting by filling empty slot and going backwards
    //         for (int k = emptySlotIndex - 1; k >= slotIndex; k--)
    //         {
    //             curWand.Spells[k + 1] = curWand.Spells[k];
    //         }
    //     }
    //     //Put spell in the spot
    //     curWand.Spells[slotIndex] = spell;
    //     
    //     //Compress the wand slots by removing holes.
    //     //Iterate through, when there's a hole, fill it with the next non-null member
    //     for (int i = 0; i < curWand.Spells.Length; i++)
    //     {
    //         if (curWand.Spells[i] != null) continue;
    //         
    //         int runner;
    //         for (runner = 0; runner + i < curWand.Spells.Length; runner++)
    //         {
    //             if (curWand.Spells[i + runner] == null) continue;
    //     
    //             curWand.Spells[i] = curWand.Spells[i + runner];
    //             curWand.Spells[i + runner] = null;
    //             break;
    //         }
    //         
    //         //Reached the end of the array without finding non-null entry
    //         if (i + runner == curWand.Spells.Length)    break;
    //     }
    //     
    //     curWand.RefreshSpells();    //Refresh spells to calculate new casting blocks
    //     
    //     //Now spells are swapped, update visuals
    //     Bind(curWand);
    //
    // }
}