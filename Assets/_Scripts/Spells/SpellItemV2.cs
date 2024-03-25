using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellItemV2 : MonoBehaviour
{
    public StatsModHolder statsMod;

    public UIInfo uiInfo = new();

    public virtual List<SpellItemV2> triggeredSpells { get; set; } = new();
    public virtual int maxNumSpells { get; } = 1;
    public virtual bool manaOverspill { get; set; } = false;

public virtual float UI_manaCost { get; }
    public virtual float UI_manaGen { get; }
    public virtual float UI_manaCurr { get; }
    
    
    public abstract float redText { get; }
    public abstract float yellowText { get; }
    public abstract float blueText { get; }
    public abstract StatType yellowStatType { get; }
    
    public static event Action<SpellItemV2> OnShoot;
    
    public abstract bool TryCast(CastCommand castCommand, float manaGain);
    
    //TODO TATE: I DON'T LIKE THIS BEING GENERIC!
    //What's wrong here is that Shooting a projectile is specific to the spell.
    //So then we're stealing parameters from the projectile recipe to shoot the projectile.
    //Probably makes more sense to have a component for shooting instead.
    //But then you need to have a way to inject into the casting...
    //Maybe casting can have an overload, or we can have a casting recipe???
    //Casting recipe seems too abstract... maybe just keep pushing and bumping against limits
    //For now we'll keep it open and a simple, lightweight, in-code solution might surface
    public abstract void Shoot(ProjectileRecipe recipe, CastCommand castCommand,
        int numProjectiles, float spread, float inaccuracy);
    
    public abstract void ClearPass();
    public abstract void FirstPass();
    public abstract void SecondPass();
    public abstract void ThirdPass();
    public abstract void FourthPass();

    public abstract ProjectileRecipe GetProjectileRecipe();

    public abstract string GetTooltip();

    //Don't need this but thought to keep it here in case
    // protected void CheckStatMods_OnEnable()
    // {
    //     if (statsMod == null)
    //     {
    //         statsMod = GetComponentInChildren<StatsModHolder>(statsMod);
    //         Debug.Log("No StatsModHolder found, creating one", gameObject);
    //         if (statsMod == null)
    //         {
    //             GameObject statModRoot = new GameObject();
    //             statModRoot.transform.parent = transform;
    //             statModRoot.name = "Stat Mod Holder";
    //             statsMod = statModRoot.AddComponent<StatsModHolder>();
    //         }
    //     }
    // }

    //Needed because we can't Invoke an event from a derived class
    protected void InvokeOnShoot(SpellItemV2 spell)
    {
        OnShoot?.Invoke(spell);
    }
    
    [Serializable]
    public class UIInfo
    {
        public string name;
        public Sprite icon;
        public int tier;
        [NonSerialized] public int xp = 1;
        public int level {
            get
            {
                if (xp < 3) return 1;
                if (xp < 6) return 2;
                else return 3;
            }
        }
        
        public int xpInLevel {
            get
            {
                if (xp < 3) return xp;
                if (xp < 6) return xp - 3;
                else return 0;
            }
        }
        
        public int xpToNextLevel {
            get
            {
                if (xp < 3) return 3;
                if (xp < 6) return 3;
                else return 0;
            }
        }
    }

    public class CastCommand
    {
        public Vector3 Pos;
        public Vector3 Dir;
        public List<GameObject> PrevHits = new();
    }
}