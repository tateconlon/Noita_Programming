using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions
{
    public static Vector3 GetCenterPoint<T>(this IEnumerable<T> enumerable, Func<T, Vector3> centerSelector)
    {
        Bounds bounds = new();  // Note that this constructs bounds with center of (0, 0, 0) and must be changed
        bool hasSetCenter = false;

        foreach (T element in enumerable)
        {
            if (!hasSetCenter)
            {
                bounds.center = centerSelector.Invoke(element);
                hasSetCenter = true;
            }
            else
            {
                bounds.Encapsulate(centerSelector.Invoke(element));
            }
        }

        return bounds.center;
    }
}
