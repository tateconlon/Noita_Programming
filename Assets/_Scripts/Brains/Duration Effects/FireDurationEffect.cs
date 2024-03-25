using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDurationEffect : MonoBehaviour
{
    public event Action OnEnd;
    
    float timer;
    private int stacks;
    private float period = 0.5f;
    private float dmg;
    
    Coroutine FireCR;

    public IActor parent;
    public IActor target;

    void OnEnable()
    {
        this.target = target;
        
        //Start up
        //target.playPS.burnStartEffects;
        //target.playPS.burnEffects;
    }

    void Init(IActor target, IActor parent, int stacks)
    {
        this.target = target;
        this.parent = parent;
        this.stacks = stacks;
        timer = period;
        
        FireCR = StartCoroutine(AbilityCR());
    }

    // void OnTagsChanged(Actor target)
    // {
    //     if (target.Tags.Contains("Water"))
    //     {
    //         CleanUp();
    //     }
    // }



    IEnumerator AbilityCR()
    {
        // if (target.actor.HasTag(Tags.Water))
        // {
        //     CleanUp();
        // }

        if (timer < 0)
        {
            //FireAbilityHere
            DoDamage();

            stacks--;
            if (stacks > 10)
            {
                timer += period / 2;
            }
            else if (stacks > 0)
            {
                timer += period;
            }
            else
            {
                CleanUp();
            }
        }

        yield return null;
    }

    void DoDamage()
    {
        HealthAttribute health = target.actor.GetAttribute<HealthAttribute>();
        health.TakeDamage(3, null);
    }

    void Add(FireDurationEffect eff)
    {
        //target.playPS.burnStartEffects;
        stacks += eff.stacks;
        dmg = Math.Max(eff.dmg, dmg);
        DoDamage(); //Apply burn damage right away for snappy visual feedback
    }

    void CleanUp()
    {
        stacks = -99;
        timer = -99f;
        //RemoveTag
        OnEnd?.Invoke();
    }
    
}