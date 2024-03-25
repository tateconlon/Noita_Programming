using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseInventorySlot : MonoBehaviour
{
    public static MouseInventorySlot main;

    [NonSerialized, ShowInInspector] SpellItem spell = null;
    [ShowInInspector] public bool isHolding => spell != null;

    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] [Required] private Image icon;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private GameObject iconRoot;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private List<Image> otherIcons;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private SpellPickup spellPickupPrefab;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private GlobalVector2 playerPosition;

    void OnEnable()
    {
        //Singleton Behaviour
        if (main == null)
        {
            main = this;
        }
        ResetVisuals();
    }

    void OnDisable()
    {
        main = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !PointerManager.IsOverUi && main.isHolding)
        {
            DropSpell();
        }
    }

    void DropSpell()
    {
        //This is for dropping where your mouse is
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        Vector3 dropDir = mousePos - (Vector3)playerPosition.value;
        dropDir.Normalize();

        //Drop 1.5 units in direction of mouse. Just feels kinda goooood
        Vector3 dropPos = (Vector3)playerPosition.value + dropDir * 1.5f;

        Vector3 spacingOffset = Vector3.right * 1.5f;
        List<SpellItem> droppedSpells = spell.GetListOfAllContainedSpells(true);
        foreach (SpellItem dropped in droppedSpells)
        {
            SpellPickup pickup = Instantiate(spellPickupPrefab, dropPos, Quaternion.identity);
            pickup.name = $"Dropped Spell {dropped.definition.name}";
            
            dropped.ClearCastingBlocks();
            pickup.Bind(dropped);
            
            dropPos += spacingOffset;
        }

        SwapSpell(null);

    }

    /// <returns>Old spell</returns>
    public SpellItem SwapSpell([CanBeNull] SpellItem newSpell)
    {
        SpellItem oldSpell = spell;
        spell = newSpell;
        if (spell != null)
        {
            spell.currCastingBlock = null;
        }

        ResetVisuals();
        
        return oldSpell;
    }

    void ResetVisuals()
    {
        foreach (Image otherIcon in otherIcons)
        {
            Destroy(otherIcon.gameObject);
        }
        otherIcons.Clear();
        
        if (spell != null)
        {
            //Using enabled because image is on the same object,
            //we don't want to disable the gameobject which turns MouseInventorySlot off
            icon.enabled = true;   
            icon.sprite = spell.definition.icon;

            foreach (SpellItem triggeredSpell in spell.GetListOfAllContainedSpells(false))
            {
                Image img = Instantiate(icon, iconRoot.transform);
                img.sprite = triggeredSpell.definition.icon;
                otherIcons.Add(img);
            }

        }
        else
        {
            icon.enabled = false;
        }
    }
}
