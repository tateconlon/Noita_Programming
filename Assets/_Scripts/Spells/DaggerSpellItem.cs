using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DaggerSpellItem : SpellItemV2
{
    public override int maxNumSpells => 1;
    //TATE TODO: This should maybe just stay in code?
    [SerializeField] private StatChange[] nextWeaponBuffs;

    [SerializeField] private DaggerProjectile projPrefab;

    //These use getters so we can tweak them live via HotReload
    private int manaCost => 5;
    private float manaGen => statsMod[StatType.ManaRegen].Modify(10);
    private float manaCurr = 0;
    
    public override float UI_manaCost => manaCost;
    public override float UI_manaGen => manaGen;
    public override float UI_manaCurr => manaCurr;
    
    public override float redText => Mathf.FloorToInt(Damage);
    public override float yellowText => NumOfProjectiles;
    public override float blueText => LifeTime;
    public override StatType yellowStatType => StatType.Projectiles;
    
    public int wandSlot => PlayerControllerV2.instance.wand.spells.IndexOf(this);
    public WandV2 wand => PlayerControllerV2.instance.wand;
    public PlayerControllerV2 player => PlayerControllerV2.instance;

    //These could be put in a SO
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_damage = 3;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_numOfProjectiles = 1;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_size = 1;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_spread = 10;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_knockback = 2;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_projectileSpeed = 3;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_bounce = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_piercing = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_inaccuracy = 10f;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_lifetime = 1f;
    
    [SerializeField] private int level = 1;
    
    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.FloorToInt(Mathf.Max(1, statsMod[StatType.Yellow].Join(statsMod[StatType.Projectiles]).Modify(b_numOfProjectiles)));
    public float LifeTime => statsMod[StatType.Blue].Modify(statsMod[StatType.LifeTime].Modify(b_lifetime));
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);

    private void OnEnable()
    {
        DaggerProjectile.ExpiredEvent += OnProjectileExpired;
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
        //Instead of calling shoot multiple times, we stack it into one big shoot w/ multiple projectiles
        Shoot(projectileRecipe, castCommand, NumOfProjectiles * numCasts, Spread, b_inaccuracy);
        return true;
    }
    
    public override void Shoot(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float spread, float inaccuracy)
    {
        Vector2 pointDirection = ((Vector2)castCommand.Dir).RandomizeDirection(inaccuracy); //Adds flavour
        if (numProjectiles > 1)
        {
            //spread = Mathf.Max(spread, 5f);   from other game, not sure why. Probably just game feel
            float num = -1f * (spread / 2f);
            for (int i = 0; i < numProjectiles; i++)
            {
                float degrees = num + (float)i / (float)(numProjectiles - 1) * spread;
                Vector2 direction = pointDirection.Rotate(degrees);
                
                GameObject proj = Instantiate(projPrefab.gameObject, castCommand.Pos, Quaternion.identity);
                DaggerProjectile projScript = proj.GetComponent<DaggerProjectile>();
                projScript.Init(recipe, direction, castCommand.PrevHits);
                
            }
        }
        else
        {
            GameObject proj = Instantiate(projPrefab.gameObject, castCommand.Pos, Quaternion.identity);
            DaggerProjectile projScript = proj.GetComponent<DaggerProjectile>();
            projScript.Init(recipe, pointDirection, castCommand.PrevHits);
        }
        InvokeOnShoot(this);
    }

    
    public override void FirstPass()
    {
        for (int i = wandSlot + 1; i < wand.spells.Count; i++)
        {
            SpellItemV2 nextSpell = wand.spells[i];
            if (nextSpell != null)
            {
                nextSpell.statsMod.ApplyStatChanges(nextWeaponBuffs);
                triggeredSpells.Add(nextSpell);
                break;
            }
        }
    }
    
    public override void SecondPass() {}

    public override void ThirdPass() {}

    public override void FourthPass() {}

    public override void ClearPass()
    {
        triggeredSpells.Clear();
    }
    
    public void OnProjectileExpired(DaggerProjectile.ExpiredEventData firstBloodEventData)
    {
        if (firstBloodEventData.caster == null || firstBloodEventData.caster != this) return;

        foreach (SpellItemV2 triggeredSpell in triggeredSpells.Where(spell => spell != null))
        {
            Vector2 newDir = firstBloodEventData.projectile.dir;
        
            triggeredSpell.TryCast(new CastCommand()
            {
                Pos = firstBloodEventData.projectile.transform.position,
                Dir = newDir,
                PrevHits = new List<GameObject> { firstBloodEventData.hitObject }
            }, manaGen);
        }
    }
    
    public override ProjectileRecipe GetProjectileRecipe()
    {
        ProjectileRecipe projectileRecipe = new()
        {
            damage = statsMod[StatType.BulletDamage].Modify(b_damage),
            spawnID = Guid.NewGuid().ToString(),
            projectileSpeed = statsMod[StatType.ProjectileSpeed].Modify(b_projectileSpeed),
            size = statsMod[StatType.ProjectileSize].Modify(b_size),
            knockback = statsMod[StatType.Knockback].Modify(b_knockback),
            bounce = Mathf.Max(0, (int)statsMod[StatType.Bounce].Modify(b_bounce)),
            piercing = Mathf.Max(0, (int)statsMod[StatType.Piercing].Modify(b_piercing)),
            owner = player.gameObject,
            lifetime = statsMod[StatType.LifeTime].Modify(b_lifetime),
            ownerSpell = this
        };
        
        return projectileRecipe;
    }
    
    private void OnDisable()
    {
        DaggerProjectile.ExpiredEvent -= OnProjectileExpired;
    }
    
    public override string GetTooltip()
    {
        return $"Gives {manaGen} mana on projectile expired";
    }
}