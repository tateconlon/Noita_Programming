using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CastingBlock
{
    //This is a hack that resolves instantiating race conditions, 
    //but should always be what we want anyways
    public Wand Owner => Wand.equippedWand;

    public const int DEFAULT_CASTING_BLOCK_SIZE = 4;
    public List<LaunchProjectileSpellDefinition> LaunchProjectileSpells {
        get
        {
            List<LaunchProjectileSpellDefinition> retVal = new();

            foreach (SpellItem spell in Spells)
            {
                if (spell == null) continue;
                
                if(spell.definition is LaunchProjectileSpellDefinition def)
                {
                    retVal.Add(def);
                }
            }
            
            return retVal;
        }
    }
    public List<ModifyProjectileSpellDefinition> ModifyProjectileSpells 
    {
        get
        {
            List<ModifyProjectileSpellDefinition> retVal = new();

            foreach (SpellItem spell in Spells)
            {
                if (spell == null) continue;
                
                if (spell.definition is ModifyProjectileSpellDefinition def)
                {
                    retVal.Add(def);
                }
            }

            return retVal;
        }
    }

    public List<Ability> LauncherMods
    {
        get
        {
            List<Ability> retVal = new();

            foreach (SpellItem spell in Spells)
            {
                if (spell == null) continue;
                
                if (spell.definition is ModifyProjectileSpellDefinition modDef)
                {
                    retVal.AddRange(modDef.launcherMods.Select(abilityRef => abilityRef.Value));
                }
                
                if (spell.definition is AddTriggerSpellDefinition trigDef)
                {
                    retVal.AddRange(trigDef.launcherMods.Select(abilityRef => abilityRef.Value));
                }
            }
            
            if (ParentSpell != null)
            {
                //Note: We don't check all the childLauncherMods of all Spells in the parent CastingBlock
                //because then we would get childLauncherMods of other triggers. We have no way to resolve this yet.
                //For now we assume that we just want the childLauncherMods of the parent trigger spell.
                if (ParentSpell.definition is AddTriggerSpellDefinition trigDef)
                {
                    retVal.AddRange(trigDef.childLauncherMods.Select(abilityRef => abilityRef.Value));
                }
            }

            return retVal;
        }
    }
    
    public List<Ability> ProjectileMods 
    {
        get
        {
            List<Ability> retVal = new();

            foreach (SpellItem spell in Spells)
            {
                if (spell == null) continue;
                
                if (spell.definition is ModifyProjectileSpellDefinition modDef)
                {
                    retVal.AddRange(modDef.projectileMods.Select(abilityRef => abilityRef.Value));
                }
                
                //This is if a trigger also has a mod to the current block
                if (spell.definition is AddTriggerSpellDefinition trigDef)
                {
                    retVal.AddRange(trigDef.projectileMods.Select(abilityRef => abilityRef.Value));
                }
            }

            if (ParentSpell != null)
            {
                //Note: We don't check all the childProjectileMods of all Spells in the parent CastingBlock
                //because then we would get childProjectileMods of other triggers. We have no way to resolve this yet.
                //For now we assume that we just want the childProjectileMods of the parent trigger spell.
                if (ParentSpell.definition is AddTriggerSpellDefinition trigDef)
                {
                    retVal.AddRange(trigDef.childProjectileMods.Select(abilityRef => abilityRef.Value));
                }
            }

            return retVal;
        }
    }
    
    //We want to know all the triggers in this casting block so we can tell the projectiles from this casting block
    //to attach these triggers.
    public List<AddTriggerSpellDefinition> ProjectileTriggers 
    {
        get
        {
            List<AddTriggerSpellDefinition> retVal = new();

            foreach (SpellItem spell in Spells)
            {
                if (spell == null) continue;
                
                if(spell.definition is AddTriggerSpellDefinition def)
                {
                    retVal.Add(def);
                }
            }
            
            return retVal;
        }
    }
    
    [NonSerialized, ShowInInspector]
    public readonly SpellItem ParentSpell = null;
    
    public SpellItem[] Spells = new SpellItem[DEFAULT_CASTING_BLOCK_SIZE];

    public SpellItem[] getSpells()
    {
        return Spells;
    }

    public CastingBlock(SpellItem _parentSpell, int numSpellSlots = DEFAULT_CASTING_BLOCK_SIZE)
    {
        ParentSpell = _parentSpell;
        Spells = new SpellItem[numSpellSlots];
    }

    public bool IsValid()
    {
        return LaunchProjectileSpells.Count > 0;
    }
    
    /// <returns>What used to be at that index, possibly null.</returns>
    [CanBeNull]
    public SpellItem InsertSpell(int index, [CanBeNull] SpellItem newSpell)
    {
        SpellItem retVal = Spells[index];
        Spells[index] = newSpell;

        if (newSpell != null)
        {
            newSpell.currCastingBlock = this;
        }

        return retVal;
    }

    public bool TryAddSpell(SpellItem newSpell)
    {
        return TryAddSpell(newSpell, this);
    }

    private bool TryAddSpell(SpellItem newSpell, CastingBlock cb)
    {
        for (int i = 0; i < cb.Spells.Length; i++)
        {
            SpellItem spell = cb.Spells[i];
            if (spell == null)
            {
                cb.InsertSpell(i, newSpell);
                return true;
            }
        }
        
        for (int i = 0; i < cb.Spells.Length; i++)
        {
            SpellItem spell = cb.Spells[i];
            if (spell.triggeredCastingBlock != null)
            {
                if (TryAddSpell(newSpell, spell.triggeredCastingBlock))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public void Cast(TargetData targetData)
    {
        if (!IsValid()) return;

        foreach (LaunchProjectileSpellDefinition projectileSpell in LaunchProjectileSpells)
        {
            Vector3 launcherPos = targetData.Origin.HasValue ? targetData.Origin.Value : Owner.transform.position;
            
            SpellLauncher launcher = LeanPool.Spawn(projectileSpell.launcherPrefab, launcherPos, Quaternion.identity);
            
            // E.g. give the shotgun +2 projectiles and +15 spread
            foreach (Ability launcherMod in LauncherMods)
            {
                //Owner.Abilities.TryActivateAbility(launcherMod, new TargetData(launcher));
            }
            
            launcher.Launch(projectileSpell, targetData, this);
        }
    }
}