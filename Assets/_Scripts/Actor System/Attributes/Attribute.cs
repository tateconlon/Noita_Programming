using System;
using Sirenix.OdinInspector;
using UnityEngine;

// https://github.com/tranek/GASDocumentation#concepts-a
// https://docs.unrealengine.com/5.2/en-US/gameplay-attributes-and-attribute-sets-for-the-gameplay-ability-system-in-unreal-engine/
// https://ck.uesp.net/wiki/Actor_Value
// https://github.com/sjai013/unity-gameplay-ability-system#attribute-system

[CreateAssetMenu(fileName = "Attribute", menuName = "ScriptableObject/Actor System/Attribute", order = 0)]
public class Attribute : ScriptableObject
{
    public string displayName;
    public float defaultValue;
    public Vector2 valueMinMax = new(0f, float.PositiveInfinity);
}

public abstract class IAttribute : MonoBehaviour
{
    public bool isInit;  // TODO: throw warning if getting or modifying if false. Throw warning if true and trying to re-init

    private void OnDisable()
    {
        isInit = false;
    }
}

public enum AttributeEnum
{
    HEALTH,
}

public class HealthAttribute : IAttribute
{
    public float value;
    public event Action OnDeath;
    public event Action OnHurt;

    //returns damage dealt
    //SNKRX style damage calculation
    public float DamageCalulator(float dmg, IActor sender)
    {
        float retVal;
        float strVal = 1;

        float strAdd = 0;
        float strMult = 1;
        
        StrengthAttribute str = sender.actor.GetAttribute<StrengthAttribute>();
        if (str != null)
        {
            strVal = sender.actor.GetAttribute<StrengthAttribute>().value;
        }
        if (sender is ShotgunBrain shotgun)
        {
            strMult *= 2;
        }

        float strFinal = (strVal + strAdd) * strMult;
        return dmg * strFinal ;
    }

    //returns uncapped damage dealt (big damage numbers that aren't capped at unit's health, etc.)
    public float TakeDamage(float dmg, IActor sender)
    {
        float dmgTaken = DamageCalulator(dmg, sender);
        value -= dmgTaken;
        if (dmgTaken > 0)
        {
            OnHurt?.Invoke();
        }
        if (value <= 0)
        {
            //owner.removetag(Alive);  Maybe here, maybe in the brain. Will see.
            OnDeath?.Invoke();
        }
        
        return dmgTaken;
    }
}

public class StrengthAttribute : IAttribute
{
    public float value;
    public StrengthAttribute(float str)
    {
        value = str;
    }
}
[Serializable] public class AttributeDict : UnitySerializedDictionary<Attribute, AttributeComponent> { }