using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackHoleSpellItem : SpellItemV2
{
    private const int NumEnemiesInsidePerTrigger = 3;
    public override int maxNumSpells => 1;
    
    //TATE TODO: This should maybe just stay in code?
    [SerializeField] private StatChange[] nextWeaponBuffs;

    [SerializeField] private BlackHoleProjectile projPrefab;
    
    //These use getters so we can tweak them live via HotReload
    private int manaCost => 20;
    private float manaGen => statsMod[StatType.ManaRegen].Modify(1);
    private float manaCurr = 0;
    
    public override float UI_manaCost => manaCost;
    public override float UI_manaGen => manaGen;
    public override float UI_manaCurr => manaCurr;
    
    public override float redText => Mathf.FloorToInt(Damage);
    public override float yellowText => LifeTime;
    public override float blueText => LifeTime;
    public override StatType yellowStatType => StatType.LifeTime;


    public int wandSlot => PlayerControllerV2.instance.wand.spells.IndexOf(this);
    public WandV2 wand => PlayerControllerV2.instance.wand;
    public PlayerControllerV2 player => PlayerControllerV2.instance;

    //These could be put in a SO
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_damage = 2;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_numOfProjectiles = 1;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_size = 4;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_spread = 10;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_knockback = 2;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_projectileSpeed = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_bounce = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_piercing = 0;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_inaccuracy = 10f;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_lifetime = 2.0f;
    
    [SerializeField] private int level = 1;
    
    //TODO TATE: Not sure if needed
    
    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.FloorToInt(Mathf.Max(1, statsMod[StatType.Yellow].Join(statsMod[StatType.Projectiles]).Modify(b_numOfProjectiles)));
    public float Size => statsMod[StatType.ProjectileSize].Modify(b_size);  //old yellow
    public float LifeTime => statsMod[StatType.Yellow].Join(statsMod[StatType.LifeTime]).Modify(b_lifetime);
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);
    public float Inaccuracy => b_inaccuracy;

    private void OnEnable()
    {
        //BlackHoleProjectile.OnExpire += OnProjectileExpire;
        BlackHoleProjectile.OnDamage += OnProjectileDamage;
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
        Shoot(projectileRecipe, castCommand, NumOfProjectiles, Spread, b_inaccuracy);
        return true;
    }

    
    public override void Shoot(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float spread, float inaccuracy)
    {
        //this.PostNotification(TweakProjectileRecipe, recipe);
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
                BlackHoleProjectile projScript = proj.GetComponent<BlackHoleProjectile>();
                projScript.Init(recipe, direction);
            }
        }
        else
        {
            GameObject proj = Instantiate(projPrefab.gameObject, castCommand.Pos, Quaternion.identity);
            BlackHoleProjectile projScript = proj.GetComponent<BlackHoleProjectile>();
            projScript.Init(recipe, pointDirection);
        }
        InvokeOnShoot(this);
    }

    
    public override void FirstPass()
    {
        for (int i = wandSlot + 1; i < wand.spells.Count && triggeredSpells.Count < maxNumSpells; i++)
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
    
    //NOTE: This is out of date but kept here since the code might be useful later
    public void OnProjectileExpire(BlackHoleProjectile.ExpireEventData expireEventData)
    {
        if (expireEventData.caster == null || expireEventData.caster != this || triggeredSpells.Count == 0) return;
        
        int numCasts = expireEventData.enemiesInside.Count / NumEnemiesInsidePerTrigger;

        for (int i = 0; i < expireEventData.enemiesInside.Count; i++)
        {
            foreach (SpellItemV2 triggeredSpell in triggeredSpells.Where(spell => spell != null))
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                
                triggeredSpell.TryCast(new CastCommand()
                {
                    Pos = expireEventData.projectile.transform.position,
                    Dir = randomDir
                }, manaGen);
            }
        }
    }
    
    
    public void OnProjectileDamage(BlackHoleProjectile.DamageEventData dmgEventData)
    {
        if (dmgEventData.caster == null || dmgEventData.caster != this || triggeredSpells.Count == 0) return;

        foreach (SpellItemV2 triggeredSpell in triggeredSpells.Where(spell => spell != null))
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
                
            triggeredSpell.TryCast(new CastCommand()
            {
                Pos = dmgEventData.projectile.transform.position,
                Dir = randomDir
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
            size = Size,
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
        //BlackHoleProjectile.OnExpire -= OnProjectileExpire;
        BlackHoleProjectile.OnDamage -= OnProjectileDamage;
    }
    
    public override string GetTooltip()
    {
        return $"Pulls Enemies and Pickups towards its center, periodically dealing damage\n" +
               $"Gives {manaGen} mana when it deals damage to an enemy";
    }
}
