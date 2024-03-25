using System;
using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using TMPro;
using UnityEngine;

public class TutorialTextDissapear : MonoBehaviour
{
    [SerializeField]
    ScopedVariable<int> levelWhenDissapear;

    public void Check(Int32 currLevel)
    {
        if (currLevel >= levelWhenDissapear)
        {
            TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();

            foreach (TextMeshPro text in texts)
            {
                text.enabled = false;
            }
        }
        else
        {
            TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();

            foreach (TextMeshPro text in texts)
            {
                text.enabled = true;
            }
        }
    }
}
