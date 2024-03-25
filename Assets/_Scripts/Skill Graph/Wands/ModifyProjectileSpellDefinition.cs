using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Modify Projectile Spell Definition", menuName = "ScriptableObject/Wand System/Spell Definition/Modify Projectile", order = 0)]
public class ModifyProjectileSpellDefinition : SpellDefinition
{
    [SerializeField] public List<AbilityReference> launcherMods = new();
    [SerializeField] public List<AbilityReference> projectileMods = new();
    
    public override string SpellTypeDisplayName => "Modifier";
}