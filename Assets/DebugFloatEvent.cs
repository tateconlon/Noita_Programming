using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFloatEvent : MonoBehaviour
{
    public string appendStr;
    public string formatStr;

    public void DebugEvent(float eventArg)
    {
        Debug.Log(eventArg.ToString(formatStr) + $" {appendStr}", this);
    }
}
