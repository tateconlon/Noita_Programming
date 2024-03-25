using System;


public class StatMod
{
    //This is so that we can have a stat that is 0,
    //but still be able to remove it. If we do multiplierReduction = 0, then we can't remove
    //Any stats from it, since effecting it requires multiplying it. If it's 0, then we can never
    //change it.
    public const float ZERO_MULT_RED = float.Epsilon;
    
    
    //Note: Floatbonus being an int is carried over from 20MTD.
    //I want to keep it because I think I like that increases will be more coarse
    //and hopefully that will make the game feel more simple
    private float _flatBonus;

    private float _multiplierBonus;

    private float _multiplierReduction = 1f;

    public float FlatBonus => _flatBonus;
    
    public float MultiplierBonus => 1f + _multiplierBonus;
    
    public float MultiplierReduction => _multiplierReduction;

    public event EventHandler ChangedEvent;

    public float Modify(float baseValue)
    {
        return (baseValue + (float)_flatBonus) * (1f + _multiplierBonus) * _multiplierReduction;
    }

    public float ModifyInverse(float baseValue)
    {
        return (baseValue + (float)_flatBonus) / ((1f + _multiplierBonus) * _multiplierReduction);
    }

    public void AddFlatBonus(float value)
    {
        _flatBonus += value;
        this.ChangedEvent?.Invoke(this, null);
    }

    public void AddMultiplierBonus(float value)
    {
        _multiplierBonus += value;
        this.ChangedEvent?.Invoke(this, null);
    }

    //"Meshes" the two stat mods together. This is used for combining stats from multiple sources
    public StatMod Join(StatMod other)
    {
        StatMod retVal = new StatMod();
        
        retVal._flatBonus = this._flatBonus + other._flatBonus;
        retVal._multiplierBonus = this._multiplierBonus + other._multiplierBonus;
        retVal._multiplierReduction = this._multiplierReduction * other._multiplierReduction;

        return retVal;
    }

    
    //Note: in 20MTD this is handled differently. Only in 1 case can you remove a mult reduction,
    //And the math is wrong. Maybe his math is "close enough" and removes this edge case.
    //He never tries to "remove" a 100% reduction, which we're trying to account for here
    //This multiplicative reduction means that it'll never be less than 0, since the reductions
    //Stack multiplicatively.
    public void AddMultiplierReduction(float value)
    {
        //When removing a reduction, and we're removing a 100% reduction, and the reduction is currently 0
        //Set it to 1 so it's like we removed the 100% reduction
        //BUG: When we remove the 100% reduction, we don't revert to the previous held value
        //BUG: eg: 0.2->0.1->0->[remove 100% reduction]->1. SHOULD BE 0.1!
        if(value >= (1/StatMod.ZERO_MULT_RED) && _multiplierReduction.IsApprox(0))
        {
            _multiplierReduction = 1f;
            this.ChangedEvent?.Invoke(this, null);
            return;
        }
        
        value = value.IsApprox(0) ? StatMod.ZERO_MULT_RED : value;
        _multiplierReduction *= value;
        
        //Make sure _multiplierReduction is never 0
        if (_multiplierReduction.IsApprox(0))
        {
            _multiplierReduction = StatMod.ZERO_MULT_RED; //This is so that the stat can basically be 0,
                                            //but we can still restore from it.
        }
    }
    public void Clear()
    {
        _flatBonus = 0;
        _multiplierBonus = 0f;
        _multiplierReduction = 1f;
        this.ChangedEvent?.Invoke(this, null);
    }
}