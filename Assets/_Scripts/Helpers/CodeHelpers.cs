using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.Linq;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public enum Direction
{
    Up,Right,Down,Left,
}
public static class CodeHelpers
{
    public static GameObject FindClosestObject(this Transform me , GameObject[] GOs)
    {
        if (GOs.Length == 0)
        {
            return null;
        }

        float closestDist = float.MaxValue;
        GameObject closestObject = null;
        float dist = 0;
        foreach (GameObject go in GOs)
        {
            dist = (go.transform.position - me.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestObject = go;
                closestDist = dist;
            }
        }

        return closestObject;
    }
    
    public static GameObject FindClosestObject(this Transform me , Component[] components)
    {
        List<GameObject> GOs = new List<GameObject>();

        for (int i = 0; i < components.Length; i++)
        {
            GOs.Add(components[i].gameObject);
        }
        
        return me.FindClosestObject(GOs.ToArray());
    }
    
    public static Vector2 ToVector2xy(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
    
    public static Vector2 V2(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    public static Vector3 MultiplyComponents(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector2 OffsetPosition(Vector2 position, float offset)
    {
        float xOffset = Random.Range(-offset, offset);
        float yOffset = Random.Range(-offset, offset);
        return position + new Vector2(xOffset, yOffset);
    }
    
    // If you edit a prefab instance (e.g. change its transform) by script, you must use this afterwards to save.
    // Otherwise, the change is treated like a "play mode" edit and is discarded once the scene is closed.
    // NOTE: I could still only get it to save variables that were Serialized; private variables still get wiped.

    public static void SaveToPrefabInstance(GameObject targetGameObject, UnityEngine.Object targetObject)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Should not save changes to prefab instance in Play mode; stopping operation.");
            return;
        }
        
        #if UNITY_EDITOR
        PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
        EditorSceneManager.MarkSceneDirty(targetGameObject.scene);
        #endif
    }

    public static T Pop<T>(this List<T> list, int index)
    {
        if (index < 0 || index >= list.Count)
        {
            Debug.LogWarning("index was " + index + " and list count was " + list.Count);
            return default(T);
        }
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }
    
    public static T PopRandom<T>(this List<T> list)
    {
        int randIndex = Random.Range(0, list.Count);
        T item = list[randIndex];
        list.RemoveAt(randIndex);
        return item;
    }

    /// <summary>
    /// Returns true at the given chance.
    /// </summary>
    /// <example>
    /// <code>
    /// RandomBool(0.50) -> returns true 50% of the time
    /// RandomBool(0.25) -> returns true 25% of the time
    /// RandomBool(0.03) -> returns true 3% of the time
    /// </code>
    /// </example>
    // Ported from the SNKRX Random:bool() method
    public static bool RandomBool(float chance = 0.5f)
    {
        return Random.Range(0f, 1f) < chance;
    }

    public static T RemoveRandom<T>(this List<T> list)
    {
        int removeIndex = Random.Range(0, list.Count);

        T removedObject = list[removeIndex];
        list.RemoveAt(removeIndex);

        return removedObject;
    }
    
    public static int ToLayerMask(this int layer)
    {
        return 1 << layer;
    }

    public static void AddLayer(ref this LayerMask layerMask, int layerToAdd)
    {
        layerMask |= (1 << layerToAdd);
    }
    
    /// <summary>
    /// Returns bool if layer is within layermask
    /// </summary>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return ((mask.value & (1 << layer)) > 0);
    }

    /// <summary>
    /// Returns true if gameObject is within layermask
    /// </summary>
    public static bool Contains(this LayerMask mask, GameObject gameObject)
    {
        return ((mask.value & (1 << gameObject.layer)) > 0);
    }

    public static void SetLayerRecursively(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public static int DivideRoundUp(this int num, int divideBy)
    {
        return (num + divideBy - 1) / divideBy;
    }
    
    public static Vector3 ClosestToSurfacePoint(this Collider collider, Vector3 point, bool throwIfFallback = true)
    {
        SphereCollider sc = collider as SphereCollider;
        if (sc != null)
        {
            Vector3 closest = sc.ClosestPoint(point);
            if (DistanceSquared(closest, point) < 0.001)
            {
                //Inside of sphere
                Vector3 toPointDir = (point - sc.transform.position);
                closest = sc.ClosestPoint(sc.transform.position + toPointDir * (sc.radius + Epsilon));
            }

            return closest;
        }

        CapsuleCollider cc = collider as CapsuleCollider;
        if (cc != null)
        {
            Vector3 closest = cc.ClosestPoint(point);
            if (DistanceSquared(closest, point) < 0.001)
            {
                Vector3 dir = cc.direction == 0 ? new Vector3(1, 0, 0) :
                    cc.direction == 1 ? new Vector3(0, 1, 0) : new Vector3(0, 0, 1);

                Vector3 vec = (cc.transform.rotation * dir) * (cc.height / 2f - cc.radius);
                Vector3 c1 = cc.transform.position + vec;
                Vector3 c2 = cc.transform.position - vec;
                Vector3 drop = ClosestOnLineSegment(point, c1, c2);

                Vector3 toPointDir = (point - drop);
                closest = cc.ClosestPoint(drop + toPointDir * (cc.radius + Epsilon));
            }

            return closest;
        }

        //BoxCollider bc = collider as BoxCollider;
        //if(bc != null)
        //{
        //	Vector3 closest = bc.ClosestPoint(point);
        //	if (DistanceSquared(closest, point) < 0.001)
        //	{
        //		Vector3 localPoint = bc.transform.InverseTransformPoint(point);
        //		Vector3 localDir = localPoint.normalized;
        //		float upDot = Vector3.Dot(localPoint, Vector3.up);
        //		float fwdDot = Vector3.Dot(localPoint, Vector3.forward);
        //		float rightDot = Vector3.Dot(localPoint, Vector3.right);

        //		float upPower = Mathf.Abs(upDot);
        //		float fwdPower = Mathf.Abs(fwdDot);
        //		float rightPower = Mathf.Abs(rightDot);
        //		if (upPower > fwdPower && upPower > rightPower)
        //		{
        //			float distance = bc.size.y * bc.transform.localScale.y * 0.5f;
        //			closest = bc.transform.TransformVector(point + (Vector3.up * Mathf.Sign(upDot) * (distance - upPower)));
        //		}
        //		else if (fwdPower > upPower && fwdPower > rightPower)
        //		{
        //			float distance = bc.size.x * bc.transform.localScale.x * 0.5f;
        //			closest = bc.transform.TransformVector(point + (Vector3.forward * Mathf.Sign(fwdDot) * (distance - fwdPower)));
        //		}
        //		else
        //		{
        //			float distance = bc.size.z * bc.transform.localScale.z * 0.5f;
        //			closest = bc.transform.TransformVector(point + (Vector3.right * Mathf.Sign(rightDot) * (distance - rightPower)));
        //		}
        //	}
        //	return Vector3.zero;
        //}
        Debug.LogError("Unsupported collider type " + collider);
        return collider.ClosestPoint(point);
    }

    public static int LongestPathIndex(this PolygonCollider2D polygonCollider2D)
    {
        int longestPathIndex = 0;
        int longestPathLength = 0;

        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            if (polygonCollider2D.GetPath(i).Length > longestPathLength)
            {
                longestPathIndex = i;
                longestPathLength = polygonCollider2D.GetPath(i).Length;
            }
        }

        return longestPathIndex;
    }

