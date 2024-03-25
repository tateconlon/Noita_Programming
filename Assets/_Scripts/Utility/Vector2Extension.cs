using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float num = Mathf.Sin(degrees * ((float)Math.PI / 180f));
        float num2 = Mathf.Cos(degrees * ((float)Math.PI / 180f));
        float x = v.x;
        float y = v.y;
        v.x = num2 * x - num * y;
        v.y = num * x + num2 * y;
        return v;
    }
    
    public static Vector3 RandomizeDirection(this Vector2 direction, float degrees)
    {
        float max = degrees / 2f;
        float min = -max;
        Vector2 v = new Vector2(direction.x, direction.y);
        v = v.Rotate(Random.Range(min, max));
        return new Vector3(v.x, v.y, 0f);
    }

    public static Vector3 V3(this Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }
    
    public static float GetAngle(this Vector2 a, Vector2 b)
    {
        return Vector2.Angle(a, b);
    }
    
    public static float GetSignedAngle(this Vector2 a, Vector2 b)
    {
        return Vector2.SignedAngle(a, b);
    }
    
    public static Quaternion DirToRotation(this Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }
}