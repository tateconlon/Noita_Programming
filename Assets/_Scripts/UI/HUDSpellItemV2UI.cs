using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HUDSpellItemV2UI : MonoBehaviour, IBindable<SpellItemV2>
{
    [CanBeNull, ReadOnly, ShowInInspector, NonSerialized] private SpellItemV2 _target;
    [CanBeNull] public SpellItemV2 BoundTarget { get; }

    [SerializeField] private int slotIndex = -1;
    
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Image icon;

    public void Start()
    {
        Bind(null);
        WandV2.OnWandRebuild += Bind_OnWandRebuild;
        if (PlayerControllerV2.instance != null && PlayerControllerV2.instance.wand != null)
        {
            Bind_OnWandRebuild();
        }
    }

    public void OnDestroy()
    {
        WandV2.OnWandRebuild -= Bind_OnWandRebuild;
    }

    void Bind_OnWandRebuild()
    {
        if (slotIndex >= 0 && slotIndex < PlayerControllerV2.instance.wand.spells.Count)
        {
            Bind(PlayerControllerV2.instance.wand.spells[slotIndex]);
        }
    }

    public void Bind([CanBeNull] SpellItemV2 target)
    {
        _target = target;

        if (_target == null)
        {
            icon.color = Color.clear;
        }
        else
        {
            icon.color = Color.white;
            icon.sprite = _target.uiInfo.icon;
        }
    }
}