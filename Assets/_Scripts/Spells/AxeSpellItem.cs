using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class AxeSpellItem : SpellItemV2
{
    private const int NumPiercesPerTrigger = 2;
    public override int maxNumSpells => 1;
    
    [SerializeField] private StatChange[] nextWeaponBuffs;
    
    [SerializeField] private AxeProjectile projPrefab;
    
    //These use getters so we can tweak them live via HotReload
private int manaCost => 20;
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
    [SerializeField] private float b_damage = 5;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_numOfProjectiles = 3;
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
    [SerializeField] private int b_piercing = 3;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_inaccuracy = 10f;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_lifetime = 1.5f;
    
    [SerializeField] private int level = 1;
    
    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.FloorToInt(Mathf.Max(1, statsMod[StatType.Yellow].Join(statsMod[StatType.Projectiles]).Modify(b_numOfProjectiles)));
    public float LifeTime => statsMod[StatType.Blue].Modify(statsMod[StatType.LifeTime].Modify(b_lifetime));
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);
    
    private void OnEnable()
    {
        AxeProjectile.PierceEvent += OnPierceEvent;
    }
    
    private void OnPierceEvent(AxeProjectile.PierceEventData pierceEventData)
    {
        if (pierceEventData.caster == null || pierceEventData.caster != this || triggeredSpells == null) return;
        if (pierceEventData.piercesUsed % NumPiercesPerTrigger != 0) return;  // Only trigger every x pierces
        
        // The axe's movement is mostly vertical (shooting up then plummeting down) so let's shoot triggered spells
        // horizontally off to either side to give strong area coverage. Alternate left and right each trigger.
        int numTriggers = pierceEventData.piercesUsed / NumPiercesPerTrigger;
        Vector2 newDir = new(numTriggers % 2 == 0 ? -1.0f : 1.0f, Random.Range(0.05f, 0.25f));

        foreach (SpellItemV2 spell in triggeredSpells.Where(spell => spell != null))
        {
            spell.TryCast(new CastCommand()
            {
                Pos = pierceEventData.projectile.transform.position,
                Dir = newDir,
                PrevHits = new List<GameObject> { pierceEventData.hitObject }
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
        //Instead of calling shoot multiple times, we stack it into one big shoot w/ multiple projectiles
        Shoot(projectileRecipe, castCommand, NumOfProjectiles * numCasts, Spread, b_inaccuracy);
        return true;
    }
    
    public override void Shoot(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float spread, float inaccuracy)
    {
        Vector2 pointDirection = ((Vector2)castCommand.Dir).RandomizeDirection(inaccuracy); //Adds flavour
        
        if (numProjectiles > 1)
        {
            float num = -1f * (spread / 2f);
            for (int i = 0; i < numProjectiles; i++)
            {
                float degrees = num + (float)i / (float)(numProjectiles - 1) * spread;
                Vector2 direction = pointDirection.Rotate(degrees);
                
                GameObject proj = Instantiate(projPrefab.gameObject, castCommand.Pos, Quaternion.identity);
                AxeProjectile projScript = proj.GetComponent<AxeProjectile>();
                projScript.Init(recipe, direction, castCommand.PrevHits);
            }
        }
        else
        {
            GameObject proj = Instantiate(projPrefab.gameObject, castCommand.Pos, Quaternion.identity);
            AxeProjectile projScript = proj.GetComponent<AxeProjectile>();
            projScript.Init(recipe, pointDirection, castCommand.PrevHits);
        }
        
        if (numProjectiles > 0)
        {
            InvokeOnShoot(this);
        }
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
    
    public override ProjectileRecipe GetProjectileRecipe()
    {
        ProjectileRecipe projectileRecipe = new ProjectileRecipe();
        
        projectileRecipe.damage = Damage;
        projectileRecipe.spawnID = Guid.NewGuid().ToString();
        projectileRecipe.projectileSpeed = statsMod[StatType.ProjectileSpeed].Modify(b_projectileSpeed);
        projectileRecipe.size = statsMod[StatType.ProjectileSize].Modify(b_size);
        projectileRecipe.knockback = statsMod[StatType.Knockback].Modify(b_knockback);
        projectileRecipe.bounce = Mathf.Max(0, (int)statsMod[StatType.Bounce].Modify(b_bounce));
        projectileRecipe.piercing = Mathf.Max(0, (int)statsMod[StatType.Piercing].Modify(b_piercing));
        projectileRecipe.owner = player.gameObject;
        projectileRecipe.lifetime = LifeTime;
        projectileRecipe.ownerSpell = this;
        return projectileRecipe;
    }
    
    private void OnDisable()
    {
        AxeProjectile.PierceEvent -= OnPierceEvent;
    }
    
    public override string GetTooltip()
    {
        return $"Throws {NumOfProjectiles} axe\nPierces {statsMod[StatType.Piercing].Modify(b_piercing)}x" +
               $"\nGives {manaGen} mana every {NumPiercesPerTrigger} Pierces";
    }
}