    public static float AngleBetweenAngles(float a1, float a2)
    {
        if (a1 < 0) a1 += 360;
        if (a2 < 0) a2 += 360;
        return 180 - Mathf.Abs(Mathf.Abs(a1 - a2) - 180);
    }

    public static float Magnitude2D(this Vector3 vector)
    {
        vector.z = 0;
        return vector.magnitude;
    }
    
    public static float Magnitude2D(this Vector2 vector)
    {
        return vector.magnitude;
    }

    public static float DistanceSquared(this Vector3 a, Vector3 b)
    {
        float vectorX = (a.x - b.x);
        float vectorY = (a.y - b.y);
        float vectorZ = (a.z - b.z);
        return (((vectorX * vectorX) + (vectorY * vectorY)) + (vectorZ * vectorZ));
    }
    
    public static float DistanceSquared2D(this Vector3 a, Vector3 b)
    {
        float vectorX = (a.x - b.x);
        float vectorY = (a.y - b.y);
        float vectorZ = 0;
        return (((vectorX * vectorX) + (vectorY * vectorY)) + (vectorZ * vectorZ));
    }
    
    public static float Distance2D(this Vector3 a, Vector3 b)
    {
        return Vector2.Distance(a, b);
    }

    public static float DistanceSquared2D(this Vector2 a, Vector2 b)
    {
        float vectorX = (a.x - b.x);
        float vectorY = (a.y - b.y);
        return (((vectorX * vectorX) + (vectorY * vectorY)));
    }
    
    #if UNITY_EDITOR
    public static List<T>  GetAllComponentsInStage <T>() where T : MonoBehaviour
    {
        List<T> list = new List<T>();
        foreach (T component in Resources.FindObjectsOfTypeAll<T>())
        {
            if (!StageUtility.GetCurrentStageHandle().Contains(component.gameObject)) continue;
            list.Add(component);
        }

        return list;
    }
    #endif

    public static Vector3 ClosestOnLineSegment(Vector3 p, Vector3 line1, Vector3 line2)
    {
        Vector3 closest = ProjectOnLine(p, line1, line2);
        if (!IsOnSegment(line1, closest, line2))
        {
            float d1 = DistanceSquared(line1, closest);
            float d2 = DistanceSquared(line2, closest);
            closest = d1 < d2 ? line1 : line2;
        }

        return closest;
    }

    public static Vector3 ProjectOnLine(Vector3 point, Vector3 line1, Vector3 line2)
    {
        Vector3 pointToLine = point - line1;
        Vector3 lineVector = line2 - line1;
        Vector3 onNormal = Vector3.ProjectOnPlane(pointToLine, lineVector);
        return (onNormal + line1);
    }

    public static bool IsOnSegment(Vector3 segStart, Vector3 point, Vector3 segEnd)
    {
        if (point.x <= Max(segStart.x, segEnd.x) + Epsilon &&
            point.x >= Min(segStart.x, segEnd.x) - Epsilon &&
            point.y <= Max(segStart.y, segEnd.y) + Epsilon &&
            point.y >= Min(segStart.y, segEnd.y) - Epsilon &&
            point.z <= Max(segStart.z, segEnd.z) + Epsilon &&
            point.z >= Min(segStart.z, segEnd.z) - Epsilon)
            return true;

        return false;
    }

    //MATH

    static float Min(double a, double b)
    {
        return a > b ? (float) b : (float) a;
    }

    static float Max(double a, double b)
    {
        return a < b ? (float) b : (float) a;
    }

    public static bool IsApprox(this float current, float other)
    {
        return Mathf.Approximately(current, other);
    }
    
    public static bool IsApprox(this Vector3 current, Vector3 other)
    {
        return Mathf.Approximately(current.x, other.x) && Mathf.Approximately(current.y, other.y) && Mathf.Approximately(current.z, other.z);
    }

    const float Epsilon = 0.00001f;

    public static float Percent(this float current, float from, float to)
    {
        if ((to - from).IsApprox(0))
        {
            return 0;
        }

        return (current - from) / (to - from);
    }
    
    public static float Percent(this int curr, float from, float to)
        {
            return ((float)curr).Percent(from, to);
        }
    
    public static float Percent(this float current, MinMax minMax)
    {
        return current.Percent(minMax.min, minMax.max);
    }
    
    public static float Percent(this int current, MinMax minMax)
    {
        return current.Percent(minMax.min, minMax.max);
    }

    public static float PercentClamped(this float current, float from, float to)
    {
        if ((to - from).IsApprox(0))
        {
            if ((current - to).IsApprox(0))
            {
                return 1; //all 3 numbers are the same, so we return 1
            }
            
            return 0;
        }

        return Mathf.Clamp((current - from) / (to - from), 0, 1);
    }
    
    public static float PercentClamped(this int curr, float from, float to)
    {
        return ((float)curr).PercentClamped(from, to);
    }
    
    public static float PercentClamped(this float current, MinMax minMax)
    {
        return current.PercentClamped(minMax.min, minMax.max);
    }
    
    public static float PercentClamped(this int current, MinMax minMax)
    {
        return current.PercentClamped(minMax.min, minMax.max);
    }

    public static void Clamp(this ref float current, float from, float to)
    {
        current = Mathf.Clamp(current, from, to); 
    }
    
    public static void Clamp(this ref int current, int from, int to)
    {
        current = Mathf.Clamp(current, from, to); 
    }
    
    public static float Clamped(this float current, float from, float to)
    {
        return Mathf.Clamp(current, from, to);
    }
    
    public static int Clamped(this int current, int from, int to)
    {
        return Mathf.Clamp(current, from, to);
    }

    public static Vector3 ClampedPositive(this Vector3 current)
    {
        return new Vector3(
            Mathf.Max(current.x, 0f), 
            Mathf.Max(current.y, 0f), 
            Mathf.Max(current.z, 0f));
    }

    public static float Squared(this float number)
    {
        return number * number;
    }
    
    public static int Squared(this int number)
    {
        return number * number;
    }

