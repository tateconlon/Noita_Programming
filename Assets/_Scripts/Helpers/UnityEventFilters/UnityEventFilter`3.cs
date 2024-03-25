using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// Inspired by https://unity-atoms.github.io/unity-atoms/tutorials/conditions
public class UnityEventFilter<T0, T1, T2> : MonoBehaviour
{
    public List<FilteredEvent<T0, T1, T2>> filteredEvents = new();

    public void Invoke(T0 arg0, T1 arg1, T2 arg2)
    {
        foreach (FilteredEvent<T0, T1, T2> filteredEvent in filteredEvents)
        {
            filteredEvent.Invoke(arg0, arg1, arg2);
        }
    }
}

[Serializable]
public class FilteredEvent<T0, T1, T2>
{
    public FilterValue<T0> filterValue0;
    public FilterValue<T1> filterValue1;
    public FilterValue<T2> filterValue2;

    public UnityEvent<T0, T1, T2> onFilterMatch;
    
    public void Invoke(T0 arg0, T1 arg1, T2 arg2)
    {
        if (filterValue0.Matches(arg0) && filterValue1.Matches(arg1) && filterValue2.Matches(arg2))
        {
            onFilterMatch.Invoke(arg0, arg1, arg2);
        }
    }
}