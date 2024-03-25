using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MouseInventorySlotV2 : MonoBehaviour, IBindable<SpellItemV2>
{
    [CanBeNull] [NonSerialized, ShowInInspector] private SpellItemV2 _target;
    [CanBeNull] public SpellItemV2 BoundTarget => _target;
    public static MouseInventorySlotV2 main { get; private set; }

    
    [ShowInInspector] public bool isHolding => _target != null;

    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Image _icon;
    
    void OnEnable()
    {
        //Singleton pattern
        if(main == null)
        {
            main = this;
        }
        Bind(_target);
    }

    private void OnDisable()
    {
        main = null;
    }
    
    [CanBeNull]
    public SpellItemV2 SwapSpell([CanBeNull] SpellItemV2 newSpell)
    {
        SpellItemV2 oldSpell = _target;

        if (newSpell != null)
        {
            newSpell.transform.SetParent(transform);
            newSpell.transform.localPosition = Vector3.zero;
        }
        main.Bind(newSpell);
        return oldSpell;
    }

    //Might not want to use Bind, instead, a reset visuals.
    //But let's try it this way for now
    //Reset Visuals works in MouseInventorySlot.cs
    public void Bind(SpellItemV2 target)
    {
        _target = target;
        if (target == null)
        {
            _icon.sprite = null;
            _icon.color = Color.clear;
        }
        else
        {
            _icon.sprite = target.uiInfo.icon;
            _icon.color = Color.white;
        }
    }
}