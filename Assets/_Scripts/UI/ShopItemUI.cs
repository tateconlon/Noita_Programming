using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IBindable<SpellItemV2>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private SpellItemV2 _target;
    public SpellItemV2 BoundTarget => _target;
    public int slotIndex = -1;

    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Image bgImg;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private TextMeshProUGUI toolTip;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Image icon;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private TextMeshProUGUI levelText;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private TextMeshProUGUI dmgText;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private TextMeshProUGUI numProjText;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private TextMeshProUGUI timeText;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private UIParticle combinePS;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Sprite sizeSprite;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Sprite amountSprite;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Sprite timeSprite;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private SpellItemToManaBar manaBar;

    public static event Action<ShopItemUI> ChooseNewSpell;

    [NonSerialized] private Vector3 orgPos;

    private void Awake()
    {
        if(slotIndex >= 0)
        {
            //Subscribe
            WandV2.OnWandRebuild += Bind_OnWandRebuild;
        }
        else
        {
            //Subscribe
            ShopRollerV2.OnRoll += Bind_OnShopRoll;
        }
        
        StatsModHolder.OnChange += OnStatsChanged;
    }
    private void OnEnable()
    {
        orgPos = transform.position;
        if(slotIndex < 0) //Shop slot
        {
            ChooseNewSpell += DestroySpell_OnChooseNewSpell;
        }
        
    }

    private void OnDisable()
    {
        ChooseNewSpell -= DestroySpell_OnChooseNewSpell;
    }

    private void OnDestroy()
    {
        WandV2.OnWandRebuild -= Bind_OnWandRebuild;
        ShopRollerV2.OnRoll -= Bind_OnShopRoll;
        StatsModHolder.OnChange -= OnStatsChanged;
    }

    private void OnStatsChanged(StatsModHolder holder)
    {
        if(_target != null && _target.statsMod == holder)
        {
            Bind(_target);  //just to refresh visuals
        }
    }

    void Bind_OnWandRebuild()
    {
        if (slotIndex >= 0)
        {
            Bind(PlayerControllerV2.instance.wand.spells[slotIndex]);
        }
    }
    
    void Bind_OnShopRoll(ShopRollerV2.ShopRoll roll)
    {
        if (slotIndex <= -1)
        {
            int rollIndex = Mathf.Abs(slotIndex) - 1;   //-1 == 0, -2 == 1, etc

            //We destroy our target if we're overwriting it with a new one from shop
            if (_target != null)
            {
                Destroy(_target.gameObject);
            }
            
            SpellItemV2 spell = GameObject.Instantiate(roll.rewards[rollIndex], transform);
            spell.gameObject.name = roll.rewards[rollIndex].name;
            
            Bind(spell);
        }
    }

    private void DestroySpell_OnChooseNewSpell(ShopItemUI shopItemUI)
    {
        if (shopItemUI != this)
        {
            if (_target != null)
            {
                Destroy(_target.gameObject);
            }
            Bind(null);
        }
    }


    public void Bind(SpellItemV2 target)
    {
        _target = target;

        if (manaBar != null)
        {
            manaBar.Bind(target);
        }
        
        if(target == null)
        {
            icon.color = Color.clear;
            toolTip.text = "";
            if (bgImg != null)
            {
                bgImg.color = SlotConnector.dark;
            }

            if (levelText != null)
            {
                levelText.text = "";
                levelText.transform.parent.gameObject.SetActive(false);
            }
            
            if(dmgText == null || numProjText == null || timeText == null) return;
            
            dmgText.text = "";
            numProjText.text = "";
            timeText.text = "";
            dmgText.transform.parent.gameObject.SetActive(false);
            numProjText.transform.parent.gameObject.SetActive(false);
            timeText.transform.parent.gameObject.SetActive(false);
            return;
        }

        if (bgImg != null)
        {
            bgImg.color = SlotConnector.gold;
        }
        icon.sprite = target.uiInfo.icon;
        icon.color = Color.white;
        toolTip.text = $"{target.uiInfo.name}\n{target.GetTooltip()}";
        
        if (levelText != null)
        {
            levelText.text = $"lvl {target.uiInfo.level}: {target.uiInfo.xpInLevel}/{target.uiInfo.xpToNextLevel}";
            levelText.transform.parent.gameObject.SetActive(true);
        }

        if(dmgText == null || numProjText == null || timeText == null) return;
        
        dmgText.transform.parent.gameObject.SetActive(true);
        numProjText.transform.parent.gameObject.SetActive(true);
        //NOTE WE DON'T USE TIME ANYMORE
        timeText.transform.parent.gameObject.SetActive(false);
        
        dmgText.text = target.redText.ToString();

        if (_target.yellowStatType == StatType.Projectiles)
        {
            numProjText.transform.parent.GetComponent<Image>().sprite = amountSprite;
        }
        else if (_target.yellowStatType == StatType.ProjectileSize)
        {
            numProjText.transform.parent.GetComponent<Image>().sprite = sizeSprite;
        }
        else if(_target.yellowStatType == StatType.LifeTime)
        {
            numProjText.transform.parent.GetComponent<Image>().sprite = timeSprite;
        }
        else
        {
            print("ShopItemUI.cs: Bind(): yellowStatType not recognized");
        }
        
        numProjText.text = target.yellowText.ToString();
        timeText.text = target.blueText.ToString("F1");
        
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //Upgrade Spell If we're holding a spell and we're clicking on a spell of the same type
        if (MouseInventorySlotV2.main.BoundTarget != null
            && _target != null 
            && MouseInventorySlotV2.main.BoundTarget.GetType() == _target.GetType())
        {
            //TODO: Merge best stats, maybe this shouldn't be done as a stat mod and should just upgrade base stats
            //However, that would mean we need to make stats generic and put it into SpellItemV2
            //So we will put that off as long as possible
            List<StatChange> statChanges = new()
            {
                new StatChange()
                {
                    type = StatType.Red,
                    flatChange = 1,
                    isFlatMod = true,
                },
                new StatChange()
                {
                    type = StatType.Yellow,
                    flatChange = 1,
                    isFlatMod = true,
                },
                //Trying only 2 stats
                // new StatChange()
                // {
                //     type = StatType.Blue,
                //     flatChange = 0.25f,
                //     isFlatMod = true,
                // }
            };
            _target.statsMod.ApplyStatChanges(statChanges.ToArray());
            _target.uiInfo.xp += MouseInventorySlotV2.main.BoundTarget.uiInfo.xp;
            
            LevelUpVisuals(icon.transform);
            
            if(combinePS != null)
            {
                combinePS.Play();
            }
            
            if (dmgText != null)
            {
                LevelUpVisuals(dmgText.transform.parent.transform);
                FlashText(dmgText, Color.green, 0.15f, 4);
            }
                
            if(numProjText != null)
            {
                LevelUpVisuals(numProjText.transform.parent.transform);
                FlashText(numProjText, Color.green, 0.15f, 4);
            }
            
            if(timeText != null)
            {
                LevelUpVisuals(timeText.transform.parent.transform);
                FlashText(timeText, Color.green, 0.15f, 4);
            }

            MouseInventorySlotV2.main.SwapSpell(null);
            //Rebuild wand instead of simply binding for consistent flow 
            PlayerControllerV2.instance.wand.Rebuild(); 
        }
        else
        {
            if (slotIndex >= 0) //We are a wand slot
            {
                
                SpellItemV2 ourSpell = _target;
                SpellItemV2 newSpell = MouseInventorySlotV2.main.SwapSpell(ourSpell);
                if (newSpell != null)
                {
                    newSpell.transform.SetParent(PlayerControllerV2.instance.wand.spellContainer.transform);
                }
            
                //If we're slotIndex that corresponds to a wand slot, then we Bind
                //From the Rebuild -> OnWandRebuild -> ShopItemContainer -> Bind
                PlayerControllerV2.instance.wand.spells[slotIndex] = newSpell;
                PlayerControllerV2.instance.wand.Rebuild();
            }
            else //We are a shop slot
            {
                SpellItemV2 ourSpell = _target;
                SpellItemV2 newSpell = MouseInventorySlotV2.main.SwapSpell(ourSpell);
                
                //SHOP STUFF
                // if (true)
                // {
                //     if (ourSpell != null && ShopRollerV2.Instance.gold.value < 2)
                //     {
                //         return;
                //     }
                //     
                //     if (ourSpell != null)
                //     {
                //         ShopRollerV2.Instance.ChangeGold(-2);
                //     }
                // }
                //SHOP STUFF END
                
                if(GameManager.Instance.Shop.IsActive && !GameManager.Instance.Shop.hasChosen)
                {
                    GameManager.Instance.Shop.hasChosen = true;
                    ChooseNewSpell?.Invoke(this);
                }
                if(newSpell != null)
                {
                    newSpell.transform.SetParent(transform);
                }
                Bind(newSpell);
            }
        }
    }

    void LevelUpVisuals(Transform trans)
    {
        trans.DOPunchRotation(Vector3.forward * 10, 0.5f, 10, 1);
        trans.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 1);
        trans.DOShakePosition(0.5f, 0.1f, 10, 1);
    }
    
    void MouseOverVisuals(Transform trans)
    {
        if (bgImg != null)
        {
            bgImg.transform.DORotate(Vector3.forward * 30, 0.1f).SetEase(Ease.OutBack);
        }
        trans.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutBack);
        //trans.DOMoveY(orgPos.y + 0.5f, 0.1f).SetEase(Ease.OutBack);// Not Working
    }
    
    void MouseOverVisualsBack(Transform trans)
    {
        if (bgImg != null)
        {
            bgImg.transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack);
        }
        //trans.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack);
        
        trans.DOScale(Vector3.one , 0.1f).SetEase(Ease.OutBack);
        //trans.DOMoveY( orgPos.y, 0.1f).SetEase(Ease.OutBack);// Not Working
    }

    public void FlashText(TextMeshProUGUI textComponent, Color endColor, float duration, int numFlashes)
    {
        // Animate the color from startColor to endColor and back
        textComponent.DOColor(endColor, duration).SetLoops(numFlashes*2, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //LevelUpVisuals(gameObject.transform);
        MouseOverVisuals(gameObject.transform);
        toolTip.gameObject.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        
        MouseOverVisualsBack(gameObject.transform);
        //LevelUpVisuals(gameObject.transform);
        toolTip.gameObject.SetActive(false);
    }

}

