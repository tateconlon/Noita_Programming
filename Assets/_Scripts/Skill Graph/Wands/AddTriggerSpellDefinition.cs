using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// See <a href="https://noita.fandom.com/wiki/Add_Trigger">Noita's "Add Trigger" spells</a>
/// </summary>
[CreateAssetMenu(fileName = "Add Trigger Spell Definition", menuName = "ScriptableObject/Wand System/Spell Definition/Add Trigger", order = 0)]
public class AddTriggerSpellDefinition : SpellDefinition
{
    [SerializeField] public List<AbilityReference> launcherMods = new();
    [SerializeField] public List<AbilityReference> projectileMods = new();
    
    [Header("Modifications to Child Casting Block")]
    [SerializeField] public List<AbilityReference> childLauncherMods = new();
    [SerializeField] public List<AbilityReference> childProjectileMods = new();
    
    [Header("Runtime Debug")]
    [NonSerialized, ShowInInspector]
    private HashSet<Actor> _projectilesWithTrigger = new();
    
    public override string SpellTypeDisplayName => "Trigger";
    
    public event Action<SpellProjectile> OnAddProjectile;
    public event Action<SpellProjectile> OnRemoveProjectile;

    public void AddToProjectile(SpellProjectile spellProjectile)
    {
        if (_projectilesWithTrigger.Contains(spellProjectile))
        {
            //The trigger has already been added. Duplicates work, since the projectile's
            //TriggerCastingBlocks is a dictionary of a trigger to a LIST of castingblocks.
            //This check just makes sure we don't add the projectile twice to the trigger manager
            return;
        }

        _projectilesWithTrigger.Add(spellProjectile);
        spellProjectile.OnDisabled += RemoveFromProjectile;
        
        OnAddProjectile?.Invoke(spellProjectile);
    }
    
    public void RemoveFromProjectile(SpellProjectile spellProjectile)
    {
        if (!_projectilesWithTrigger.Contains(spellProjectile))
        {
            Debug.LogException(new KeyNotFoundException($"{spellProjectile.name} not found in {name}"), this);
            return;
        }
        
        _projectilesWithTrigger.Remove(spellProjectile);
        spellProjectile.OnDisabled -= RemoveFromProjectile;
        
        OnRemoveProjectile?.Invoke(spellProjectile);
    }
    
    public bool IsAddedToProjectile(Actor actor, out SpellProjectile spellProjectile)
    {
        if (_projectilesWithTrigger.Contains(actor))
        {
            spellProjectile = actor as SpellProjectile;
            return true;
        }
        
        spellProjectile = null;
        return false;
    }
}