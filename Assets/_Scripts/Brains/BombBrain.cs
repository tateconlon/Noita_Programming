using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBrain : MonoBehaviour//, ITagged<Elite, Enemy, Flying>
{
    // [SerializeField] private HitBox _explosionHitBox;
    //private ExplodeAbility _explodeAbility;
    private int level;

    private void OnEnable()
    {
        //Tags.addtags
    }

    //We can have different hitboxes for different abilities
    private void OnExplosionHitbox(List<Actor> hitActors)
    {
        // _explodeAbility = new();
        // _explodeAbility.Activate(hitActors);
        if (level >= 2)
        {
            //FireAbility fire = new();
            //fire.Activate(hitActors);
        }
    }

    // public class ExplodeAbility
    // {
    //     private float dmg;
    //     private float level;
    //     public void Activate(List<Actor> targets)
    //     {
    //         dmg = targets.Count * level * 5;
    //         foreach (Actor target in targets)
    //         {
    //             
    //             // target.ModifyAttribute<HealthAttribute>(-dmg);
    //             // preffered
    //             if (target.TryGetAttribute<HealthAttribute>(out HealthAttribute health))
    //             {
    //                 health.TakeDamage(dmg);
    //                 Tags.Elite.Add(target);
    //             }
    //             
    //             
    //             //DON'T DO THIS, THEN ALL ELITES MUST INHERIT!!! BADDDD if (target is Elite as elite) 
    //             if (target.HasTag(Tags.Elite))  // also ability to check if tag contains actor and query all in tag
    //             {
    //             }
    //
    //
    //             foreach (var VARIABLE in EliteTag.actors)
    //             {
    //
    //             }
    //
    //             // actor has Dictionary<Packet, Coroutine>
    //
    //             target.StartCoroutine(FireOverTime(target));
    //         }
    //     }
    //     
    //     private IEnumerator FireOverTime(Actor target)
    //     {
    //         target.Tags.Add(Tags.Fire);
    //         float timeLeft = 4;
    //         while (timeLeft > 0)
    //         {
    //
    //             FireDurationEffect fireAbility = new();
    //             target.FireDurationEffect(fireAbility);
    //             
    //             
    //             if (target.TryGetAttribute<HealthAttribute>(out HealthAttribute health))
    //             {
    //                 health.TakeDamage(dmg);
    //             }
    //             
    //             yield return new WaitForSeconds(0.6f);
    //             timeLeft -= 0.6f;
    //         }
    //         
    //         target.Tags.Remove(Tags.Fire);
    //     }
    //     
    //     private IEnumerator FireOverTime(Actor target)
    //     {
    //         target.Tags.Add(Tags.Fire);
    //         float timeLeft = 4;
    //         while (timeLeft > 0)
    //         {
    //             //target.GetBurned(stacks: 1);
    //
    //             yield return new WaitForSeconds(0.6f);
    //             timeLeft -= 0.6f;
    //         }
    //         
    //         target.Tags.Remove(Tags.Fire);
    //     }
    // }
    //
    // public class FireAbility
    // {
    //     public void Activate(List<Actor> targets)
    //     {
    //         foreach (Actor target in targets)
    //         {
    //             //target.Tags.Add("Fire");
    //             //target.Effects.AddDurationEffect("Fire");
    //         }
    //     }
    // }
    //
    // public class FireDurationEffect : MonoBehaviour
    // {
    //     public event Action OnEnd;
    //     
    //     float timer;
    //     private int stacks;
    //     private float period;
    //     private float dmg;
    //
    //     public IEnumerator FireCR;
    //
    //     public Actor target;
    //
    //     void Start(Actor target)
    //     {
    //         this.target = target;
    //         FireCR = Update();
    //         target.StartCoroutine(FireCR);
    //     }
    //
    //     // void OnTagsChanged(Actor target)
    //     // {
    //     //     if (target.Tags.Contains("Water"))
    //     //     {
    //     //         CleanUp();
    //     //     }
    //     // }
    
    //  
    //
    //     void Update()
    //     {
    //         //Start up
    //         //target.playPS.burnStartEffects;
    //         //target.playPS.burnEffects;
    //         
    //         while (stacks < 0)
    //         {
    //             yield return new WaitForSeconds(period);
    //             //timer -= Time.deltaTime;
    //             if (target.Tags.Contains("Soaked"))
    //             {
    //                 CleanUp();
    //             }
    //             if (timer < 0)
    //             {
    //                 //FireAbilityHere
    //                 DoDamage();
    //             
    //                 stacks--;
    //                 if (stacks > 10)
    //                 {
    //                     timer += period / 2;
    //                 }
    //                 else if(stacks > 0)
    //                 {
    //                     timer += period;
    //                 }
    //                 else
    //                 {
    //                     CleanUp();
    //                 }
    //             }
    //         }
    //         //End of effect
    //     }
    //
    //     void DoDamage()
    //     {
    //         target.Attributes.HealthAttribute.TakeDamage(dmg*stacks);
    //     }
    //
    //     void Add(FireDurationEffect eff)
    //     {
    //         //target.playPS.burnStartEffects;
    //         stacks += eff.stacks;
    //         dmg = Math.Max(eff.dmg, dmg);
    //         DoDamage(); //Apply burn damage right away for snappy visual feedback
    //     }
    //
    //     void CleanUp()
    //     {
    //         stacks = -99;
    //         timer = -99f;
    //         //RemoveTag
    //         OnEnd?.Invoke();
    //     }
    //     
    // }
}