using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

//https://gameprogrammingpatterns.com/subclass-sandbox.html
//Inspired by https://www.youtube.com/watch?v=iU6mKyQjOYI&t=71s
//Fill the sandbox with all the verbs of the game, then you can hook into them as you wish
//You can keep it global by assigning the function to a global event, or you can keep it per spell
//by having the spell call the appropriate events
[Serializable]
public class Relic : IDisposable
{
    public virtual string GiveName()
    {
        throw new NotImplementedException();
    }

    //Pass in from MonoBehaviour
    public virtual void Update() { }
    
    public virtual void Activate() {}
    
    public virtual void Deactivate() {}
    
    public virtual void OnHit() {}
    
    public virtual void OnKill(HealthV2.HpChangeParams healthChangedParams) {}
    
    public virtual void OnBuy() {}
    
    public virtual void OnSell() {}
    
    //Not sure if needed, would be more for "mods" than relics
    // public virtual void OnAttach(SpellItemV2 spell) {}
    //
    // public virtual void OnDetach(SpellItemV2 spell) {}
    
    public virtual void OnSpellBuy(SpellItemV2 spell) {}
    
    public virtual void OnSpellSell(SpellItemV2 spell) {}
    
    public virtual void OnWandRebuild() {}

    public virtual void Dispose() { }
}

[Serializable]
public class BuffFirstSlotOnHit : Relic
{
    private StatChange statChange = new() {type = StatType.BulletDamage, isFlatMod = true, flatChange = 1};
    private Dictionary<SpellItemV2, int> stacks = new();
    int numHitsReq => 10;
    private int currHits = 0;
    int maxStacks => 10;
    
    public string Name => GiveName();
    public override string GiveName()
    {
        return $"Every 10 hits buffs the first spell in your wand by +{statChange.flatChange} damage (max {maxStacks} stacks)";
    }

    public void Apply(SpellItemV2 spell)
    {
        if(stacks.TryGetValue(spell, out int numStacks) && numStacks >= maxStacks) return;

        StatChange[] changes = new StatChange[] { statChange };
        spell.statsMod.ApplyStatChanges(changes);
        stacks[spell]= stacks.ContainsKey(spell) ? stacks[spell] + 1 : 1;
    }

    public override void OnHit()
    {
        currHits++;
        if (currHits >= numHitsReq)
        {
            if (currHits > numHitsReq)
            {
                Debug.LogError("currHits > numHitsReq");
            }
            currHits = 0;
            
            SpellItemV2 spell = PlayerControllerV2.instance.wand.spells[0];
            {
                if (spell != null)
                {
                    Apply(spell);
                }
            }
        }
    }

    public override void Deactivate()
    {
        foreach (KeyValuePair<SpellItemV2, int> spell in stacks)
        {
            if (spell.Key == null) continue;
            for (int i = 0; i < spell.Value; i++)
            {
                StatChange[] changes = new StatChange[] { statChange };
                spell.Key.statsMod.RemoveStatChanges(changes.ToArray());
            }
        }
    }
}

[Serializable]
public class BuffSecondSlotMoreProj : Relic
{
    private StatChange statChange = new() {type = StatType.Projectiles, isFlatMod = true, flatChange = 2};
    private SpellItemV2 appliedSpell = null;
    public string Name => GiveName();
    public override string GiveName()
    {
        return "2nd Spell in your wand gets +2 projectiles";
    }

    public BuffSecondSlotMoreProj()
    {
        WandV2.OnWandRebuild += Activate;
    }
    
    public override void Dispose()
    {
        WandV2.OnWandRebuild -= Activate;
    }

    public override void Activate()
    {
        UnApply();
        SpellItemV2 spell = PlayerControllerV2.instance.wand.spells[1];
        Apply(spell);
    }

    public void Apply(SpellItemV2 spell)
    {
        if(spell == null) return;
        if (appliedSpell != null)
        {
            Debug.Log("Applying to a spell without removing the previous one first");
            return;
        }
        
        appliedSpell = spell;
        StatChange[] changes = new StatChange[] { statChange };
        spell.statsMod.ApplyStatChanges(changes);
    }

    public void UnApply()
    {
        if (appliedSpell != null)
        {
            StatChange[] remChanges = new StatChange[] { statChange };
            appliedSpell.statsMod.RemoveStatChanges(remChanges);
            appliedSpell = null;
        }
    }
    
    public override void Deactivate()
    {
        UnApply();
    }
}

public class ConnectedSpellsBuff : Relic, IDisposable
{
    private List<StatChange> statChange = new List<StatChange> {
        new StatChange { type = StatType.Projectiles, isFlatMod = true, flatChange = 1 },
        new StatChange { type = StatType.BulletDamage, isFlatMod = true, flatChange = 1 },
        new StatChange { type = StatType.ProjectileSize, isFlatMod = false, multChange = 0.1f },
        new StatChange { type = StatType.LifeTime, isFlatMod = false, multChange = 0.1f },
    };
    private HashSet<SpellItemV2> appliedSpells = new();

