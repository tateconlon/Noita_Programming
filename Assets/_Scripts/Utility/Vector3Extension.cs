using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    /// <summary>
    /// Find the closest unit in the laser's range and angular cone
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="radius"></param>
    /// <param name="dir">Needed for the cone Angle</param>
    /// <param name="angleDeg">angleDeg represents total cone angle.</param>
    /// <returns></returns>
    public static GameObject FindClosestEnemyInSize(this Vector3 pos, float radius, Vector2 dir = default, float angleDeg = 360f, HashSet<GameObject> ignore = null)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, radius);
        
        GameObject closest = null;
        float distance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Hurtbox hBox))
            {
                if (hBox.Owner.HasHTag(HTags.Enemy))
                {
                    if (ignore != null && ignore.Contains(hBox.Owner))
                    {
                        continue;
                    }
                    float curDistance = (hBox.transform.position - pos).sqrMagnitude;
                    
                    //See if the angle is within the angular range
                    Vector2 toTarget = (hBox.transform.position - pos).normalized;
                    float angle = Vector2.Angle(dir, toTarget);
            
                    //angleDeg is total cone angle, so we divide it by 2 to get half the cone angle
                    if (angle <= angleDeg/2 && curDistance < distance)
                    {
                        closest = hBox.Owner;
                        distance = curDistance;
                    }
                }
            }
        }

        return closest;
    }
}