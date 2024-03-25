using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;using Image = UnityEngine.UI.Image;

public class HUDDraggableSpell : MonoBehaviour, IBindable<SpellItem>, IPointerDownHandler
{
    [ShowInInspector]
    [CanBeNull] //Just for debugging
    private SpellItem _target;
    
    public CastingBlock myCastingBlock;
    public int castingBlockIndex;

    [SerializeField]
    private AudioClip clickSfx;
    public SpellItem BoundTarget
    {
        get { return _target; }
    }

    public void Bind(SpellItem target)
    {
        throw new NotImplementedException();
    }

    public event Action<SpellItem> RequestChange;

    [FoldoutGroup("Bindings", false), SerializeField]
    private Image icon;
    [FoldoutGroup("Bindings", false), SerializeField]
    TextMeshProUGUI upperTooltip;
    [FoldoutGroup("Bindings", false), SerializeField]
    TextMeshProUGUI lowerTooltip;

    [FoldoutGroup("Bindings", expanded: false)]
    //TODO: This should belong to spellSlot, but it doesn't know about click events to turn off the particle system
    //This should be fixed later
    public GameObject _particleSystem;  

    public void Bind(SpellItem target, CastingBlock cb, int cbIndex)
    {
         bool fireOnChange = _target != target;

         _target = target;
         myCastingBlock = cb;
         castingBlockIndex = cbIndex;

         if (_target != null && _target.definition == null)
         {
             Debug.LogError("Somethin' weird is goin' on", this.gameObject);
         }
         
         icon.color = _target != null ? Color.white : Color.clear;
         icon.sprite = _target != null && _target.definition != null ? _target.definition.icon : null;
        if (_target == null)
        {
            icon.color = Color.clear;
            lowerTooltip.text = "";
            upperTooltip.text = "";
        }
        else
        {
            string tooltip = tooltip = _target.GetToolTip();
            
            //If we're an Attack in a casting block, we construct a tooltip listing all the mods & triggers
            if (myCastingBlock != null)
            {
                if (_target.definition is LaunchProjectileSpellDefinition)
                {
                    bool hasMods = false;
                    foreach (ModifyProjectileSpellDefinition spellDef in myCastingBlock.ModifyProjectileSpells)
                    {
                        if (!hasMods)
                        {
                            tooltip += ":\n";
                            hasMods = true;
                        }
                        
                        tooltip += $"{spellDef.attackDescTooltip}\n";
                    }
            
                    //Triggers are in the parent casting block
                    //This tooltip doesn't make sense with the current Inventory system
                    //But I'm keeping the code in case we need it later
                    // if (myCastingBlock.ParentSpell != null)
                    // {
                    //     if(myCastingBlock.ParentSpell.definition is AddTriggerSpellDefinition triggerSpellDef)
                    //     {
                    //         if (!hasMods)
                    //         {
                    //             tooltip += ":\n";
                    //             hasMods = true;
                    //         }
                    //         tooltip += $"{triggerSpellDef.attackDescTooltip}\n";
                    //     }
                    //     
                    // }
                }
            }
            
                lowerTooltip.text = tooltip;
                upperTooltip.text = tooltip;
                
                icon.color = Color.white;
                icon.sprite = target.definition.icon;
            }
            
             //if(fireOnChange) RequestChange?.Invoke();
    }

