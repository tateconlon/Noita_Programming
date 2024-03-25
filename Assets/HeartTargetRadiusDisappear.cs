using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class HeartTargetRadiusDisappear : MonoBehaviour
{
    [SerializeField] GlobalGameObject heart;
    [SerializeField] ScopedVariable<float> radiusTargetDisappear;
    [SerializeField] GameObject visualsRoot;
    
    // Update is called once per frame
    void Update()
    {
        if (heart != null)
        {
            float dist = Vector3.Magnitude(this.transform.position - heart.value.transform.position);
            if (dist < radiusTargetDisappear)
            {
                visualsRoot.SetActive(false);
            }
            else
            {
                visualsRoot.SetActive(true);
            }
        }
    }
}
