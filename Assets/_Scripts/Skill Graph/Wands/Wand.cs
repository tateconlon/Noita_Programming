using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Wand : Actor
{
    public static Wand equippedWand;

    [Header("Starting Spells")]
    //Very difficult to load starting Spells through the _rootCastingBlock Inspector
    [SerializeReference] public SpellDefinition[] startingSpells = 
        new SpellDefinition[CastingBlock.DEFAULT_CASTING_BLOCK_SIZE];  // SerializeReference so elements can be null
    
    [SerializeField, Required] private Timer _castDelayTimer;
    [SerializeField, Required] private Timer _rechargeTimer;
    
   
    [Header("Feedbacks")]
    [SerializeField, Required] private MMF_Player _castFx;
    
    [Header("Casting Blocks Debug")]
    [NonSerialized] public CastingBlock _rootCastingBlock;
    [ShowInInspector] private int _castingBlockIndex = 0;
    [ShowInInspector] public Dictionary<SpellItem, CastingBlock> _spellsToCastingBlocks = new();
    
    [ShowInInspector] public bool IsReadyToCast => _castDelayTimer.IsTimedOut && _rechargeTimer.IsTimedOut;
    public event Action OnReadyToCast;

    public static event Action<Wand> OnChange;
    
    protected void OnEnable()
    {
        //base.OnEnable();

        //Init _rootCastingBlock
        //These are unassignable via Inspector, so we do it here
        _rootCastingBlock = new CastingBlock(null, startingSpells.Length);
        for (int i = 0; i < startingSpells.Length ; i++)
        {
            if (startingSpells[i] == null)
            {
                _rootCastingBlock.InsertSpell(i, null);
                continue;
            }
            
            _rootCastingBlock.InsertSpell(i, new SpellItem(startingSpells[i]));
        }
        
        _castDelayTimer.OnTimeout += RefreshIsReadyToCast;
        _rechargeTimer.OnTimeout += RefreshIsReadyToCast;
        
        //Singleton Behaviour
        equippedWand = this;
        OnChange?.Invoke(equippedWand);
    }
    
    public bool TryCast(TargetData targetData)
    {
        if (!IsReadyToCast) return false;

        _rootCastingBlock.Cast(targetData);
        _castFx.PlayFeedbacks();
        
        _castingBlockIndex += 1;
        _castDelayTimer.Restart();
        
        Recharge();
        
        return true;
    }

    //Must be called when CastingBlock changes to propogate changes to UI
    //(or whatever else is listening)
    public void RefreshSpells()
    {
        OnChange?.Invoke(this);
    }

    public bool TryAddSpell(SpellItem newSpell)
    {
        bool success = _rootCastingBlock.TryAddSpell(newSpell);
        if (success)
        {
            RefreshSpells();
        }

        return success;
    }

    private void Recharge()
    {
        _castingBlockIndex = 0;
        
        _rechargeTimer.Restart();
    }
    
    private void RefreshIsReadyToCast()
    {
        if (IsReadyToCast)
        {
            OnReadyToCast?.Invoke();
        }
    }
    
    private void OnDisable()
    {
        _castDelayTimer.OnTimeout -= RefreshIsReadyToCast;
        _rechargeTimer.OnTimeout -= RefreshIsReadyToCast;
        
        equippedWand = null;
        OnChange?.Invoke(equippedWand);
    }
}