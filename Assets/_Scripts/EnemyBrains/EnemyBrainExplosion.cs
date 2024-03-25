using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyBrainExplosion : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField, Required] private Hitbox _hitbox;
    
    private const float BaseSize = 4.0f;
    private const float BaseDuration = 0.25f;
    
    private void Awake()
    {
        _hitbox.OnHit += OnHitboxHit;
    }
    
    private void OnHitboxHit(Hitbox hitbox, HashSet<Hurtbox> hurtboxes)
    {
        foreach (Hurtbox hurtbox in hurtboxes)
        {
            if (hurtbox.Owner.HasHTag(HTags.Player))
            {
                if (hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
                {
                    healthV2.ChangeHp(-0.5f, gameObject);
                }
            }
            else if (hurtbox.Owner.HasHTag(HTags.Enemy))
            {
                if (hurtbox.Owner.TryGetComponent(out HealthV2 healthV2))
                {
                    healthV2.ChangeHp(-4.0f, gameObject);
                }
            }
        }
    }
    
    private void OnEnable()
    {
        transform.localScale = BaseSize * Vector3.one;
        StartCoroutine(LifetimeCR());
    }
    
    private IEnumerator LifetimeCR()
    {
        yield return new WaitForSeconds(BaseDuration);
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        _hitbox.OnHit -= OnHitboxHit;
    }
}