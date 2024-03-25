using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInventorySlot : MonoBehaviour, IBindable<SpellItem>, IPointerDownHandler
{
    private SpellItem _target;
    public SpellItem BoundTarget
    {
        get { return _target; }
    }

    public SpellItemList list;

    [ShowInInspector, NonSerialized] public int listIndex;

    [FoldoutGroup("Bindings", false)]
    [SerializeField] private Image spellIcon;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private ParticleSystem filledPS;

    void OnEnable()
    {
        spellIcon.raycastTarget = false; //this should always be false, so the BGslot image can catch the raycast
    }
    
    public void Bind(SpellItem target)
    {
        _target = target;

        if (_target == null)
        {
            spellIcon.gameObject.SetActive(false);
            filledPS.gameObject.SetActive(false);
        }
        else
        {
            spellIcon.gameObject.SetActive(true);
            spellIcon.sprite = _target.definition.icon;
            filledPS.gameObject.SetActive(true);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        SpellItem ourSpell = _target;
        SpellItem newSpell = MouseInventorySlot.main.SwapSpell(ourSpell);
        
        //Calls PlayerInventoryList.OnModifier which cause rebinding of the inventory.
        list[listIndex] = newSpell;

        if (newSpell == null) return;

        List<SpellItem> flattenedTree = newSpell.GetListOfAllContainedSpells(false);
        
        //Add the elements to a bigger Inventory
        list.maxSize += flattenedTree.Count;
        for (int i = 0; i < flattenedTree.Count; i++)
        {
            //Clear all casting block data
            flattenedTree[i].ClearCastingBlocks();

            //Add each spell to the inventory
            int indexFromBack = flattenedTree.Count - i;
            list[^indexFromBack] = flattenedTree[i];
        }

        newSpell.ClearCastingBlocks();
    }

    //Depth first walk of the tree
    //This may fail if a SpellItem somehow points back to itself.
    //I've included the depth check with an error message to help with that
    void GetListOfAllContainedSpells(SpellItem spell, List<SpellItem> persistentlist, int depth)
    {
        if (depth <= 0)
        {
            Debug.LogError("CRITICAL ERROR! Possible recursive casting block!");
            return;
        }
        
        if (spell == null || spell.triggeredCastingBlock == null) return;

        foreach (SpellItem nextSpell in spell.triggeredCastingBlock.Spells)
        {
            if (nextSpell == null) continue;
            
            //Not checking for duplicate spells because then the error will be noticable
            //And we can diagnose it
            persistentlist.Add(nextSpell);
            GetListOfAllContainedSpells(nextSpell, persistentlist, depth--);
        }
    }
}
