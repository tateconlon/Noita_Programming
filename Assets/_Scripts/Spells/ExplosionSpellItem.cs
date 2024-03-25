using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


public class ExplosionSpellItem : SpellItemV2
{
    public override int maxNumSpells => 1;
    [SerializeField] private ExplosionProjectile projPrefab;
    
    //These use getters so we can tweak them live via HotReload
    private int manaCost => 20;
    private float manaGen => statsMod[StatType.ManaRegen].Modify(10);
    private float manaCurr = 0;
    
    public override float UI_manaCost => manaCost;
    public override float UI_manaGen => manaGen;
    public override float UI_manaCurr => manaCurr;
    
    public override float redText => Mathf.FloorToInt(Damage);
    public override float yellowText => Size;
    public override float blueText => LifeTime;
    public override StatType yellowStatType => StatType.ProjectileSize;

    public int wandSlot => PlayerControllerV2.instance.wand.spells.IndexOf(this);
    public WandV2 wand => PlayerControllerV2.instance.wand;
    public PlayerControllerV2 player => PlayerControllerV2.instance;
    
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_damage = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_numOfProjectiles = 1;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_size = 1;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_spread = 10;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_knockback = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_projectileSpeed = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_bounce = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_piercing = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_inaccuracy = 10f;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_lifetime = 1f;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_triggerTimer = 0.5f;
    
    [SerializeField] private int level = 1;
    
    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.FloorToInt(Mathf.Max(1, (int)statsMod[StatType.Projectiles].Modify(b_numOfProjectiles)));
    public float LifeTime => statsMod[StatType.Blue].Modify(statsMod[StatType.LifeTime].Modify(b_lifetime)); 
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);
    public float Size => statsMod[StatType.Yellow].Join(statsMod[StatType.ProjectileSize]).Modify(b_size);

    private void OnEnable()
    {
        ExplosionProjectile.ExplodeEvent += OnExplodeTriggered;
    }
    
    private void OnDisable()
    {
        ExplosionProjectile.ExplodeEvent -= OnExplodeTriggered;
    }
    
    private void OnExplodeTriggered(ExplosionProjectile.ExplodeEventData explodeEventData)
    {
        if (explodeEventData.caster != this || !explodeEventData.projectile.isTriggerable) return;
        
        List<SpellItemV2> validSpells = triggeredSpells.Where(spell => spell != null).ToList();
        
        if (validSpells.Count > 0)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;

            // Picks a random spell and shoots it in a random direction
            validSpells.GetRandom().TryCast(new CastCommand()
            {
                Pos = explodeEventData.projectile.transform.position,
                Dir = dir
            }, manaGen);
        }
    }

    public override bool TryCast(CastCommand castCommand, float manaGen)
    {
        manaCurr += manaGen;
        if (manaCurr < manaCost) return false;
        int numCasts = 0;
        
        if (RelicsManager.instance.relics.Any(relic => relic is OverspillMana))
        {
            numCasts = Mathf.FloorToInt(manaCurr/manaCost);
            manaCurr = manaCurr % manaCost;
        }
        else
        {
            numCasts = 1;
            manaCurr = 0;
        }
        
        ProjectileRecipe projectileRecipe = GetProjectileRecipe();
        
        projectileRecipe.size *= 1 + (numCasts * .1f);
        //Instead of calling shoot multiple times, we stack it into one big shoot w/ multiple projectiles
        Shoot(projectileRecipe, castCommand, NumOfProjectiles, Spread, b_inaccuracy);
        return true;
    }


    public override void Shoot(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float spread, float inaccuracy)
    {
        for (int i = 0; i < numProjectiles; i++)
        {

            Vector3 spawnPosition = castCommand.Pos;
            if (i > 0)
            {
                spawnPosition += Random.insideUnitCircle.V3() * 0.2f;
            }
            
            GameObject proj = Instantiate(projPrefab.gameObject, spawnPosition, Quaternion.identity);
            proj.name = $"TurretProjectile";
            ExplosionProjectile projScript = proj.GetComponent<ExplosionProjectile>();
            projScript.Init(recipe, castCommand.Dir);
            projScript.isTriggerable = true;
        }
        InvokeOnShoot(this);
    }
    
    public override ProjectileRecipe GetProjectileRecipe()
    {
        ProjectileRecipe projectileRecipe = new ProjectileRecipe();

        projectileRecipe.damage = Damage;
        projectileRecipe.spawnID = Guid.NewGuid().ToString();
        projectileRecipe.projectileSpeed = statsMod[StatType.ProjectileSpeed].Modify(b_projectileSpeed);
        projectileRecipe.size = Size;
        projectileRecipe.knockback = statsMod[StatType.Knockback].Modify(b_knockback);
        projectileRecipe.bounce = Mathf.Max(0, (int)statsMod[StatType.Bounce].Modify(b_bounce));
        projectileRecipe.piercing = Mathf.Max(0, (int)statsMod[StatType.Piercing].Modify(b_piercing));
        projectileRecipe.owner = player.gameObject;
        projectileRecipe.lifetime = LifeTime;
        projectileRecipe.ownerSpell = this;
        
        return projectileRecipe;
    }

    public override string GetTooltip()
    {
        return $"Explodes\nGives {manaGen} mana on explosion";
    }

    public override void ClearPass()
    {
        triggeredSpells.Clear();
    }
    
    public override void FirstPass()
    {
        for (int i = wandSlot + 1; i < wand.spells.Count; i++)
        {
            SpellItemV2 nextSpell = wand.spells[i];
            if (nextSpell != null)
            {
                triggeredSpells.Add(nextSpell);
                if (triggeredSpells.Count == maxNumSpells)
                {
                    break;
                }
            }
        }
    }

    public override void SecondPass() { }

    public override void ThirdPass() { }

    public override void FourthPass() { }

}