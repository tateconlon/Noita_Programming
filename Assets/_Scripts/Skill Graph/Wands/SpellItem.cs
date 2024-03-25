using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// An inventory instance of a spell
/// </summary>
[Serializable]
public class SpellItem
{
    [SerializeField, Required] private SpellDefinition _definition;
    
    // TODO: StatsHolder stats;
    
    //This will change if spells become more composed. eg: A tier 3 projectile that has a trigger behaviour
    public bool hasChildren
    {
        get { return _definition is AddTriggerSpellDefinition; }
    }
    
    [CanBeNull, NonSerialized, ShowInInspector]
    public CastingBlock triggeredCastingBlock;    //Used to create casting blocks from a projectile mod
    [CanBeNull, NonSerialized, ShowInInspector]
    public CastingBlock currCastingBlock;
    public int currCastingBlockIndex
    {
        get
        {
            if (currCastingBlock != null)
            {
                for (int i = 0; i < currCastingBlock.getSpells().Length; i++)
                {
                    if (currCastingBlock.getSpells()[i] == this) return i;
                }
            }

            return -1;
        }
    }
    
    
    public SpellDefinition definition => _definition;
    
    
    public SpellItem(SpellDefinition spellDef)
    {
        _definition = spellDef;
        currCastingBlock = null;
        
        if (_definition is AddTriggerSpellDefinition)
        {
            triggeredCastingBlock = new CastingBlock( this);
        }
    }
    
    public string GetToolTip()
    {
        return $"{definition.displayName}\n" +
               $"[{definition.SpellTypeDisplayName}]\n" +
               $"{definition.description}";
    }

    public void ClearCastingBlocks()
    {
        if (triggeredCastingBlock != null)
        {
            CastingBlock oldCB = triggeredCastingBlock;
            triggeredCastingBlock = new CastingBlock(this, oldCB.Spells.Length);
        }
        
        currCastingBlock = null;
    }

    /// <summary>
    /// Depth first walk of the tree
    ///This may fail if a SpellItem somehow points back to itself.
    ///I've included the depth check with an error message to help with that
    /// </summary>
    /// <param name="includeThis"></param>
    /// <returns>All descendent items of this spell</returns>
    public List<SpellItem> GetListOfAllContainedSpells(bool includeThis = false)
    {
        List<SpellItem> flattenedTree = new();
        if (includeThis)
        {
            flattenedTree.Add(this);
        }
        
        GetListOfAllContainedSpells(this, flattenedTree, 100);

        return flattenedTree;
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