    public static float Pow(this float number, int exponent)
    {
        return Mathf.Pow(number, exponent);
    }

    public static float Abs(this float number)
    {
        return Mathf.Abs(number);
    }

    public static float Sign(this float number)
    {
        return Mathf.Sign(number);
    }

    public static float Sqrt(this float number)
    {
        return Mathf.Sqrt(number);
    }

    public static void ForceInputFieldToUpper(this TMP_InputField inputField)
    {
        inputField.onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };
    }
    
    public static bool IsPointerOverButton()
    {
        return Application.isMobilePlatform ? 
            EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : 
            EventSystem.current.IsPointerOverGameObject();
    }
    
    public static bool IsInputFieldSelected()
    {
        // // The line below would probably be enough to handle mobile, but I'll use the code below to be consistent
        // if (TouchScreenKeyboard.isSupported && TouchScreenKeyboard.visible) return true;
        
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        
        if (selectedGameObject == null) return false;
        
        return selectedGameObject.TryGetComponent(out TMP_InputField _) ||
               selectedGameObject.TryGetComponent(out InputField _);
    }

    public static bool SameDirection(this Vector3 a, Vector3 b)
    {
        //TODO swap for dot product cuse its faster
        return Vector3.Angle(a, b) < 90;
    }
    
    public static bool SameDirection(this Vector2 a, Vector2 b)
    {
        //TODO swap for dot product cuse its faster
        return Vector2.Angle(a, b) < 90;
    }

    public static float GetAngle(this Vector3 a, Vector3 b)
    {
        return Vector3.Angle(a, b);
    }
    
    
    //Moved to Vector2Extensions
    // public static float GetAngle(this Vector2 a, Vector2 b)
    // {
    //     return Vector2.Angle(a, b);
    // }

    public static string ToIntString(this float val)
    {
        return val.ToString("0");
    }
    
    public static string ToTimeString(this float valSeconds)
    {
        int minutes = (int) valSeconds / 60 ;
        int seconds = (int) valSeconds - 60 * minutes;
        int milliseconds = (int) (1000 * (valSeconds - minutes * 60 - seconds));
        int hundredthsOfSeconds = (int) milliseconds / 10;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredthsOfSeconds);    //{2:000} ,milliseconds => 00:00:000
    }

    public static string ToMinSecs(this float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        return $"{timeSpan.Minutes:0}:{timeSpan.Seconds:00}";
    }
    
    public static string ToSecondsAndMilliseconds(this float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        return $"{timeSpan.Seconds:0}.{timeSpan.Milliseconds:00}";
    }
    
    public static string CountdownToString(this float number, int minDisplayedDigit = 0)
    {
        float countdown = Mathf.Ceil(number);
        if (countdown < 0 + minDisplayedDigit)
        {
            countdown = 0 + minDisplayedDigit;
        }
        return countdown.ToString();
    }

    public static bool IsBetween(this float check, float min, float max)
    {
        //TODO swap for dot product cuse its faster
        return check > min && check < max;
    }

    const float GIZMO_DISK_THICKNESS = 0.01f;
    public static void DrawCircleFilled(this Vector3 position, float radius, Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
        Gizmos.DrawSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }

    public static void DrawCircle(this Transform transform, float radius, Color color)
    {
        Gizmos.color = color;
        Vector3 origin = transform.position;
        Vector3 startRotation = transform.right * radius; // Where the first point of the circle starts
        Vector3 lastPosition = origin + startRotation;
        float angle = 0;
        while (angle <= 360)
        {
            angle += 360 / (radius * 3);
            Vector3 nextPosition = origin + (Quaternion.Euler(0, angle, 0) * startRotation);
            Gizmos.DrawLine(lastPosition, nextPosition);
            Gizmos.DrawSphere(nextPosition, 0.1f);
            lastPosition = nextPosition;
        }
    }
    
    public static void GizmosDrawBox(Transform trans, float width, Color color)
    {
        Gizmos.color = color;
        Gizmos.matrix = trans.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * width);
    }
    
    public static void DebugDrawBox(Vector2 point, Vector2 size, float zAngle, Color color, float duration)
    {
        var orientation = Quaternion.Euler(0, 0, zAngle);

        // Basis vectors, half the size in each direction from the center.
        Vector2 right = orientation * Vector2.right * size.x / 2f;
        Vector2 up = orientation * Vector2.up * size.y / 2f;

        // Four box corners.
        var topLeft = point + up - right;
        var topRight = point + up + right;
        var bottomRight = point - up + right;
        var bottomLeft = point - up - right;

        // Now we've reduced the problem to drawing lines.
        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
    
    public static void DebugDrawBox(Vector2 point, Vector2 size, Quaternion zRot, Color color, float duration)
    {
        var orientation = zRot.eulerAngles.z;
        DebugDrawBox(point, size, orientation, color, duration);
    }

    public static float Divide(this int num, int num2)
    {
        return (float) num / num2;
    }

    
    //why does this not work?
    public static Vector3 GetRayAndConeIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 coneOrigin,
        Vector3 coneVector, float coneAngle)
    {
        //reference : http://lousodrome.net/blog/light/2017/01/03/intersection-of-a-ray-and-a-cone/
        Vector3 CO = rayOrigin - coneOrigin;
        //Vector3 CO = coneOrigin - rayOrigin;
        float DdotV = Vector3.Dot(rayDirection, coneVector);
        float COdotV = Vector3.Dot(CO, coneVector);

        float a = DdotV.Squared() - (Mathf.Cos(coneAngle).Squared());
        float b = 2 * ((DdotV * COdotV) - Vector3.Dot(rayDirection, CO * Mathf.Cos(coneAngle).Squared()));
        float c = COdotV.Squared() - Vector3.Dot(CO, CO * Mathf.Cos(coneAngle).Squared());

        float delta = b.Squared() - 4 * a * c;
        if (delta < 0)
        {
            Debug.Log("No crossing");
            return Vector3.zero;
        }
        else if (delta.IsApprox(0))
        {
            Debug.Log("Only going through once");
            return Vector3.zero;
        }
        else
        {
            float rootDeltaOver2 = Mathf.Sqrt(coneAngle) / 2;
            float t1 = -b - rootDeltaOver2 * a;
            float t2 = -b + rootDeltaOver2 * a;
            if (t1 < 0 && t2 < 0)
            {
                Debug.Log("wrong end");
                return Vector3.zero;
            }

            Debug.DrawLine(coneOrigin, rayOrigin + rayDirection * t1, Color.red);
            Debug.DrawLine(coneOrigin, rayOrigin + rayDirection * t2, Color.red);
            if (t1 < 0) t1 = Mathf.Infinity;
            if (t2 < 0) t2 = Mathf.Infinity;
            Vector3 P = rayOrigin + rayDirection * (t1 < t2 ? t1 : t2);

            if (coneAngle < 90 && Vector3.Dot(P - coneOrigin, coneVector) <= 0)
            {
                Debug.Log("Shadow cone");
                return Vector3.zero; //Checking for reverse cone
            }

            return P;
        }
    }

    public static float Closest(this float value, float a, float b)
    {
        return value.Difference(a) <= value.Difference(b) ? a : b; 

    }

    public static void SetPos(this Transform t, float x = float.NaN, float y = float.NaN, float z = float.NaN)
    {
        if (x == float.NaN && y == float.NaN && z == float.NaN) return;

        Vector3 temp = t.position;
        temp.x = !float.IsNaN(x) ? x : temp.x;
        temp.y = !float.IsNaN(y) ? y : temp.y;
        temp.z = !float.IsNaN(z) ? z : temp.z;

        t.position = temp;
    }
    
    public static void SetLocalPos(this Transform t, float x = float.NaN, float y = float.NaN, float z = float.NaN)
    {
        if (x == float.NaN && y == float.NaN && z == float.NaN) return;

        Vector3 temp = t.localPosition;
        temp.x = !float.IsNaN(x) ? x : temp.x;
        temp.y = !float.IsNaN(y) ? y : temp.y;
        temp.z = !float.IsNaN(z) ? z : temp.z;

        t.localPosition = temp;
    }

    public static void SetAnchoredPos(this RectTransform t, float x = float.NaN, float y = float.NaN)
    {
        if (x == float.NaN && y == float.NaN) return;

        Vector2 temp = t.anchoredPosition;
        temp.x = !float.IsNaN(x) ? x : temp.x;
        temp.y = !float.IsNaN(y) ? y : temp.y;

        t.anchoredPosition = temp;
    }
    
    public static Vector2 WorldToUIPoint(this Camera camera, RectTransform canvasRect, Vector2 worldPoint)
    {
        Vector2 viewportPoint = camera.WorldToViewportPoint(worldPoint);

        return viewportPoint * canvasRect.sizeDelta;
    }

    static readonly Plane[] FrustumPlanes = new Plane[6];
    
    public static bool AreBoundsInsideCameraFrustum(this Camera camera, Bounds bounds)
    {
        GeometryUtility.CalculateFrustumPlanes(camera, FrustumPlanes);
        return GeometryUtility.TestPlanesAABB(FrustumPlanes, bounds);
    }

    public static void OurDestroy(this GameObject go)
    {
        go.OurDestroy();
    }

    //The functions return -1 when the target direction is left, +1 when it is right 
    public static float AngleDirection(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public static bool IsWithinDistance(Vector3 position1, Vector3 position2, float distance)
    {
        return DistanceSquared(position1, position2) < distance.Squared();
    }

    public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match)
    {
        foreach (var key in dict.Keys.ToArray()
            .Where(key => match(key, dict[key])))
            dict.Remove(key);
    }

    public static float LerpTo(this float startValue, float endValue, float interpolation)
    {
        return Mathf.Lerp(startValue, endValue, interpolation);
    }

    public static int ModifyBit(int currentBit, int position, int boolean)
    {
        int mask = 1 << position;
        return (currentBit & ~mask) | ((boolean << position) & mask);
    }

    #region UI

    public static void SetColor(this Image img, float r = -1f, float g = -1f, float b = -1f, float a = -1f)
    {
        if (r == -1f && g == -1f && b == -1f && a == -1f) return;

        Color c = img.color;
        c.r = r != -1f ? r : c.r;
        c.g = g != -1f ? g : c.g;
        c.b = b != -1f ? b : c.b;
        c.a = a != -1f ? a : c.a;
        
        img.color = c;
    }

    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }

    public static void ModifyValueHSV(this Graphic graphic, float valueDelta)
    {
        Color.RGBToHSV(graphic.color, out float hue, out float saturation, out float value);
        graphic.color = Color.HSVToRGB(hue, saturation, value + valueDelta);
    }

    public static Color AddHSV(this Color color, float hueDelta, float saturationDelta, float valueDelta)
    {
        Color.RGBToHSV(color, out float hue, out float saturation, out float value);
        return Color.HSVToRGB(hue + hueDelta, saturation + saturationDelta, value + valueDelta);
    }

    public static void ClearColor(this Texture2D texture2D, Color color)
    {
        Color[] pixelColors = new Color[texture2D.width * texture2D.height];
        
        for (int i = 0; i < pixelColors.Length; i++)
        {
            pixelColors[i] = color;
        }
        
        texture2D.SetPixels(pixelColors);
        
        texture2D.Apply();
    }

    public static void Clear(this RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
    
    #endregion

    //Vector rotations
    
    public static Vector2 RotateAroundPivot(this Vector2 startingDirection,float aDegree)
    {
        return Quaternion.Euler(0,0,aDegree) * startingDirection;
    }
    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin((angleRad)));
    }

    // Note that the returned vector is normalized
    public static Vector2 AngleToDirection2D(float angleDegrees)
    {
        return new Vector2(Mathf.Cos(angleDegrees * Mathf.Deg2Rad), Mathf.Sin(angleDegrees * Mathf.Deg2Rad));
    }
    
    public static float Direction2DToAngle(Vector2 moveVector)
    {
        return Vector2.SignedAngle(Vector2.right, moveVector);
    }

    public static Vector2 Direction(this Rigidbody2D rb2D)
    {
        return AngleToDirection2D(rb2D.rotation);
    }

    public static void AlignRotationWithVelocity(this Rigidbody2D rb2D)
    {
        rb2D.rotation = Direction2DToAngle(rb2D.velocity);
    }
    
    public static void RotateTowardsVelocity(this Rigidbody2D rb2D, float lerpValue)
    {
        rb2D.MoveRotation(Mathf.LerpAngle(rb2D.rotation, Direction2DToAngle(rb2D.velocity), lerpValue));
    }

    public static Quaternion GetRotationFromDir2D(Vector3 moveVector)
    {
        return Quaternion.Euler(0,0, Direction2DToAngle(moveVector));
    }


    //https://gamedevbeginner.com/how-to-rotate-in-unity-complete-beginners-guide/#rotate_towards_object
    //forward is the axis that you want to point at the target
    public static void RotateTowardsWithSpeed(this Transform trans, Transform target, float rotationSpeed, float timeDotDeltaTime, Vector3 forward)
    {
        float maxAnglesThisFrame = rotationSpeed * timeDotDeltaTime;
        Vector3 dir = (target.position - trans.position).normalized;
        Quaternion goalRotation = Quaternion.LookRotation(forward, dir);
        trans.rotation = Quaternion.RotateTowards(trans.rotation, goalRotation, maxAnglesThisFrame);
    }
    
    //Rotation in 2D has to always be around the Z axis, hence this special function
    public static void Rotate2DTowardsWithSpeed(this Transform trans, Transform target, float rotationSpeed,
        float timeDotDeltaTime, Vector3 fwd)
    {
        float maxDegreesThisFrame = rotationSpeed * timeDotDeltaTime;

        Vector3 newFwd = (target.position - trans.position);
        
        float currZAngle = trans.rotation.eulerAngles.z;
        
        //Used to make the correct axis face the target. eg: 0,1,0 returns -90
        //Which when the using Atan2 to face target with fwd=Xaxis, apply offset to then have it face the y axis
        //Note: Atan2 gracefully handles division by 0 errors. https://docs.unity3d.com/ScriptReference/Mathf.Atan2.html 
        float fwdOffset = -Mathf.Atan2(fwd.y, fwd.x);
        float ZAngleWithXAxisFwd = Mathf.Atan2(newFwd.y, newFwd.x);
        float calibratedAngleWithCustomFwdVector = (ZAngleWithXAxisFwd + fwdOffset) * Mathf.Rad2Deg;

        float resAngle = Mathf.MoveTowardsAngle(currZAngle, calibratedAngleWithCustomFwdVector, maxDegreesThisFrame);

        Quaternion q = Quaternion.AngleAxis(resAngle, Vector3.forward);    //Rotate on Z axis
        trans.rotation = q;
    }
    
    //Rotation in 2D has to always be around the Z axis, hence this special function
    public static void LookAt2D(this Transform trans, Transform target, Vector3 fwd)
    {
        Vector3 newFwd = (target.position - trans.position);

        //Used to make the correct axis face the target. eg: 0,1,0 returns -90
        //Which when the using Atan2 to face target with fwd=Xaxis, apply offset to then have it face the y axis
        //Note: Atan2 gracefully handles division by 0 errors. https://docs.unity3d.com/ScriptReference/Mathf.Atan2.html 
        float fwdOffset = -Mathf.Atan2(fwd.y, fwd.x);
        float ZAngleWithXAxisFwd = Mathf.Atan2(newFwd.y, newFwd.x);
        float calibratedAngleWithCustomFwdVector = (ZAngleWithXAxisFwd + fwdOffset) * Mathf.Rad2Deg;

        Quaternion q = Quaternion.AngleAxis(calibratedAngleWithCustomFwdVector, Vector3.forward);    //Rotate on Z axis
        trans.rotation = q;
    }

    public static Quaternion ToRotation2D(this float angleDegrees)
    {
        return Quaternion.Euler(0, 0, 90 + angleDegrees);  // Add 90 so that Right vector points in right dir
    }

    public static void CopyComponent(Component original, Component paste)
    {
        System.Type type = original.GetType();
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields(); 
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(paste, field.GetValue(original));
        }
        
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(paste, prop.GetValue(original, null), null);
        }

    
    }

    public static Dictionary<int, X> EnumToInt<T, X>(this Dictionary<T, X> dictionary)
    {
        Dictionary<int, X> newDict = new Dictionary<int, X>();
        foreach (KeyValuePair<T,X> pair in dictionary)
        {
            int integer = (int)Convert.ChangeType(pair.Key, typeof(int));
            newDict.Add(integer, pair.Value);
        }

        return newDict;
    }

    public static void Add<T, X>(this Dictionary<T, X> dictionary, KeyValuePair<T,X> value)
    {
        dictionary.Add(value.Key, value.Value);
    }

    public static void IncrementDictionary<T>(this Dictionary<T, float> dictionary, T type)
    {
        if (dictionary.ContainsKey(type))
        {
            dictionary[type] += 1.0f;
        }
        else
        {
            dictionary.Add(type, 1.0f);
        }
    }
    
    public static T GetComponentInChildrenExclusive<T>(this GameObject obj)where T:Component{
        List<T> tList = new List<T>();
        foreach (Transform child in obj.transform)
        {
            T script = child.GetComponent<T>();    
            if(script != null)
            {
                return script;
            }
        }
        return null;
    }
    
    public static bool TryGetComponentDownAndUp<T>(this Component obj, out T component) where T : Component
    {
        if (obj.TryGetComponentInChildren<T>(out component))
        {
            return true;
        }

        return obj.TryGetComponentInParents<T>(out component);
    }
    
    public static bool TryGetComponentDownAndUp(this Component obj, Type tp, out Component component)
    {
        if (obj.TryGetComponentInChildren(tp, out component))
        {
            return true;
        }

        return obj.TryGetComponentInParents(tp, out component);
    }

    //Can get component at runtime (not statically typed)
    public static bool TryGetComponentInParents(this Component obj, Type tp, out Component component)
    {
        component = obj.GetComponentInParent(tp);

        return component != null;
    }
    
    //Can get component at runtime (not statically typed)
    public static bool TryGetComponentInChildren(this Component obj, Type tp, out Component component)
    {
        component = obj.GetComponentInChildren(tp);

        return component != null;
    }

    public static bool TryGetComponentInChildren<T>(this Component obj, out T component) where T : Component
    {
        return obj.gameObject.TryGetComponentInChildren<T>(out component);
    }

    public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component) where T : Component
    {
        component = obj.GetComponentInChildren<T>();

        return component != null;
    }

    public static T InstantiateWorkerComponent<T>(bool dontDestroyOnLoad, HideFlags hideFlags) where T : Component
    {
        GameObject gameObject = new(typeof(T).Name)
        {
            hideFlags = hideFlags
        };

        if (dontDestroyOnLoad)
        {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }
        
        if (hideFlags == HideFlags.DontSave && !dontDestroyOnLoad)
        {
            Debug.LogWarning("Note that HideFlags.DontSave will make GameObject NOT be destroyed/call OnDestroy");
        }
        
        return gameObject.AddComponent<T>();
    }

    /// <summary>
    /// True if this GameObject's scene is unloading as the result of unloading the scene, loading a new scene,
    /// or quitting the application.
    ///
    /// Use for cases like instantiating new GameObjects to prevent spawning in response to OnDestroy.
    /// </summary>
    public static bool IsSceneUnloading(this GameObject gameObject)
    {
        return !gameObject.scene.isLoaded;
    }

    public static Rect ToEncompassingRect(this Vector2[] points)
    {
        float xMin = points[0].x;
        float yMin = points[0].y;
        float xMax = points[0].x;
        float yMax = points[0].y;

        for (int i = 1; i < points.Length; i++)
        {
            xMin = Mathf.Min(points[i].x, xMin);
            xMax = Mathf.Max(points[i].x, xMax);
            
            yMin = Mathf.Min(points[i].y, yMin);
            yMax = Mathf.Max(points[i].y, yMax);
        }

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static Bounds ToWorldBounds2D(this Bounds localBounds, Transform transform)
    {
        Vector3 worldCenter = transform.TransformPoint(localBounds.center);
        
        // transform the local extents' axes
        Vector2 worldExtents = localBounds.extents;
        Vector2 axisX = transform.TransformVector(worldExtents.x, 0, 0);
        Vector2 axisY = transform.TransformVector(0, worldExtents.y, 0);
 
        // sum their absolute value to get the world extents
        worldExtents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x);
        worldExtents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y);

        return new Bounds(worldCenter, 2 * worldExtents);
    }

    public static bool Intersects2D(this Bounds bounds, Bounds otherBounds)
    {
        if (bounds.min.x > otherBounds.max.x || bounds.max.x < otherBounds.min.x) return false;
        if (bounds.min.y > otherBounds.max.y || bounds.max.y < otherBounds.min.y) return false;

        return true;
    }
    
    public static float Difference(this float a, float b)
    {
        return Math.Abs(a - b);
    }

    public static float CalculateOptimalRenderScale()
    {
        int targetScreenWidth = 1920;
        int targetScreenHeight = 1080;

        if (Application.isMobilePlatform)
        {
            targetScreenWidth = 1280;
            targetScreenHeight = 720;
        }
        
        int targetNumPixels = targetScreenWidth * targetScreenHeight;
        int curNumPixels = Screen.width * Screen.height;
        
        return Mathf.Clamp01((float)targetNumPixels / curNumPixels);
    }
    
    /// <summary>
    /// Gives same results as using Preprocessor Directives, but allows everything to be compiled and analyzed by IDE.
    /// (E.g. if your build target is StandaloneOSX, this will return OSXPlayer and NOT _____Editor)
    /// </summary>
    public static RuntimePlatform GetRuntimePlatform()
    {
        #if UNITY_EDITOR
        // From https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/BuildTargetConverter.cs
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneOSX: return RuntimePlatform.OSXPlayer;
            case BuildTarget.StandaloneWindows: return RuntimePlatform.WindowsPlayer;
            case BuildTarget.iOS: return RuntimePlatform.IPhonePlayer;
            case BuildTarget.Android: return RuntimePlatform.Android;
            case BuildTarget.StandaloneWindows64: return RuntimePlatform.WindowsPlayer;
            case BuildTarget.WebGL: return RuntimePlatform.WebGLPlayer;
            case BuildTarget.WSAPlayer: return RuntimePlatform.WSAPlayerARM;
            case BuildTarget.StandaloneLinux64: return RuntimePlatform.LinuxPlayer;
            case BuildTarget.PS4: return RuntimePlatform.PS4;
            case BuildTarget.XboxOne: return RuntimePlatform.XboxOne;
            case BuildTarget.tvOS: return RuntimePlatform.tvOS;
            case BuildTarget.Switch: return RuntimePlatform.Switch;
            case BuildTarget.Lumin: return RuntimePlatform.Lumin;
            case BuildTarget.GameCoreXboxSeries: return RuntimePlatform.GameCoreXboxSeries;
            case BuildTarget.GameCoreXboxOne: return RuntimePlatform.GameCoreXboxOne;
            case BuildTarget.PS5: return RuntimePlatform.PS5;
            default: return RuntimePlatform.WindowsPlayer;  // No "None" option available, hopefully never an issue
        }
        #else
        return Application.platform;
        #endif
    }

    public static bool ParseQueryString(this string query, out OrderedDictionary parameterValueDict)
    {
        parameterValueDict = new OrderedDictionary();
        
        char[] separators = {'?', '&', '='};
        string[] parametersAndValues = query.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        if (parametersAndValues.Length % 2 != 0)
        {
            Debug.LogWarning("Provided URL has improperly-formatted query");
            return false;
        }
        
        for (int i = 0; i < parametersAndValues.Length - 1; i += 2)
        {
            string parameter = parametersAndValues[i];
            string value = parametersAndValues[i + 1];

            parameterValueDict.Add(parameter, value);
        }

        return true;
    }

    public static string ToQueryString(this OrderedDictionary parameterValueDict)
    {
        if (parameterValueDict.Count < 1) return "";
        
        StringBuilder queryStringBuilder = new StringBuilder();

        queryStringBuilder.Append('?');

        foreach (DictionaryEntry dictionaryEntry in parameterValueDict)
        {
            queryStringBuilder.Append(dictionaryEntry.Key);
            queryStringBuilder.Append('=');
            queryStringBuilder.Append(dictionaryEntry.Value);
            queryStringBuilder.Append('&');
        }

        queryStringBuilder.Length -= 1;  // Removes the last '&' tacked on the end

        return queryStringBuilder.ToString();
    }
    
    // Max int representable by 4 characters = 475253
    public static string IntToLetters(int value, int minNumLetters = 4)
    {
        StringBuilder letters = new StringBuilder();
        
        while (value >= 0)
        {
            letters.Insert(0, (char) ('A' + value % 26));
            value /= 26;
            value -= 1;
        }

        while (letters.Length < minNumLetters)
        {
            letters.Insert(0, 'A');  // Fill the front of the resulting string with A's by default
        }
        
        return letters.ToString();
    }
    
    public static Toggle GetSelectedToggle(this ToggleGroup toggleGroup)
    {
        return toggleGroup.ActiveToggles().FirstOrDefault();
    }

    // public static LocalizationText GetText(this Toggle toggle)
    // {
    //     return toggle.GetComponentInChildren<LocalizationText>();
    // }

    public static string GetValueString(this TMP_Dropdown dropdown)
    {
        if (dropdown.options.Count <= 0)
        {
            return "";
        }
        
        return dropdown.options[Math.Max(0, dropdown.value)].text;
    }

    public static float Round(this float value)
    {
        return Mathf.Round(value);
    }
    
    public static int RoundDown(this float value)
    {
        return (int) value;
    }
    
    public static int RoundUp(this float value)
    {
        return (int)Math.Ceiling(value);
    }
    
    public static int RoundToInt(this float value)
    {
        return Mathf.RoundToInt(value);
    }

    public static float RoundToMultiple(this float value, float multipleOf)
    {
        return (value/multipleOf).Round() * multipleOf;
    }

    public static float RoundToDecimal(this float value, int decimalNum)
    {
        float multValue = 1 * Mathf.Pow(10, decimalNum);
        value = Mathf.Round(value * multValue) / multValue;
        return value;
    }
    
    public static float RoundDownTo(this float value, float multipleOf) 
    {
        return (value/multipleOf).RoundDown() * multipleOf;
    }
    
    public static float RoundUpTo(this float value, float multipleOf) 
    {
        return (value/multipleOf).RoundUp() * multipleOf;
    }

    public static void AddOrIncreaseValue<T>(this Dictionary<T, float> dictionary, T key, float value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] += value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
    
    public static void AddOrReplaceValue<T>(this Dictionary<T, float> dictionary, T key, float value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
    
    public static Tweener DoSimplePunchScale(this RectTransform transform)
    {
        transform.DOKill(true);
        return transform.DOPunchScale(new Vector3 (D.vv.tweenBounceScale, D.vv.tweenBounceScale, 0), D.vv.tweenBounceDuration);
    }
    
    public static void DoSimplePunchScale(this Transform transform)
    {
        transform.DOKill(true);
        transform.DOPunchScale(new Vector3 (D.vv.tweenBounceScale, D.vv.tweenBounceScale, 0), D.vv.tweenBounceDuration);
    }
    
    /// <remarks>
    /// Ported from SNKRX source code Trigger:set_every_multiplier()
    /// </remarks>
    public static void SetEveryMultiplier(this Tween tween, float multiplier)
    {
        tween.timeScale = 1.0f / multiplier;
    }
    
    public static AudioClip PlayOneShot(this AudioSource audioSource, OneShotSfx oneShotSFX)
    {
        if (oneShotSFX.audioClips.Length == 0)
        {
            // Debug.LogWarning("Zero AudioClips assigned for OneShotSFX: " + oneShotSFX);
            return D.au.nullAudioClip;
        }

        float volume = oneShotSFX.volumeDistribution.x;
        if (!Mathf.Approximately(oneShotSFX.volumeDistribution.x, oneShotSFX.volumeDistribution.y))
        {
            float curveValueY = D.au.sfxVolumeDistributionCurve.Evaluate(Random.Range(0f, 1f));

            volume = Mathf.Lerp(oneShotSFX.volumeDistribution.x, oneShotSFX.volumeDistribution.y, curveValueY);
        }

        volume = Mathf.Clamp01(volume);

        AudioClip chosenAudioClip = oneShotSFX.audioClips[Random.Range(0, oneShotSFX.audioClips.Length)];
        audioSource.PlayOneShot(chosenAudioClip, volume);

        return chosenAudioClip;  // Return the AudioClip that is actually played
    }
    
    public static void PlayPersistentAudioType(this AudioSource audioSource, PersistentAudioType singleTrackType)
    {
        if (!D.au.persistentAudioData.ContainsKey(singleTrackType))
        {
            audioSource.Stop();
            Debug.LogWarning("SingleTrackAudio not set for SingleTrackType." + singleTrackType);
            return;
        }
        
        PlaySingleTrack(audioSource, D.au.persistentAudioData[singleTrackType]);
    }
    
    public static void PlaySingleTrack(this AudioSource audioSource, SingleTrackAudio singleTrack)
    {
        audioSource.maxDistance = D.au.audioRangeValues[singleTrack.maxRange];
        audioSource.volume = singleTrack.trackVolume;
        audioSource.clip = singleTrack.audioTrack;
        audioSource.timeSamples = 0;  // Manually reset to play from start of track
        audioSource.loop = true;
        audioSource.Play();
    }
    
    #region Scriptable Objects
    
    /// <summary>
    /// Creates and returns a clone of any given scriptable object.
    /// From: https://forum.unity.com/threads/create-copy-of-scriptableobject-during-runtime.355933/
    /// </summary>
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogWarning($"Called Clone() on null scriptable object, returning null value.");  // Returning default {typeof(T)} object.");
            return null;    //(T)ScriptableObject.CreateInstance(typeof(T));
        }
 
        T instance = UnityEngine.Object.Instantiate(scriptableObject);
        //instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }

    #if UNITY_EDITOR
    /// <summary>
    /// To be used in conjunction with AssetDatabase.LoadAssetAtPath<YourObj>(path);
    /// </summary>
    /// <param name="folder"></param>
    /// The folder you wish to search
    /// <param name="searchQuery"></param>
    /// To filter by type: "t:ItemSO". "t:ItemSO boost" would get all items that have the string "boost" in them
    /// <returns></returns>
    public static List<string> GetAssetPathsFromFolder(DefaultAsset folder, string searchQuery)
    {
        string[] itemDir = {AssetDatabase.GetAssetPath(folder)};
        string[] itemGUIDs = AssetDatabase.FindAssets(searchQuery, itemDir);

        List<string> paths = new();
        foreach (string GUID in itemGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUID);
            paths.Add(path);
        }

        return paths;
    }
    #endif
    
    #endregion
}

