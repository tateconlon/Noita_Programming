using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[AddComponentMenu("_UI/HUD/Spell Slot")]
public class HUDSpellSlot : MonoBehaviour, IBindable<SpellItem>
{
    [SerializeField] private HUDDraggableSpell dragSlot;
    
    public CastingBlock myCastingBlock;
    public int castingBlockIndex;
    
    public bool InventorySlot = false;  //TODO: remove - Quick n' dirty negating of castingblocks visuals for inventory slots

    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Image spellImage;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Image rightCastingBlockImg;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Image leftCastingBlockImg;

    public SpellItem BoundTarget
    {
        get { return dragSlot.BoundTarget; }
    }

    /// <summary>
    /// DO NOT USE THIS METHOD, YOU MUST SPECIFY A CASTING BLOCK AND INDEX
    /// If this is for something that doesn't have a casting block
    /// use castingBlock = null, index = -1 with the other bind method
    /// </summary>
    /// <param name="target"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Bind(SpellItem target)
    {
        throw new NotImplementedException();
    }

    public event Action<HUDSpellSlot, SpellItem> OnChange;

    public void OnEnable()
    {
        dragSlot.RequestChange += FireRequestChangeEvent;
    }
    
    public void OnDisable()
    {
        dragSlot.RequestChange -= FireRequestChangeEvent;
    }

    public void Bind(SpellItem target, CastingBlock cb, int cbIndex)
    {
        myCastingBlock = cb;
        castingBlockIndex = cbIndex;
        dragSlot.Bind(target, cb, cbIndex);

        ResetVisuals();
    }

    void ResetVisuals()
    {
        if (BoundTarget == null || InventorySlot)
        {
            leftCastingBlockImg.gameObject.SetActive(false);
            rightCastingBlockImg.gameObject.SetActive(false);
            return;
        }

        CastingBlock cb;

        if (!Wand.equippedWand._spellsToCastingBlocks.TryGetValue(BoundTarget, out cb))
        {
            leftCastingBlockImg.gameObject.SetActive(false);
            rightCastingBlockImg.gameObject.SetActive(false);
            return;
        }
        
        leftCastingBlockImg.gameObject.SetActive(true);
        rightCastingBlockImg.gameObject.SetActive(true);
        // FillCastingBlock(leftCastingBlockImg, cb);
        
        // if (cb.ChildCastingBlock != null && BoundTarget.definition is AddTriggerSpellDefinition)
        // {
        //     FillCastingBlock(rightCastingBlockImg, cb.ChildCastingBlock); 
        // }
        // else
        // {
        //     rightCastingBlockImg.color = leftCastingBlockImg.color;
        // }
    }

    public void EnableParticleSystem()
    {
        dragSlot._particleSystem.SetActive(true);
    }

    // void FillCastingBlock(Image img, CastingBlock cb)
    // {
    //     switch (cb.OrderInWandUi)
    //     {
    //         case 0:
    //             img.color = zero;
    //             break;
    //         case 1:
    //             img.color = one;
    //             break;
    //         case 2:
    //             img.color = two;
    //             break;
    //         case 3:
    //             img.color = three;
    //             break;
    //         case 4:
    //             img.color = four;
    //             break;
    //         case 5:
    //             img.color = five;
    //             break;
    //         case 6:
    //             img.color = six;
    //             break;
    //         case 7:
    //             img.color = seven;
    //             break;
    //         case 8:
    //             img.color = eight;
    //             break;
    //         default:
    //             break;
    //     }
    // }

    void FireRequestChangeEvent(SpellItem spellItem)
    {
        //TODO TATE: Fix
        OnChange?.Invoke(this, spellItem);
    }
}
