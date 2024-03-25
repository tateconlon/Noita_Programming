using System;
using UnityEngine;

public class SetCursorVisibleWhileEnabled : MonoBehaviour
{
    [SerializeField] private bool _isVisibleWhileEnabled = false;
    private void OnEnable()
    {
        Cursor.visible = _isVisibleWhileEnabled;
    }

    private void Update()
    {
        //Weird race condition on startup sometimes.
        //Hack to resolve it. The race conditon failing might be very very bad (or it's just 
        //ObjectUsagesHeader failing) and causes lots of UI fuck-ups so this redundancy is nice
        if (Cursor.visible != _isVisibleWhileEnabled)
        {
            Cursor.visible = _isVisibleWhileEnabled;
        }
    }

    private void OnDisable()
    {
        Cursor.visible = !_isVisibleWhileEnabled;
    }
}