using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MeteorProjectile : StateMachine<MeteorProjectile>
{
    [Header("Bindings")]
    [SerializeField, Required] private SpriteRenderer _fallingShadow;
    [SerializeField, Required] private SpriteRenderer _fallingMeteor;
    [SerializeField, Required] private SpriteRenderer _explosionSprite;
    [SerializeField, Required] private Hitbox _hitbox;
    
    [Header("States")]
    [SerializeField] private FallingState _falling;
    [SerializeField] private ExplodingState _exploding;
    
    public Vector2 dir;
    public float speed;
    
    public SpellItemV2 spellOwner;
    public ProjectileRecipe recipe;
    
    private const float ProportionOfLifetimeSpentFalling = 0.75f;
    
    public static event Action<EnemyKilledEventData> EnemyKilledEvent; 
    
    public void Init(ProjectileRecipe recipe, Vector2 direction)
    {
        this.recipe = recipe;
        spellOwner = recipe.ownerSpell;
        dir = direction;
        speed = recipe.projectileSpeed;
        transform.localScale = Vector3.one * recipe.size;
        
        Transition(_falling);
    }
    
    [Serializable]
    public class FallingState : State
    {
        private float _fallingDuration;
        private float _fallingStartTime;
        private float _fallingEndTime;
        
        public override void Enter()
        {
            StateMachine._fallingShadow.enabled = true;
            StateMachine._fallingMeteor.enabled = true;
            
            StateMachine._explosionSprite.enabled = false;
            StateMachine._hitbox.enabled = false;
            
            base.Enter();
            
            // TODO: replace this with doing work in Update. Grow shadow and move the meteor downwards over time
            
            _fallingStartTime = Time.time;
            _fallingDuration = ProportionOfLifetimeSpentFalling * StateMachine.recipe.lifetime;
            _fallingEndTime = _fallingStartTime + _fallingDuration;
            
            StateMachine._fallingShadow.transform.localScale = Vector3.zero;
            StateMachine._fallingMeteor.transform.position = StateMachine.transform.position;
            StateMachine._fallingMeteor.transform.Translate(0f, 20f, 0f);
        }

        public override void Update()
        {
            base.Update();
            
            float lerpFactor = Mathf.InverseLerp(_fallingStartTime, _fallingEndTime, Time.time);
            
            StateMachine._fallingShadow.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpFactor);
            
            Vector3 meteorLocalPos = StateMachine._fallingMeteor.transform.localPosition;
            meteorLocalPos.y = Mathf.Lerp(20f, 0f, lerpFactor);
            StateMachine._fallingMeteor.transform.localPosition = meteorLocalPos;
            
            if (Time.time >= _fallingEndTime)
            {
                StateMachine.Transition(StateMachine._exploding);
            }
        }
    }
    
    [Serializable]
    public class ExplodingState : State
    {
        public override void Enter()
        {
            StateMachine._fallingShadow.enabled = false;
            StateMachine._fallingMeteor.enabled = false;
            
            StateMachine._explosionSprite.enabled = true;
            
            base.Enter();
            
            StateMachine._hitbox.OnHit += HitboxOnOnHit;
            StateMachine._hitbox.enabled = true;
            
            StateMachine.StartCoroutine(ExplosionLifetimeCR());
        }
        
        private void HitboxOnOnHit(Hitbox hitbox, HashSet<Hurtbox> hurtboxes)
        {
            foreach (Hurtbox hurtbox in hurtboxes)
            {
                if (hurtbox.Owner.HasHTag(HTags.Enemy) && 
                    hurtbox.Owner.TryGetComponent(out HealthV2 healthV2) &&
                    !healthV2.IsDead)
                {
                    healthV2.ChangeHp(-StateMachine.recipe.damage, StateMachine.gameObject);
                    RelicsManager.instance.OnHit();
                    
                    if (hurtbox.Owner.TryGetComponent(out KnockbackReceiver knockbackReceiver))
                    {
                        Vector2 knockbackDirection = hurtbox.Owner.transform.position - hitbox.Owner.transform.position;
                        knockbackReceiver.ApplyKnockback(knockbackDirection, StateMachine.recipe.knockback);
                    }
                    
                    if (healthV2.IsDead)
                    {
                        EnemyKilledEvent?.Invoke(new EnemyKilledEventData
                        {
                            caster = StateMachine.spellOwner,
                            projectile = StateMachine,
                            killedEnemy = hurtbox.Owner
                        });
                    }
                }
            }
        }
        
        private IEnumerator ExplosionLifetimeCR()
        {
            yield return new WaitForSeconds((1.0f - ProportionOfLifetimeSpentFalling) *
                                            StateMachine.recipe.lifetime);
            
            Destroy(StateMachine.gameObject);
        }
        
        public override void Exit()
        {
            StateMachine._hitbox.OnHit -= HitboxOnOnHit;
            
            base.Exit();
        }
    }
    
    public class EnemyKilledEventData
    {
        public SpellItemV2 caster;
        public MeteorProjectile projectile;
        public GameObject killedEnemy;
    }
}