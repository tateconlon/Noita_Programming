using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVoidEvent : MonoBehaviour
{
    public string debugStr;

    public void DebugEvent()
    {
        Debug.Log(debugStr, this);
    }
}
