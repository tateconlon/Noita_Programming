using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FindClosestObject : MonoBehaviour
{
    public Transform originPoint;

    public UnityEvent<GameObject> FindClosestObjectEvent;

    public void FindClosestObjectAction(GameObject[] GOs)
    {
        GameObject go = transform.FindClosestObject(GOs);
        FindClosestObjectEvent.Invoke(go);
    }
    
    public void FindClosestObjectAction(Collider[] cols)
    {
        List<GameObject> GOs = new List<GameObject>();

        for (int i = 0; i < cols.Length; i++)
        {
            GOs.Add(cols[i].gameObject);
        }
        
        FindClosestObjectAction(GOs.ToArray());
    }
}