[Serializable]
public class MinMax
{
    public float min = -1;
    public float max = 1;

    public MinMax(float min = 0, float max = 0)
    {
        this.min = min;
        this.max = max;
    }

    public float Middle
    {
        get { return (min + max) / 2; }
    }

    public bool IsWithin(float num)
    {
        return (num >= min && num <= max);
    }

    public float Lerp(float percent)
    {
        return Mathf.Lerp(min, max, percent);
    }

    public float NegLerp(float percent)
    {
        return Mathf.Lerp(max, min, percent);
    }
}

//Starts infinite (so not expired)
[Serializable]
public class TimerOld
{
    public float _cooldown = 0;
    float _timeAtReset = -1;
    bool _playNoMatterWhat = false;

    public delegate void OnExpireDelegate();
    OnExpireDelegate _expireFunction = null;
    bool _isInfinite = true;
    bool _useUnscaledTime = false;


    float GetTime()
    {
        return _useUnscaledTime ? Time.unscaledTime : Time.time;
    }
    void Update()
    {
        if (IsExpired() && _expireFunction != null)
        {
            OnExpireDelegate lastDelegate = _expireFunction;
            ClearOnExpire();
            lastDelegate.Invoke();
        }
    }

    public TimerOld(float cooldown, bool startExpired = false)
    {
        this._cooldown = cooldown;
        if (startExpired)
        {
            SetExpired();
        }
    }

