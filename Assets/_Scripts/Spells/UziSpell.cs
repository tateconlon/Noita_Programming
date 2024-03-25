using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class UziSpell : SpellItemV2
{
    public override int maxNumSpells => 1;

    [SerializeField] private UziProjectile projPrefab;
    
    //These use getters so we can tweak them live via HotReload
    private int manaCost => 10;
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
    [SerializeField] private int b_numOfProjectiles = 10;
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_size = 3;
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
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private float b_triggerChance = 0.5f;
    
    [SerializeField] private int level = 1;
    
    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.FloorToInt(Mathf.Max(1, statsMod[StatType.Yellow].Join(statsMod[StatType.Projectiles]).Modify(b_numOfProjectiles)));
    public float LifeTime => statsMod[StatType.Blue].Modify(statsMod[StatType.LifeTime].Modify(b_lifetime));
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);

    private void OnEnable()
    {
        UziProjectile.ImpactEvent += OnHitTriggered;
    }

    private void OnHitTriggered(UziProjectile.ImpactEventData impactEventData)
    {
        if (impactEventData.caster != this || !impactEventData.projectile.isTriggerable) return;
        if (!RandomExtensions.Bool(b_triggerChance)) return;  // Rely on random chance to trigger or not
        
        foreach (SpellItemV2 spell in triggeredSpells)
        {
            if (spell != null)
            {
                spell.TryCast(new CastCommand()
                {
                    Pos = impactEventData.projectile.transform.position,
                    Dir = impactEventData.projectile.dir,
                    PrevHits = new List<GameObject> { impactEventData.hitObject }
                }, manaGen);
            }
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
        StartCoroutine(UziShootCR(recipe, castCommand, numProjectiles, inaccuracy));
    }

    //Shoots in circles, but the more projectiles wider the angle (can do multiple circles)
    IEnumerator UziShootCR(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float inaccuracy)
    {
        float spawnRadius = 0.75f;
        // float totalTime = 0.25f;
        float delayTime = 0.05f; //Constant delay feels more right, more projectiles will result in a longer spiral, v cool//totalTime / numProjectiles;
        int numProjPerCircle = 12;

        float gapAngleDeg = 360f / numProjPerCircle;
        float totalAngleDeg = gapAngleDeg * (numProjectiles - 1); //number of gaps is numProjectiles - 1
        float angleOffsetDeg = Vector2.right.GetSignedAngle(castCommand.Dir) - totalAngleDeg / 2f;   //Offset the angle so that the projectiles are centered around the dir
        //angleOffsetDeg += Vector2.right.GetSignedAngle(dir) - totalAngleDeg / 2f;   //Offset the angle so that the projectiles are centered around the dir

        
        //If there's an even number of projectiles, the uzi won't hit where the cursor is.
        //Looks weird with 6 < numProjectiles, but fine with enough projectiles
        if (numProjectiles % 2 == 0)
        {
            angleOffsetDeg += gapAngleDeg / 2;  
        }
        
        if (numProjectiles > 0)
        {
            InvokeOnShoot(this);
        }
        
        for (int i = 0; i < numProjectiles; i++)
        {
            // Calculate the angle for each projectile, in deg
            float angleDeg = i * 360f / numProjPerCircle + angleOffsetDeg;

            // 2nd circle, the projectiles go in the gaps of the previous circle (looks cooler)
            int ringNum = Mathf.FloorToInt(i / numProjPerCircle);
            if (ringNum % 2 == 1)
            {
                angleDeg += gapAngleDeg / 2;  //Add an offset that's half of the gap angle
            }

            Vector3 spawnPosition = new Vector3(
                Mathf.Cos(angleDeg * Mathf.Deg2Rad) * spawnRadius, 
                Mathf.Sin(angleDeg * Mathf.Deg2Rad) * spawnRadius,
                0
            ) + castCommand.Pos;

            UziProjectile projGO = Instantiate(projPrefab, spawnPosition, Quaternion.identity);
            Vector2 dir = Vector2.right.Rotate(angleDeg);
            dir = dir.RandomizeDirection(inaccuracy);   //for flavor
            projGO.Init(recipe, dir, castCommand.PrevHits);
            
            yield return new WaitForSeconds(delayTime);
        }
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

    //These values can be in a SO
    public override ProjectileRecipe GetProjectileRecipe()
    {
        ProjectileRecipe projectileRecipe = new ProjectileRecipe();
        //projectileRecipe.objectPoolTag = gunData.bulletOPTag; from other game
        
        //example of tweaking projectile recipe
        // if (gunData.isSummonGun)
        // {
        //     projectileRecipe.b_damage = stats[StatType.SummonDamage].Modify(stats[StatType.BulletDamage].Modify(gunData.b_damage));
        // }
        // else
        // { ... }
        
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
        UziProjectile.ImpactEvent -= OnHitTriggered;
    }
    
    public override string GetTooltip()
    {
        return $"Sprays {NumOfProjectiles} bullets in a circle.\n{b_triggerChance:P0} chance to give {manaGen} mana when hitting an Enemy";
    }
}
