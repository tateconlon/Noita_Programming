using System.Collections.Generic;
using UnityEngine;

// https://github.com/tranek/GASDocumentation#4111-target-data
// https://docs.unrealengine.com/5.3/en-US/API/Plugins/GameplayAbilities/Abilities/FGameplayAbilityTargetData/

/// <summary>
/// Generic targeting data, such as Actors targeted by attack and location/direction/origin info.
/// </summary>
public class TargetData
{
    public HashSet<Actor> Targets = new();
    public HashSet<Actor> Ignored = new();
    
    //TODO TATE: Why are these nullable?
    public Vector3? Origin = null;
    public Vector3? EndPoint = null;
    public Vector3? Direction = null;
    
    public TargetData()
    {
        
    }
    
    public TargetData(Actor target)
    {
        Targets = new HashSet<Actor> { target };
    }
    
    public TargetData(IEnumerable<Actor> targets)
    {
        Targets = new HashSet<Actor>(targets);
    }
    
    public TargetData(Vector3 direction)
    {
        Direction = direction;
    }
}

// TATE: When/if we make it generic, we can use this to pass ability values
// public struct AbilityPacket
// {
//     
//     // --------- Generic Ability Values --------- (will take shape over time)
//     private float dmg;
//     private float dmg1;
//     private float dmg2;
//     
//     // --------- Portal Ability Values ---------
//     private float portalVector;
//     private float portalAngle;
//     // --------- Bomb Ability Values ---------
//     private float bombVector;
//     private float bombAngle;
// }