    public TimerOld(bool startExpired)
    {
        if (startExpired)
        {
            SetExpired();
        }
    }
    
    public TimerOld()
    {
    }

    public void Reset(float resetTimer)
    {
        if (resetTimer < 0)
        {
            SetExpired();
        }
        else
        {
            _cooldown = resetTimer;
            Reset();
        }
    }

    public void Reset()
    {
        if(_playNoMatterWhat) PlayOnExpire();
        ClearOnExpire();
        _isInfinite = false;
        _timeAtReset = GetTime();
    }

    public bool IsExpired()
    {
        if (_isInfinite) return false;
        return _timeAtReset + _cooldown <= GetTime();
    }

    public bool IsRunning()
    {
        return !IsExpired() && !IsInfinite();
    }

    public float GetIncreasingPercent()
    {
        if (_isInfinite)
        {
            return 0;
        }

        //Increases over time
        return GetTime().PercentClamped(_timeAtReset, GetTimeOfExpire());
    }

    public float GetDecreasingPercent()
    {
        if (_isInfinite)
        {
            return 1;
        }

        //Decreases over time
        return GetTime().Percent(GetTimeOfExpire(), _timeAtReset);
    }

    private float GetTimeOfExpire()
    {
        return _timeAtReset + _cooldown;
    }

