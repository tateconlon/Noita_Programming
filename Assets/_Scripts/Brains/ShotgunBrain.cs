using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShotgunBrain : MonoBehaviour, IActor
{
    [SerializeField, Required] public Actor MyActor;
    public Actor actor => MyActor;
    //public SphotgunSpawner Parent;
    public SpellItem SpellItem;

    public Health health;
    private float str = 5;

    public int level = 0;

    void Awake()
    {
        // health = MyActor.AddAttribute<HealthAttribute>();
        //stats[dmg].AddFlatValue(2 * str);

        // float tempDmg = playerstats.Modify(dmg);
        // float resDmg = stats.Modify(tempDmg);
        
        
        //dmg = 5;
    }

    private void OnEnable()
    {
        // health.value = 100;
        // str.value = 100;

        // MyActor.AddTag(Tags.Attack);
    }

    private void OnBulletHitbox(List<Actor> hitActors)
    {
        // _explodeAbility = new();
        // _explodeAbility.Activate(hitActors);
        if (level >= 2)
        {
            //FireAbility fire = new();
            //fire.Activate(hitActors);
        }

        foreach (Actor hitActor in hitActors)
        {
            if (hitActor.TryGetComponent(out HealthAttribute targetHealth))
            {
                //targetHealth.TakeDamage(stats[strength].modify(SpellItem.damage), this);
            }
            
            // if (hitActor.Attributes.Contains())
            //
            // if (hitActor.health != null)
            // {
            //     //health = hitActor.health as HealthAttribute;
            //     health.TakeDamage(dmg);
            //     Tags.Elite.Add(target);
            // }
        }
        
    }

    public class Shotgun_FireAbility : MonoBehaviour
    {
        private float period = 0.5f;
        private float dmg = 0;
        private float duration = 5;

        private List<Actor> targets;
        private Dictionary<Actor, Coroutine> CRs = new ();

        void Activate(List<Actor> targets)
        {
            foreach (Actor actor in targets)
            {
                //Coroutine cr = actor.StartCoroutine(Shotgun_FireCR(actor));
                //CRs[actor] = cr;
            }
        }

        IEnumerator Shotgun_FireCR(Actor target, Actor sender)
        {
            // target.AddTag(Tags.Fire);
            while (duration >= 0)
            {
                yield return new WaitForSeconds(period);
                duration -= period;
                HealthAttribute health = target.GetAttribute<HealthAttribute>();
                health.TakeDamage(dmg, null);
            }
            // target.RemoveTag(Tags.Fire);
        }

        void Cancel()
        {
            foreach ((Actor actor, Coroutine cr) in CRs)
            {
                actor.StopCoroutine(cr);
                // actor.RemoveTag(Tags.Fire);
            }
        }
    }
}