    #region Drag & Drop
    //HUDSpellSlot parentSlot;
    //Canvas cachedCanvas; //To Reduce GetComponentInParent in OnDrag
    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     M.am.PlayGlobalSfx(clickSfx);
    //     
    //     // Can't drag empty slots, cancel the event
    //     if (_target == null) eventData.pointerDrag = null;
    //     
    //     _particleSystem.SetActive(false);
    //     
    //     cachedCanvas = gameObject.GetComponentInParent<Canvas>();
    //     parentSlot = transform.parent.GetComponent<HUDSpellSlot>();  //Save our spot if we don't drop somewhere good
    //     
    //     transform.SetParent(transform.root);
    //     transform.SetAsLastSibling();   //Last in hierarchy so it's above all UI
    //     
    //     icon.raycastTarget = false; //So we can raycast through when we drop
    // }
    //
    //
    //
    // public void OnDrag(PointerEventData eventData)
    // {
    //     OnMouseStay();
    //     Vector2 pos;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedCanvas.transform as RectTransform, Input.mousePosition, cachedCanvas.worldCamera, out pos);
    //     transform.position = cachedCanvas.transform.TransformPoint(pos);
    // }
    //
    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     RectTransform trans = transform as RectTransform;
    //     trans.SetParent(parentSlot.transform);
    //     trans.localPosition = Vector3.zero;
    //     icon.raycastTarget = true;
    //     
    //     M.am.PlayGlobalSfx(clickSfx, volume: 0.8f, pitch: 0.7f);  //lower pitch == high volume, so we lower volume to compensate
    // }
    //
    // public void OnDrop(PointerEventData eventData)
    // {
    //     SpellItem ourSpell = _target;
    //     //Bind dropped spell to us
    //     GameObject dropped = eventData.pointerDrag;
    //     HUDDraggableSpell droppedSpell = dropped.GetComponent<HUDDraggableSpell>();
    //     SpellItem newSpell = droppedSpell.BoundTarget;
    //
    //     
    //     
    //     //We need to make the empty slot null first
    //     //So that spells can properly shift into it.
    //     //First eg is null changed first, second is null changed second (wrong)
    //     //[a][b][/] -> put b in a -> [a][/][/] -> [b][a][/]
    //     //[a][b][/] -> put b in a -> [b][a][b] -> [b][/][b]
    //     //The shifting causes a different spell to be put in the slot that we would null out if done second
    //     
    //     droppedSpell.RequestChangeSpell(null);
    //     RequestChange?.Invoke(newSpell);
    //     
    //     //Hacky, but we need a way to refresh the wand since we edit the wand directly then call the change on it
    //     Wand.equippedWand.RefreshSpells();
    // }

    public void RequestChangeSpell(SpellItem spell)
    {
        //RequestChange?.Invoke(spell);
    }
    #endregion
    #region Tooltip
    //Called from EventTrigger Component
    public void OnMouseEnter()
    {
        if (Input.mousePosition.y > Screen.height / 2f)
        {
            lowerTooltip.gameObject.SetActive(true);
            upperTooltip.gameObject.SetActive(false);
        }
        else
        {
            lowerTooltip.gameObject.SetActive(false);
            upperTooltip.gameObject.SetActive(true);
        }
    }

    //Called from Update
    public void OnMouseStay()
    {
        if (Input.mousePosition.y > Screen.height / 2f)
        {
            lowerTooltip.gameObject.SetActive(true);
            upperTooltip.gameObject.SetActive(false);
        }
        else
        {
            lowerTooltip.gameObject.SetActive(false);
            upperTooltip.gameObject.SetActive(true);
        }
    }
    
    //Called from EventTrigger Component
    public void OnMouseExit()
    {
        lowerTooltip.gameObject.SetActive(false); 
        upperTooltip.gameObject.SetActive(false);
    }

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        SpellItem ourSpell = _target;
        SpellItem newSpell = MouseInventorySlot.main.SwapSpell(ourSpell);
        
        //Our slot represents a spot in the tree
        if (myCastingBlock != null)
        {
            myCastingBlock.InsertSpell(castingBlockIndex, newSpell);
        }
        
        //We refresh spells this instead of binding
        //Because the flow is that we change the wand then the wand propogates changes
        //through HUDPlayerInventoryBinder, which will do all binding as it walks the CastingBlock tree
        _target = newSpell;
        Wand.equippedWand.RefreshSpells();
    }
    
   
}