    public float GetElapsedTime()
    {
        return GetTime() - _timeAtReset;
    }
    
    public float GetRemainingTime()
    {
        return GetTimeOfExpire() - GetTime();
    }
    
    public void SetRemainingTime(float time)
    {
        _timeAtReset = time - _cooldown + GetTime();
    }

    public void OnExpire(OnExpireDelegate expireFunction, bool playNoMatterWhat = false)
    {
        _playNoMatterWhat = playNoMatterWhat;
        _expireFunction = expireFunction;
        M.dm.TimerUpdater.AddListener(Update);
    }

    public void ResetAndPlayOnExpire(OnExpireDelegate expireFunction,  bool playNoMatterWhat, float newTimer = -1)
    {
        if (newTimer == -1)
        {
            Reset();
        }
        else
        {
            Reset(newTimer);
        }
 
        OnExpire(expireFunction, playNoMatterWhat);
    }

    public void ResetAndPlayOnExpire(OnExpireDelegate expireFunction, float newTimer = -1)
    {
        if (newTimer == -1)
        {
            Reset();
        }
        else
        {
            Reset(newTimer);
        }
 
        OnExpire(expireFunction);
    }

    public void SetExpired()
    {
        _timeAtReset = (-1f * _cooldown) - 10000f;    //Just has to be low so _cooldown + _resetTime is negative
        _isInfinite = false;
    }


