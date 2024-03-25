using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Launch Projectile Spell Definition", menuName = "ScriptableObject/Wand System/Spell Definition/Launch Projectile", order = 0)]
public class LaunchProjectileSpellDefinition : SpellDefinition
{
    [Tooltip("E.g. represents a shotgun with attributes representing spread, recoil, number of projectiles, etc.")]
    [SerializeField, Required] public SpellLauncher launcherPrefab;
    
    [Tooltip("E.g. represents a bullet with attributes representing damage, speed, piercing, etc.")]
    [SerializeField, Required] public SpellProjectile projectilePrefab;
    
    [Tooltip("E.g. if a pistol bullet hitting an enemy triggers another pistol shot, it shouldn't hit the same enemy")]
    [SerializeField] public bool projectilesIgnorePrevTarget = false;
    
    public override string SpellTypeDisplayName => "Attack";
    
}