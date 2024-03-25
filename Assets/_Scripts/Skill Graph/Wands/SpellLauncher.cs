using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpellLauncher : Actor
{
    [SerializeField, Required] private ParticleSystemPrefabSpawner _spawner;
    [SerializeField] public int count;
    [SerializeField] private AimAtClosestTag _aimer;
    [SerializeField] private bool _aimAtClosestEnemy;
    
    // TODO: each launcher could also have personalized feedbacks, e.g. muzzle flash, particles, SFX
    public void Launch(LaunchProjectileSpellDefinition spell, TargetData targetData, CastingBlock castingBlock)
    {
        if (spell.projectilesIgnorePrevTarget)
        {
            // A bit hacky for this one-off use case, but this will ignore all targets
            targetData.Ignored.UnionWith(targetData.Targets);
            targetData.Targets.Clear();
        }
        
        Aim(targetData);
        
        SpawnProjectiles(spell, targetData, castingBlock);
        
        gameObject.SetActive(false);
    }

    private void SpawnProjectiles(LaunchProjectileSpellDefinition spell, TargetData targetData, CastingBlock castingBlock)
    {
        bool ignorePrevTarget = spell.projectilesIgnorePrevTarget && targetData.Ignored.Count > 0;
        
        // Note: if you want to spawn Actors over time, you can make ParticleSystemPrefabSpawner.Spawn a coroutine
        // or something that plays the particle system for a certain amount of time so you can create abilities like
        // spawning a circle of knives in a swirl around the player over 1 second
        foreach (SpellProjectile projectile in _spawner.Spawn(spell.projectilePrefab, count))
        {
            projectile.Parent = castingBlock.Owner;
            
            // E.g. give the bullet +1 pierce and +1 damage
            foreach (Ability projectileMod in castingBlock.ProjectileMods)
            {
                //castingBlock.Owner.Abilities.TryActivateAbility(projectileMod, new TargetData(projectile));
            }
            
            // Register this projectile to be tracked by SpellTriggerManagers
            foreach (SpellItem _spell in castingBlock.getSpells())
            {
                if (_spell == null) continue;
                
                if(_spell.definition is AddTriggerSpellDefinition projectileTrigger)
                {
                    projectileTrigger.AddToProjectile(projectile);
                    //Add the spell's casting block as the trigger
                    bool triggerNotSetOrIsNull = !projectile.TriggerCastingBlocks.ContainsKey(projectileTrigger)
                                                 || projectile.TriggerCastingBlocks[projectileTrigger] == null;
                    
                    
                    if (triggerNotSetOrIsNull)
                    {
                        projectile.TriggerCastingBlocks[projectileTrigger] = new();
                    }

                    projectile.TriggerCastingBlocks[projectileTrigger].Add(_spell.triggeredCastingBlock);
                }
            }

            if (ignorePrevTarget)
            {
                IgnorePrevTarget(projectile, targetData.Ignored);
            }
        }
    }
    
    // TODO: this method could be moved into SpellProjectile and handled on a case-by-case basis there
    private void IgnorePrevTarget(SpellProjectile projectile, ICollection<Actor> ignored)
    {
        if (projectile.TryGetComponentInChildren(out AbilityTriggerOnTouch triggerOnTouch))
        {
            foreach (Actor ignore in ignored)
            {
                triggerOnTouch.IgnoreActor(ignore);
            }
        }
        
        if (projectile.TryGetComponentInChildren(out NavToClosestTag navToClosestTag))
        {
            navToClosestTag.FindTarget(ignored: ignored);
        }
    }
    
    // TODO: by default all triggers will pass a TargetData.Direction value. Once there are mod spells to customize
    // targeting behavior, this temp code will have to change
    private void Aim(TargetData targetData)
    {
        if (targetData.Direction.HasValue)
        {
            transform.right = targetData.Direction.Value;
        }
        else if (_aimAtClosestEnemy)
        {
            _aimer.AimAtClosest(ignored: targetData.Ignored);
        }
    }
}