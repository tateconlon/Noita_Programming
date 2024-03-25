using System;

[Serializable]
public struct StatChange
{
    public StatType type;

    public bool isFlatMod;

    public float flatChange;

    public float multChange;
}