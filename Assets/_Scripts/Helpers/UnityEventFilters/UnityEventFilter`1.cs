using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// Inspired by https://unity-atoms.github.io/unity-atoms/tutorials/conditions
public class UnityEventFilter<T0> : MonoBehaviour
{
    public List<FilteredEvent<T0>> filteredEvents = new();

    public void Invoke(T0 arg0)
    {
        foreach (FilteredEvent<T0> filteredEvent in filteredEvents)
        {
            filteredEvent.Invoke(arg0);
        }
    }
}

[Serializable]
public class FilteredEvent<T0>
{
    public FilterValue<T0> filterValue0;
    
    public UnityEvent<T0> onFilterMatch;

    public void Invoke(T0 arg0)
    {
        if (filterValue0.Matches(arg0))
        {
            onFilterMatch.Invoke(arg0);
        }
    }
}