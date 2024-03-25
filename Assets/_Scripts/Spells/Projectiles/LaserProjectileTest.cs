using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

//Lasers are not projectiles though............
public class LaserProjectileTest : MonoBehaviour
{
    public SpellItemV2 spellOwner;
    public ProjectileRecipe recipe;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LineRenderer _shadowLineRenderer;
    
    private int bounceCount = 0;
    private int pierceCount = 0;
    private List<GameObject> _ignoreObjects = new();
    
    //To prevent laser from going back and forth between 2 enemies
    private HashSet<GameObject> ignoreBounceEnemies = new HashSet<GameObject>();
    
    //We keep these components so that when speed = 0, dir can still have
    //A value that we can use to calculate the direction of a triggered projectile
    public Vector2 dir;
    public float speed;

    public float laserWidth;
    public float laserDist;
    //Not doing a velocity field so that people don't actually do velocity.normalized
    //for direction and they're code doesn't work when speed = 0
    //Just calculate vel everytime rn, seems like a reasonable mental overhead.

    private bool shouldTriggerOnExpire = true;
    
    public static event Action<LastHitEventData> LastHitEvent;
    
    private List<Vector3> points = new List<Vector3>();

    private void OnHit(Hurtbox hurtbox)
    {
        if (!_ignoreObjects.Contains(hurtbox.Owner) &&
            hurtbox.Owner.HasHTag(HTags.Enemy) && 
            hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
        {
            healthV2.ChangeHp(-recipe.damage, gameObject);
            RelicsManager.instance.OnHit();
            
            if (hurtbox.Owner.TryGetComponent(out KnockbackReceiver knockbackReceiver))
            {
                Vector2 knockbackDirection = hurtbox.Owner.transform.position - hurtbox.Owner.transform.position;
                knockbackReceiver.ApplyKnockback(knockbackDirection, recipe.knockback);
            }
        }
    }

    public void Init(ProjectileRecipe recipe, Vector2 direction, List<GameObject> ignoreObjects)
    {
        this.recipe = recipe;
        spellOwner = recipe.ownerSpell;
        dir = direction;
        speed = recipe.projectileSpeed;
        laserWidth = recipe.size / 4f;
        laserDist = recipe.lifetime * 3;//Mult by 3 here so we can keep the stat # down
        bounceCount = recipe.bounce;
        pierceCount = recipe.piercing;
        points.Clear();
        transform.localScale = Vector3.one * recipe.size;
        _ignoreObjects = ignoreObjects;

        StartCoroutine(LifetimeCR());
    }

    //Laser is represented by a series of points for the trail renderer to draw
    private bool hasRunOnce = false;
    void FixedUpdate()
    {
        if (hasRunOnce)
        {
            return;
        }
        
        const int POINTS_PER_UNIT = 4;
        float distPerPoint = 1f / POINTS_PER_UNIT;
        
        int numPoints = Mathf.FloorToInt(POINTS_PER_UNIT * laserDist);

        Vector2 currDir = dir;
        Vector3 spawnPos = transform.position;

        LastHitEventData lastHitEventData = null;
        
        points.Add(spawnPos);
        
        for(int i = 1; i < numPoints; i++)
        {
            points.Add(points[i-1] + (currDir * distPerPoint).V3());
            RaycastHit2D[] hits = Physics2D.CircleCastAll(points[i-1], laserWidth, currDir, distPerPoint);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null) continue;
                
                if (hit.collider.TryGetComponent(out Hurtbox hurtbox) 
                    && !ignoreBounceEnemies.Contains(hurtbox.Owner)
                    && hurtbox.Owner.HasHTag(HTags.Enemy) && 
                    hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
                {
                    shouldTriggerOnExpire = false;
                    healthV2.ChangeHp(-recipe.damage, gameObject);
        
                    if (hurtbox.Owner.TryGetComponent(out KnockbackReceiver knockbackReceiver))
                    {
                        Vector2 knockbackDirection = hurtbox.Owner.transform.position - hurtbox.Owner.transform.position;
                        knockbackReceiver.ApplyKnockback(knockbackDirection, recipe.knockback);
                    }

                    lastHitEventData = new()
                    {
                        projectile = this,
                        hitObject = hurtbox.Owner,
                        caster = recipe.ownerSpell,
                        hitPos = points[points.Count - 1], //Our current point
                        dir = currDir.normalized
                    };
                    
                    if (bounceCount > 0)
                    {
                        //Complication - since projectiles bounce (or change direction), we can't pass
                        //recipe.direction to the next projectile. We need to pass the current direction of projectile.
                        //or in the laser's case, the direction of the laser at the point of impact.
                        bounceCount--;
                        currDir = BounceOffEnemyHoming(hurtbox.Owner, currDir);
                    }
                    else if (pierceCount > 0)
                    {
                        pierceCount--;
                    }
                }
            }
        }
        
        _lineRenderer.positionCount = points.Count;
        _shadowLineRenderer.positionCount = points.Count;
        
        _lineRenderer.SetPositions(points.ToArray());
        
        List<Vector3> shadowPoints = new List<Vector3>();
        for(int i = 0; i < points.Count; i++)
        {
            shadowPoints.Add(points[i] + Vector3.down * 0.3f);
        }

        _shadowLineRenderer.SetPositions(shadowPoints.ToArray());

        _lineRenderer.startWidth = laserWidth;
        _shadowLineRenderer.startWidth = laserWidth;
        _lineRenderer.endWidth = laserWidth;
        _shadowLineRenderer.endWidth = laserWidth;

        dir = currDir;  //ending Dir is the direction of the last point
        hasRunOnce = true;
        
        if (lastHitEventData != null)
        {
            LastHitEvent?.Invoke(lastHitEventData);
        }
    }

    //Projectile.cs
    //Returns new dir
    protected Vector2 BounceOffEnemyHoming(GameObject enemy, Vector2 orgDir)
    {
        if (enemy != null)
        {
            Vector2 hitPos = enemy.transform.position;
            
            ignoreBounceEnemies.Add(enemy);
            //just use the entire size of the laser for the closest enemy since it doesn't matter what the size of the check is
            //since it'll just return the closest enemy in range anyways
            Transform transform = enemy.transform.position.FindClosestEnemyInSize(laserDist, ignore: ignoreBounceEnemies)?.transform;
            if (transform != null)
            {
                Vector2 newDir = new Vector2(transform.position.x, transform.position.y) - hitPos;
                return newDir.normalized;
            }
        }

        //Nothing in range, just reflect randomly
        Vector2 defaultBounceDir = Vector2.Reflect(base.transform.right, orgDir).normalized;
        return defaultBounceDir.Rotate(UnityEngine.Random.Range(-45, 45));
    }
    
    private IEnumerator LifetimeCR()
    {
        yield return new WaitForSeconds(0.5f);  // Wait to destroy purely for visual purposes
        
        Destroy(gameObject);
    }
    
    private void OnDisable()
    {
        ignoreBounceEnemies.Clear();
    }
    
    public class LastHitEventData
    {
        public SpellItemV2 caster;
        public LaserProjectileTest projectile;
        public GameObject hitObject;
        public Vector2 dir; //direction when hit
        public Vector3 hitPos;
    }
}