    public void SetInfinite()
    {
        _isInfinite = true;
        if(_playNoMatterWhat) PlayOnExpire();
        ClearOnExpire();
    }

    public bool IsInfinite()
    {
        return _isInfinite;
    }

    public void ClearOnExpire()
    {
        _expireFunction = null;
        M.dm.TimerUpdater.RemoveListener(Update);
    }

    public void SetPercent(float percent)
    {
        SetRemainingTime(0.Percent(_cooldown, percent));
    }

    public void PlayOnExpire()
    {
        _expireFunction?.Invoke();
    }
}

public class StopWatch
{
    bool _isRunning = false;
    float _startTime = 0f;
    float _timeFromPreviousStarts = 0f;
    
    bool _useUnscaledTime = false;


    float GetTime()
    {
        return _useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    public void Start()
    {
        if (_isRunning) return;
        
        _startTime = GetTime();
        _isRunning = true;
    }

    public void Stop()
    {
        if (!_isRunning) return;
        
        _timeFromPreviousStarts += (GetTime() - _startTime);
        _isRunning = false;
    }

    public void Reset()
    {
        Stop();
        _timeFromPreviousStarts = 0f;
    }

    public float Percent(float compareTime)
    {
        return Mathf.Clamp01(GetTotalTime() / compareTime);
    }

    public float GetTotalTime()
    {
        float totalTime = _timeFromPreviousStarts;
        
        if (_isRunning) totalTime += (GetTime() - _startTime);

        return totalTime;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }
}

[Serializable]
public class StackTimerTracker
{
    int _count = 0;
    float _firstTimedUse = 0;
    int _maxCount = -1;
    [SerializeField] float _resetCooldown;
    [SerializeField] bool _resetAllOnCooldown = false;

