using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireCrossSpellItem : SpellItemV2
{
    public override int maxNumSpells => 1;
    private const int RINGS_PER_UNIT = 4; // 4 projectiles per unit distance from center
    
    [SerializeField] private FireCrossProjectile projPrefab;

    //These use getters so we can tweak them live via HotReload
private int manaCost => 20;
    private float manaGen => statsMod[StatType.ManaRegen].Modify(5);
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
    
    [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_damage = 5;
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
    private float b_triggerTimer => 0.1f;
    
    [SerializeField] private int level = 1;
    
    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.Max(1, (int)statsMod[StatType.Projectiles].Modify(b_numOfProjectiles));
    public float LifeTime => statsMod[StatType.Yellow].Join(statsMod[StatType.LifeTime]).Modify(b_lifetime);
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);
    public float Size => statsMod[StatType.ProjectileSize].Modify(b_size);  //old yellow

    private void OnEnable()
    {
        FireCrossProjectile.TimerTriggerEvent += OnTimerTriggered;
    }
    
    private void OnTimerTriggered(FireCrossProjectile.TimerEventData timerEventData)
    {
        if (timerEventData.caster != this || !timerEventData.projectile.isTriggerable) return;
        
        List<SpellItemV2> validSpells = triggeredSpells.Where(spell => spell != null).ToList();
        
        if (validSpells.Count > 0)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
        
            // Picks a random spell and shoots it in a random direction
            validSpells.GetRandom().TryCast(new CastCommand()
            {
                Pos = timerEventData.projectile.transform.position,
                Dir = randomDir
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
        
        projectileRecipe.lifetime *= 1 + (numCasts * .1f);
        //Instead of calling shoot multiple times, we stack it into one big shoot w/ multiple projectiles
        Shoot(projectileRecipe, castCommand, NumOfProjectiles, Spread, b_inaccuracy);
        return true;
    }


    public override void Shoot(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float spread, float inaccuracy)
    {
        int numRings = Mathf.FloorToInt(recipe.size * RINGS_PER_UNIT);
        float ringDistance = recipe.size / numRings;
        int projectilesCast = numRings * 4; // 4 projectiles per ring, each at 90 deg

        //Pointing X towards direction of firing doesn't look that good, so we don't do it
        //ringRotOffset += Vector2.Angle(Vector2.right, pointDirection);
        
        //rotated by 45 degrees + random inaccuracy (for flavour)
        float ringRotOffset = Mathf.Deg2Rad * (45 + UnityEngine.Random.Range(-b_inaccuracy/2, b_inaccuracy/2));

        //Spawn root object to hold all projectiles
        GameObject projRoot = null;
        if (projectilesCast > 0)
        {
            projRoot = new GameObject();
            projRoot.name = "FireCrossProjectileRoot";
            projRoot.transform.position = castCommand.Pos;
            projRoot.transform.rotation = Quaternion.identity;
        }
        
        for (int i = 0; i < projectilesCast; i++)
        {
            int currRing = Mathf.FloorToInt(i/4) + 1;
            float distFromOrigin = ringDistance * currRing;
            
            float angle = (Mathf.Deg2Rad * 90) * i + ringRotOffset; // angle is every 90 degrees
            float x = Mathf.Cos(angle) * distFromOrigin;
            float y = Mathf.Sin(angle) * distFromOrigin;

            Vector3 spawnPosition = new Vector3(x, y, 0) + castCommand.Pos;

            GameObject proj = Instantiate(projPrefab.gameObject, spawnPosition, Quaternion.identity, projRoot.transform);
            proj.name = $"FireCrossProjectile {currRing}-{Mathf.FloorToInt(i/4)}";
            FireCrossProjectile projScript = proj.GetComponent<FireCrossProjectile>();
            projScript.Init(recipe, castCommand.Dir);
            projScript.isTriggerable = i == 0;
            projScript.triggerTimer = b_triggerTimer;   //TODO TATE: Make this a stat
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
        return $"Ignites the ground.\nGives {manaGen} every {b_triggerTimer}s";
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
    
    private void OnDisable()
    {
        FireCrossProjectile.TimerTriggerEvent -= OnTimerTriggered;
    }
}