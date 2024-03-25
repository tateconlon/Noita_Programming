using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("UniBulletHell/Shot Pattern/Orbit Shot")]
public class UbhOrbitShot : UbhBaseShot
{
    [Header("===== OrbitShot Settings =====")]
    public float m_radius = 1;

    // ReSharper disable once ArrangeTypeMemberModifiers
#pragma warning disable CS0414
    int m_nowIndex;
#pragma warning restore CS0414
    float m_delayTimer;

    public override void Shot()
    {
        if (m_bulletNum <= 0)
        {
            UbhDebugLog.LogWarning(name + " Cannot shot because BulletNum is not set.", this);
            return;
        }

        if (m_shooting)
        {
            return;
        }

        m_shooting = true;
        m_nowIndex = 0;
    }

    protected virtual void Update()
    {
        if (m_shooting == false)
        {
            return;
        }

        List<Vector3> posList = DistributedCircle(transform.position, m_radius, m_bulletNum);
        foreach (Vector3 pos in posList)
        {
            UbhBullet bullet = GetBullet(pos);
            
            if (bullet == null)
            {
                FinishedShot();
                return;
            }
            
            MMAutoRotate rotScript = bullet.GetComponentInChildren<MMAutoRotate>();
            if (rotScript == null)
            {
                FinishedShot();
                return;
            }

            rotScript.OrbitCenterTransform = transform;
            rotScript.Orbiting = true;
            rotScript.OrbitRotationSpeed = m_bulletSpeed;
            rotScript.OrbitRadius = m_radius;
            rotScript.OrbitRotationAxis = shotCtrl.m_axisMove == UbhUtil.AXIS.X_AND_Y ? Vector3.forward : Vector3.up;

            ShotBullet(bullet, 0f, 0f);
            FiredShot();
        }
        
        FinishedShot();
    } 
    
    public List<Vector3> DistributedCircle(Vector3 center, float radius, int numPoints, float startDeg = 90f)
    {
        List<Vector3> res = new();
        float ang = startDeg;
        for (int i = 0; i < numPoints; i++)
        {
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = center.z;
            res.Add(pos);
            
            ang = ang + 360 / numPoints;
        }
        return res;
    }   

}
