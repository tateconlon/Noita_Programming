using System;

[Serializable]
public class PeriodicEffect : InstantEffect
{
    // Note: Can't serialize this because it would lead to recursive serialization
    [NonSerialized] public DurationEffect parentDurationEffect;  // TODO: set ownership somewhere
}