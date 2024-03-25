using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlackHoleProjectile : MonoBehaviour
{
    private int DOTHitsPerSec = 5;
    [SerializeField, Required] private PointEffector2D _pointEffector;
    [SerializeField, Required] private CircleCollider2D _collider;
    [SerializeField] private ContactFilter2D _overlapContactFilter;
    
    public SpellItemV2 spellOwner;
    
    public ProjectileRecipe recipe;
    
    //We keep these components so that when speed = 0, dir can still have
    //A value that we can use to calculate the direction of a triggered projectile
    //Not doing a velocity field so that people don't actually do velocity.normalized
    //for direction and they're code doesn't work when speed = 0
    //Just calculate vel everytime rn, seems like a reasonable mental overhead
    public Vector2 dir;
    public float speed;
    
    // An initial size of 16 will be enough for most cases, but the list will be resized if needed at runtime.
    private readonly List<Collider2D> _overlapResults = new(16);
    
    public static event Action<ExpireEventData> OnExpire;
    public static event Action<DamageEventData> OnDamage;
    
    public void Init(ProjectileRecipe recipe, Vector2 direction)
    {
        this.recipe = recipe;
        spellOwner = recipe.ownerSpell;
        dir = direction;
        speed = recipe.projectileSpeed;
        transform.localScale = Vector3.one * recipe.size;
        // TODO: init point effector with some multiplier of recipe.knockback
        
        StartCoroutine(PeriodicDamageCR());
        StartCoroutine(LifetimeCR());
    }

    private void Update()
    {
        //Rotate -90 because the projectile visuals face up instead of to the right
        transform.rotation = dir.Rotate(-90).DirToRotation();
        transform.position += dir.V3() * speed * Time.deltaTime;
    }
    
    private IEnumerator PeriodicDamageCR()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(1f/DOTHitsPerSec);
            
            int numHits = _collider.OverlapCollider(_overlapContactFilter, _overlapResults);
            
            for (int i = 0; i < numHits; i++)
            {
                if (_overlapResults[i].TryGetComponent(out Hurtbox hurtbox))
                {
                    PeriodicDamageHurtbox(hurtbox);
                    DamageEventData damageEventData = new()
                    {
                        caster = recipe.ownerSpell,
                        projectile = this,
                        enemy = hurtbox.Owner
                    };
                    OnDamage?.Invoke(damageEventData);
                }
            }
        }
    }
    
    private void PeriodicDamageHurtbox(Hurtbox hurtbox)
    {
        if (!hurtbox.Owner.HasHTag(HTags.Enemy)) return;
        
        float distanceToCenter = Vector2.Distance(hurtbox.Owner.transform.position, transform.position);
        float closenessToCenter = Mathf.InverseLerp(0f, _collider.radius, distanceToCenter);
        
        if (hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
        {
            // Scale damage so enemies near center take more damage than farther ones.
            //We don't ceil this to 1 because enemy HP is not high enough for that to be balanced
            float damage = Mathf.Lerp(0f, recipe.damage/DOTHitsPerSec, closenessToCenter);
            
            healthV2.ChangeHp(-damage, gameObject);
            RelicsManager.instance.OnHit();

        }
        
        if (hurtbox.Owner.TryGetComponent(out KnockbackReceiver knockbackReceiver))
        {
            // Try perpendicular so that enemies get knocked around like they're orbiting the center
            Vector2 knockbackDirection = Vector2.Perpendicular(hurtbox.Owner.transform.position - transform.position);
            
            // Scale knockback so enemies receive more knockback the farther they are from the center
            float knockbackMagnitude = Mathf.Lerp(recipe.knockback, 0f, closenessToCenter);
            
            knockbackReceiver.ApplyKnockback(knockbackDirection, knockbackMagnitude);
        }
    }
    
    private IEnumerator LifetimeCR()
    {
        yield return new WaitForSeconds(recipe.lifetime);
        
        Implode();
        
        Destroy(gameObject);
    }
    
    private void Implode()
    {
        ExpireEventData expireEventData = new()
        {
            caster = recipe.ownerSpell,
            projectile = this
        };
        
        int numHits = _collider.OverlapCollider(_overlapContactFilter, _overlapResults);
        
        for (int i = 0; i < numHits; i++)
        {
            if (_overlapResults[i].TryGetComponent(out Hurtbox hurtbox) &&
                hurtbox.Owner.HasHTag(HTags.Enemy))
            {
                expireEventData.enemiesInside.Add(hurtbox.Owner);
            }
        }
        
        OnExpire?.Invoke(expireEventData);
    }
    
    public class ExpireEventData
    {
        public SpellItemV2 caster;
        public BlackHoleProjectile projectile;
        public List<GameObject> enemiesInside = new();
    }

    public class DamageEventData
    {
        public SpellItemV2 caster;
        public BlackHoleProjectile projectile;
        public GameObject enemy = null;
    }
}
