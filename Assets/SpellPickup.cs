using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SpellPickup : MonoBehaviour, IBindable<SpellItem>
{
    public SpellItem spell;
    public SpellItemList playerInventory;
    public SpriteRenderer sprite;
    public TextMeshProUGUI tooltip;
    public AudioClip pickupAudio;
    [NonSerialized, ShowInInspector] public bool isRoomPickup;
    
    //Quick and dirty way to only select one Pickup
    public static event Action RoomPickupChosen;
    
    public SpellItem BoundTarget
    {
        get
        {
            return spell;
        }
    }

    private void Start()
    {
        GameManager.Instance.Shop.OnSetIsActive += OnActivateShop;
        OnActivateShop(GameManager.Instance.Shop.IsActive);
    }
    
    private void OnActivateShop(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    
    void OnEnable()
    {
        RoomPickupChosen += RoomPickupChosenCallback;
        Bind(spell);
    }

    void OnDisable()
    {
        RoomPickupChosen -= RoomPickupChosenCallback;
    }

    public void PickUp()
    {
        bool success = Wand.equippedWand.TryAddSpell(spell);
        
        if (success)
        {
            M.am.PlayGlobalSfx(pickupAudio, volume: 1f);
            Hide();
            if (isRoomPickup)
            {
                RoomPickupChosen?.Invoke();
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RoomPickupChosenCallback()
    {
        if (isRoomPickup)
        {
            Hide();
        }
    }

    public void Bind(SpellItem target)
    {
        spell = target;
        sprite.sprite = spell.definition.icon;
        tooltip.text = spell.GetToolTip() + "\n    Press [E] to Pickup";;
    }

    private void OnDestroy()
    {
        GameManager.Instance.Shop.OnSetIsActive -= OnActivateShop;
    }
}