    public string Name => GiveName();
    public override string GiveName()
    {
        return "Connected Spells get +1 all stats";
    }

    public ConnectedSpellsBuff()
    {
        WandV2.OnWandRebuild += Activate;
    }
    
    public override void Dispose()
    {
        WandV2.OnWandRebuild -= Activate;
    }

    public override void Activate()
    {
        UnApplyAll();
        foreach (SpellItemV2 spell in PlayerControllerV2.instance.wand.spells.Where(spell => spell != null))
        {
            if(spell.triggeredSpells.Count > 0)
            {
                Apply(spell);
            }
            foreach (SpellItemV2 spell2 in spell.triggeredSpells.Where(spell => spell != null))
            {
                Apply(spell2);
            }
        }
    }

    public void Apply(SpellItemV2 spell)
    {
        if(spell == null || appliedSpells.Contains(spell)) return;

        appliedSpells.Add(spell);
        spell.statsMod.ApplyStatChanges(statChange.ToArray());
    }

    public void UnApplyAll()
    {
        foreach (SpellItemV2 appliedSpell in appliedSpells)
        {
            appliedSpell.statsMod.RemoveStatChanges(statChange.ToArray());
        }
        appliedSpells.Clear();
    }
    
    public override void Deactivate()
    {
        UnApplyAll();
    }
}

public class AdjacentSpellBuff : Relic
{
    private List<StatChange> statChange = new List<StatChange> {
        new StatChange { type = StatType.Projectiles, isFlatMod = true, flatChange = 1 },
        new StatChange { type = StatType.BulletDamage, isFlatMod = true, flatChange = 1 },
        new StatChange { type = StatType.ProjectileSize, isFlatMod = false, multChange = 0.1f },
        new StatChange { type = StatType.LifeTime, isFlatMod = false, multChange = 0.1f },
    };
    private Dictionary<SpellItemV2, int> appliedSpells = new();

    public string Name => GiveName();
    public override string GiveName()
    {
        return "+1 all stats for each adjacent spell";
    }

    public AdjacentSpellBuff()
    {
        WandV2.OnWandRebuild += Activate;
    }
    
    public override void Dispose()
    {
        WandV2.OnWandRebuild -= Activate;
    }

    public override void Activate()
    {
        UnApplyAll();
        for (int i = 0; i < PlayerControllerV2.instance.wand.spells.Count; i++)
        {
            SpellItemV2 spell = PlayerControllerV2.instance.wand.spells[i];
            if (spell == null) continue;
            
            SpellItemV2 nextSpell = null;
            SpellItemV2 prevSpell = null;
            
            //If there's a slot after this one, get it
            if((i+1) < PlayerControllerV2.instance.wand.spells.Count)
            {
                nextSpell = PlayerControllerV2.instance.wand.spells[i+1];
            }
            //If there's a slot before this one, get it
            if((i-1) >= 0)
            {
                prevSpell = PlayerControllerV2.instance.wand.spells[i-1];
            }
            
            //Apply once for each adjacent spell
            if(nextSpell != null)
            {
                Apply(spell);
            }
            
            if(prevSpell != null)
            {
                Apply(spell);
            }
        }
    }

    public void Apply(SpellItemV2 spell)
    {
        if(spell == null) return;
        
        appliedSpells[spell] = appliedSpells.ContainsKey(spell) ? appliedSpells[spell] + 1: 1;
        spell.statsMod.ApplyStatChanges(statChange.ToArray());
    }

    public void UnApplyAll()
    {
        foreach (KeyValuePair<SpellItemV2, int> spell in appliedSpells)
        {
            if (spell.Key == null) continue;
            for (int i = 0; i < spell.Value; i++)
            {
                spell.Key.statsMod.RemoveStatChanges(statChange.ToArray());
            }
        }
        appliedSpells.Clear();
    }
    
    public override void Deactivate()
    {
        UnApplyAll();
    }
}

public class OverspillMana : Relic
{
    public override string GiveName()
    {
        return "Overflow mana is conserved";
    }
}

//Note: This won't work when swapping wands!
public class ExtraWandRegen : Relic
{
    private StatChange statChange = new StatChange { type = StatType.ManaRegen, isFlatMod = true, flatChange = 5 };

    public override string GiveName()
    {
        return "Wand generates +5 mana/s";
    }
    
    public override void Activate()
    {
        PlayerControllerV2.instance.wand.statsMod.ApplyStatChanges(new StatChange[]
        { statChange });
    }
    
    public override void Deactivate()
    {
        PlayerControllerV2.instance.wand.statsMod.RemoveStatChanges(new StatChange[]
            { statChange });
    }
}

