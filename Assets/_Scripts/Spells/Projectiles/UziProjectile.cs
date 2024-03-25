using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class UziProjectile : MonoBehaviour
{
    [SerializeField, Required] private Hitbox _hitbox;
    
    public SpellItemV2 spellOwner;

    public ProjectileRecipe recipe;
    
    private List<GameObject> _ignoreObjects = new();
    private int pierceCount = 0;

    //We keep these components so that when speed = 0, dir can still have
    //A value that we can use to calculate the direction of a triggered projectile
    //Not doing a velocity field so that people don't actually do velocity.normalized
    //for direction and they're code doesn't work when speed = 0
    //Just calculate vel everytime rn, seems like a reasonable mental overhead
    public Vector2 dir;
    public float speed;
    
    public bool isTriggerable = true;
    public float triggerTimer = 0f;
    
    public static event Action<ImpactEventData> ImpactEvent;
    
    private void OnEnable()
    {
        _hitbox.OnHit += HitboxOnOnHit;
    }
    
    private void HitboxOnOnHit(Hitbox hitbox, HashSet<Hurtbox> hurtboxes)
    {
        foreach (Hurtbox hurtbox in hurtboxes)
        {
            if (!_ignoreObjects.Contains(hurtbox.Owner) &&
                hurtbox.Owner.HasHTag(HTags.Enemy) && 
                hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
            {
                healthV2.ChangeHp(-recipe.damage, gameObject);
                RelicsManager.instance.OnHit();
                
                if (hurtbox.Owner.TryGetComponent(out KnockbackReceiver knockbackReceiver))
                {
                    Vector2 knockbackDirection = hurtbox.Owner.transform.position - hitbox.Owner.transform.position;
                    knockbackReceiver.ApplyKnockback(knockbackDirection, recipe.knockback);
                }
                
                ImpactEventData e = new()
                {
                    caster = recipe.ownerSpell,
                    projectile = this,
                    hitObject = hurtbox.Owner
                };

                ImpactEvent?.Invoke(e);
                
                pierceCount--;
                
                if (pierceCount < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void Init(ProjectileRecipe recipe, Vector2 direction, List<GameObject> ignoreObjects)
    {
        this.recipe = recipe;
        spellOwner = recipe.ownerSpell;
        dir = direction;
        speed = recipe.projectileSpeed;
        transform.localScale = Vector3.one * recipe.size;
        pierceCount = recipe.piercing;
        _ignoreObjects = ignoreObjects;
        
        StartCoroutine(LifetimeCR());
    }
    
    void Update()
    {
        transform.rotation = dir.DirToRotation();
        transform.position += dir.V3() * speed * Time.deltaTime;
    }
    
    private IEnumerator LifetimeCR()
    {
        yield return new WaitForSeconds(recipe.lifetime);
        
        Destroy(gameObject);
    }
    
    private void OnDisable()
    {
        _hitbox.OnHit -= HitboxOnOnHit;
    }
    
    public class ImpactEventData
    {
        public SpellItemV2 caster;
        public UziProjectile projectile;
        public GameObject hitObject;
    }
}