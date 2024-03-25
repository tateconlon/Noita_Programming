using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

public class SpellProjectile : Actor
{
    [NonSerialized, ShowInInspector]
    public Dictionary<AddTriggerSpellDefinition, List<CastingBlock>> TriggerCastingBlocks = new();

    public event Action<SpellProjectile> OnDisabled;
    
    //Called when re-entering the LEANPool
    private void OnDisable()
    {
        TriggerCastingBlocks.Clear();
        OnDisabled?.Invoke(this);
    }
}