    public StackTimerTracker()
    {
        
    }
    
    public StackTimerTracker(float resetCooldown, bool resetAllOnCooldown = false)
    {
        _resetCooldown = resetCooldown;
        _resetAllOnCooldown = resetAllOnCooldown;
    }
    
    public void Used(float resetCooldown, int maxCount = -1)
    {
        _resetCooldown = resetCooldown;
        _maxCount = maxCount;
        Used();
    }

    public void Used()
    {
        CheckReset();
        if (_count == 0)
        {
            _firstTimedUse = Time.unscaledTime;
        }
        _count += 1;
        if (_maxCount >= 0)
        {
            _count = Mathf.Min(_count, _maxCount);
        }
    }

    public int GetCount()
    {
        CheckReset();
        return _count;
    }

    void CheckReset()
    {
        if (_resetAllOnCooldown)
        {
            float timePassed = Time.unscaledTime - _firstTimedUse;
            if (timePassed > _resetCooldown)
            {
                _count = 0;
            }
        }
        else
        {
            float timePassed = Time.unscaledTime - _firstTimedUse;
            while (timePassed > _resetCooldown && _count > 0)
            {
                timePassed -= _resetCooldown;
                _firstTimedUse += _resetCooldown; //adding to the timer, undoing the count
                _count--;
            }
        }
    }

    public void Clear()
    {
        _count = 0;
    }
}

[Serializable]
public class RegenBar
{
    [SerializeField] float _max = 100;
    [SerializeField] float _regenRate = 10;
    [SerializeField] float _usageRate = -5;
    [SerializeField] float _requiredToUse = 5;
    float _current = 0;

    public void UseTick(float deltaTime)
    {
        AddToValue(-1 * _usageRate * deltaTime);
    }

    public void SetVariables(float max, float regenRate, float usageRate, float requiredToUse)
    {
        _max = max;
        _regenRate = regenRate;
        _usageRate = usageRate;
        _requiredToUse = requiredToUse;
    }

    public void RegenTick(float deltaTime)
    {
        AddToValue(_regenRate * deltaTime);
    }

    public void Tick(bool inUse, float deltaTime)
    {
        if (inUse)
        {
            UseTick(deltaTime);
        }
        else
        {
            RegenTick(deltaTime);
        }
    }

    public float GetPercent()
    {
        if (_max.IsApprox(0))
        {
            return 0;
        }

        return _current / _max;
    }

    public float Current()
    {
        return _current;
    }

    public void Subtract(float amount)
    {
        AddToValue(-amount);
    }

    public void AddToValue(float add)
    {
        _current += add;
        if (_current <= 0)
        {
            _current = 0;
        }
        else if (_current >= _max)
        {
            _current = _max;
        }
    }

    public void MaxOut()
    {
        _current = _max;
    }

    public void SetPercent(float percent)
    {
        _current = _max * percent;
    }

    public bool IsEmpty()
    {
        return _current <= 0;
    }

    public bool CanUse()
    {
        return _current >= _requiredToUse;
    }
}

[Serializable]
public class UnityEventBoolean : UnityEvent<bool>
{
}

[Serializable]
public class UnityEventInt : UnityEvent<int>
{
}

[Serializable]
public class UnityEventFloat : UnityEvent<float>
{
}

/// <summary>
/// Attribute to select a single layer.
/// </summary>
public class LayerAttribute : PropertyAttribute
{
}