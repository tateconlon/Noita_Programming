using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    // [SerializeField, Required] private Hitbox _hitbox;
    
    public SpellItemV2 spellOwner;

    public ProjectileRecipe recipe;
    private float size;

    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private GameObject explosionRoot;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private GameObject bombRoot;

    // private int pierceCount = 0;

    //We keep these components so that when speed = 0, dir can still have
    //A value that we can use to calculate the direction of a triggered projectile
    //Not doing a velocity field so that people don't actually do velocity.normalized
    //for direction and they're code doesn't work when speed = 0
    //Just calculate vel everytime rn, seems like a reasonable mental overhead
    // public Vector2 dir;
    // public float speed;
    //
    public bool isTriggerable = true;
    
    public static event Action<ExplodeEventData> ExplodeEvent;
    

    public void Init(ProjectileRecipe recipe, Vector2 direction)
    {
        this.recipe = recipe;
        spellOwner = recipe.ownerSpell;
        // dir = direction;
        // speed = recipe.projectileSpeed;
        explosionRoot.transform.localScale = Vector3.one * recipe.size;
        size = recipe.size;
        // pierceCount = recipe.piercing;
        explosionRoot.SetActive(false);
        bombRoot.SetActive(true);
        StartCoroutine(Explode());
        StartCoroutine(LifetimeCR());
    }
    
    private IEnumerator Explode()
    {
        //Just here in case we want to add a delay to the explosion, but currently not needed
        yield return new WaitForSeconds(0.4f);
        yield return new WaitForFixedUpdate();
        
        List<GameObject> hitEnemies = new List<GameObject>();

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, size/2f);
        foreach (Collider2D col in cols)
        {
            if (col.TryGetComponent(out Hurtbox hurtbox))
            {
                if (hurtbox.Owner.HasHTag(HTags.Enemy) && 
                    hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
                {
                    hitEnemies.Add(hurtbox.Owner);
                    healthV2.ChangeHp(-recipe.damage, gameObject);
                    RelicsManager.instance.OnHit();
                    
                    if (hurtbox.Owner.TryGetComponent(out KnockbackReceiver knockbackReceiver))
                    {
                        Vector2 knockbackDirection = hurtbox.Owner.transform.position - transform.position;
                        knockbackReceiver.ApplyKnockback(knockbackDirection, recipe.knockback);
                    }
                }
            }
        }
        
        ExplodeEvent?.Invoke(new ExplosionProjectile.ExplodeEventData()
        {
            caster = recipe.ownerSpell,
            projectile = this,
            hitEnemiesGO = hitEnemies
        });
        
        bombRoot.SetActive(false);
        explosionRoot.SetActive(true);
        yield return new WaitForSeconds(0.2f);
    }
    
    private IEnumerator LifetimeCR()
    {
        yield return new WaitForSeconds(recipe.lifetime);
        
        Destroy(gameObject);
    }

    public class ExplodeEventData
    {
        public SpellItemV2 caster;
        public ExplosionProjectile projectile;
        public List<GameObject> hitEnemiesGO = new List<GameObject>();
    }
}