public class ExtraManaRegen : Relic
{
    int index = 0;
    private StatChange statChange = new StatChange { type = StatType.ManaRegen, isFlatMod = false, multChange = 1.2f };
    
    SpellItemV2 appliedSpell = null;
    public override string GiveName()
    {
        return $"Slot {index+1} gets +20% mana regen";
    }

    public ExtraManaRegen(int index)
    {
        this.index = index;
        WandV2.OnWandRebuild += OnWandRebuild;
    }

    public override void Dispose()
    {
        WandV2.OnWandRebuild -= OnWandRebuild;
    }

    public override void OnWandRebuild()
    {
        UnApply();
        Apply();
    }

    public override void Activate()
    {
        Apply();
    }

    void Apply()
    {
        if (appliedSpell == null &&
            index < PlayerControllerV2.instance.wand.spells.Count
            && PlayerControllerV2.instance.wand.spells[index] != null)
        {
            appliedSpell = PlayerControllerV2.instance.wand.spells[index];
            appliedSpell.statsMod.ApplyStatChanges(new StatChange[]
                { statChange });
        }
    }

    void UnApply()
    {
        if (appliedSpell != null)
        {
            appliedSpell.statsMod.RemoveStatChanges(new StatChange[]
                { statChange });
            appliedSpell = null;
        }
    }
    
    public override void Deactivate()
    {
        UnApply();
    }
}

public class EliteDamageUp : Relic
{
    public float multAdd = 4f;
    public override string GiveName()
    {
        return "+400% damage to Elites";
    }
}

public class EnemyDmgUp : Relic
{
    public float multAdd = 1f;

    public override string GiveName()
    {
        return "+100% damage to Enemies";
    }
}

public class LongerWavesOnKill : Relic
{
    public override string GiveName()
    {
        return "+1s to Wave Duration on Kill";
    } 
    
    public override void OnKill(HealthV2.HpChangeParams _)
    {
        GameManager.Instance.WaveCombat.IncreaseWaveTime(1f);
    }
}

public class BlackHoleOn10Kills : Relic
{
    private int stacks = 0;
    public override string GiveName()
    {
        return "Every 10 kills summon a black hole";
    } 
    
    public override void OnKill(HealthV2.HpChangeParams healthChangedParams)
    {
        stacks++;
        if (stacks >= 10)
        {
            stacks = 0;
            BlackHoleSpellItem bhole = UnityEngine.Object.Instantiate(Resources.Load<BlackHoleSpellItem>("BlackHoleSpellItem"), RelicsManager.instance.transform);

            ProjectileRecipe recipe = bhole.GetProjectileRecipe();
            
            // Vector3 targetPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            // Vector3 dir = targetPos - PlayerControllerV2.instance.gameObject.transform.position;
            // targetPos.z = 0;
            
            //healthChangedParams.Attacker.

            SpellItemV2.CastCommand castCommand = new SpellItemV2.CastCommand()
            {
                Dir = Vector2.left, 
                Pos = healthChangedParams.Attacker.transform.position,
            };
            bhole.Shoot(recipe, castCommand, bhole.NumOfProjectiles, bhole.Spread, bhole.Inaccuracy);
        }
    }
}

public class RelicsManager : MonoBehaviour
{
    public static RelicsManager instance;

    public List<Relic> relics = new();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        relics.Add(new BuffFirstSlotOnHit());
        relics.Add(new BuffSecondSlotMoreProj());
        //relics.Add(new ConnectedSpellsBuff()); Kinda bad, just means you need 2 in a wand
        relics.Add(new AdjacentSpellBuff());
        relics.Add(new OverspillMana());
        relics.Add(new ExtraWandRegen());
        relics.Add(new EnemyDmgUp());
        relics.Add(new EliteDamageUp());
        //relics.Add(new ExtraManaRegen(UnityEngine.Random.Range(0, 5)));
        //relics.Add(new LongerWavesOnKill());
        relics.Add(new BlackHoleOn10Kills());
    }

    void Start()
    {
        HealthV2.OnAnyDeath += OnKillCheck;
        foreach(Relic relic in relics)
        {
            relic.Activate();
        }
    }
    
    void OnDestroy()
    {
        HealthV2.OnAnyDeath -= OnKillCheck;
        foreach (Relic relic in relics)
        {
            relic.Dispose();
        }
    }

    void OnKillCheck(HealthV2.HpChangeParams healthChangedParams)
    {
        if (healthChangedParams.Victim == null) return;
        if (healthChangedParams.Victim.gameObject.HasHTag(HTags.Enemy))
        {
            OnKill(healthChangedParams);
        }
    }

    public void OnHit()
    {
        foreach (Relic relic in relics)
        {
            relic.OnHit();
        }
    }
    
    public void OnKill(HealthV2.HpChangeParams healthChangedParams)
    {
        foreach (Relic relic in relics)
        {
            relic.OnKill(healthChangedParams);
        }
    }

  
}
