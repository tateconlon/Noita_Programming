using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class LaserSpellItem : SpellItemV2
{
    public override int maxNumSpells => 1;
    [SerializeField] private StatChange[] nextWeaponBuffs;

    [SerializeField] private LaserProjectileTest projPrefab; //TODO TATE: Should there be a lookup manager to replace references?
    
    //These use getters so we can tweak them live via HotReload
private int manaCost => 20;
    private float manaGen => statsMod[StatType.ManaRegen].Modify(10);
    private float manaCurr = 0;
    
    public override float UI_manaCost => manaCost;
    public override float UI_manaGen => manaGen;
    public override float UI_manaCurr => manaCurr;
    
    public override float redText => Mathf.FloorToInt(Damage);
    public override float yellowText => LifeTime;
    public override float blueText => LifeTime;
    public override StatType yellowStatType => StatType.ProjectileSize;

    public int wandSlot => PlayerControllerV2.instance.wand.spells.IndexOf(this);
    public WandV2 wand => PlayerControllerV2.instance.wand;
    public PlayerControllerV2 player => PlayerControllerV2.instance;
    
    [Header("Base Stats")]
    [SerializeField] private float b_damage = 5;
    // [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_numOfProjectiles = 3;
    // [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_size = 1;
    //[FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_laserDistance = 3;
    //[FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_spread = 10;
    //[FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_knockback = 0;
   // [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_projectileSpeed = 10;
    //[FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_bounce = 0;
    //[FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private int b_piercing = 0;
    //[FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_inaccuracy = 0f;
   // [FoldoutGroup("Base Values",expanded:false)]
    [SerializeField] private float b_lifetime = 1f;
    [SerializeField] private float b_homingAngle = 90f;

    public float Damage => statsMod[StatType.Red].Join(statsMod[StatType.BulletDamage]).Modify(b_damage);
    public int NumOfProjectiles => Mathf.FloorToInt(Mathf.Max(1, (int)statsMod[StatType.Projectiles].Modify(b_numOfProjectiles)));  //old yellow
    public float LifeTime => statsMod[StatType.Yellow].Join(statsMod[StatType.LifeTime]).Modify(b_lifetime);
    public float Spread => statsMod[StatType.Spread].Modify(b_spread);
    
    public void OnEnable()
    {
        LaserProjectileTest.LastHitEvent += OnLastHit;
    }
    
    public void OnDisable()
    {
        LaserProjectileTest.LastHitEvent -= OnLastHit;
    }

    private void OnLastHit(LaserProjectileTest.LastHitEventData lastHitEventData)
    {
        if (lastHitEventData.caster != this) return;
        
        foreach (SpellItemV2 spell in triggeredSpells)
        {
            if (spell != null)
            {
                spell.TryCast(new CastCommand()
                {
                    Pos = lastHitEventData.hitPos,
                    Dir = lastHitEventData.dir,
                    PrevHits = new List<GameObject> { lastHitEventData.hitObject }
                }, manaGen);
            }
        }
    }

    public override void Shoot(ProjectileRecipe recipe, CastCommand castCommand, int numProjectiles, float spread, float inaccuracy)
    {
        const int PROJ_PER_UNIT = 4;
        Vector3 sideBySide = ((Vector2)castCommand.Dir).Rotate(90);
        float offset = 1 * numProjectiles * 0.5f / 2;

        float laserDist = b_laserDistance;//statsMod[StatType.ProjectileSize].Modify(b_size);

        //Commented out code is for multiple projectiles
        // for (int j = 0; j < numProjectiles; j++)
        // {
        //     Vector3 spawnPos = pos + offset * -sideBySide + j * 0.5f * sideBySide;
        //     for (int i = 0; i < PROJ_PER_UNIT * laserDist; i++)
        //     {
        //         //spawnPos = pos + (dir * i / (float)PROJ_PER_UNIT).V3();

                GameObject proj = Instantiate(projPrefab.gameObject, castCommand.Pos, Quaternion.identity);
                LaserProjectileTest projScript = proj.GetComponent<LaserProjectileTest>();
                projScript.Init(recipe, castCommand.Dir, castCommand.PrevHits);
                InvokeOnShoot(this);
           // }
        //}
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
        
        ProjectileRecipe recipe = GetProjectileRecipe();
        HashSet<GameObject> ignore = new();
        for (int i = 0; i < numCasts; i++)
        {
            float homeRadius = b_laserDistance; //Gameplay: Could also make this scale with size. statsMod[StatType.ProjectileSize].Modify(b_laserDistance);

            GameObject targetGO = castCommand.Pos.FindClosestEnemyInSize(homeRadius, castCommand.Dir, b_homingAngle, ignore: ignore);
        
            if (targetGO != null)
            {
                castCommand.Dir = (targetGO.transform.position - castCommand.Pos).normalized;
                ignore.Add(targetGO);
            }
        
            Shoot(recipe, castCommand, NumOfProjectiles, Spread, b_inaccuracy);
        }
        
        
        return true;
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

    public override ProjectileRecipe GetProjectileRecipe() 
    {
        ProjectileRecipe projectileRecipe = new ProjectileRecipe();
        
        projectileRecipe.damage = Damage;
        projectileRecipe.spawnID = Guid.NewGuid().ToString();
        projectileRecipe.projectileSpeed = statsMod[StatType.ProjectileSpeed].Modify(b_projectileSpeed);
        projectileRecipe.size = statsMod[StatType.ProjectileSize].Modify(b_size);
        projectileRecipe.knockback = statsMod[StatType.Knockback].Modify(b_knockback);
        projectileRecipe.bounce = Mathf.Max(0, (int)statsMod[StatType.Bounce].Modify(b_bounce)) * uiInfo.level;
        projectileRecipe.piercing = Mathf.Max(0, (int)statsMod[StatType.Piercing].Modify(b_piercing));
        projectileRecipe.owner = player.gameObject;
        projectileRecipe.lifetime = LifeTime;
        projectileRecipe.ownerSpell = this;
        
        return projectileRecipe;
    }

    private void OnDestroy()
    {
        LaserProjectileTest.LastHitEvent -= OnLastHit;
    }
    
    public override string GetTooltip()
    {
        return $"High powered homing beam\nBounces {statsMod[StatType.Bounce].Modify(b_bounce) * uiInfo.level}x\nGives {manaGen} mana on last bounce";
    }
    
}