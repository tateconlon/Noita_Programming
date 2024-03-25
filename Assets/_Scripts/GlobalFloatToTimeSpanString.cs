using System;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;

public class GlobalFloatToTimeSpanString : MonoBehaviour
{
    [Tooltip("The format string applied to the TimeSpan")]
    public string timeSpanFormatString;
    
    [Tooltip("The format string applied to the result of the TimeSpan formatting. Leave blank for only TimeSpan")]
    public string outputFormatString;

    [InfoBox("A global float that holds the time value in seconds")]
    [SerializeField] GlobalFloat globalFloat;

    [SerializeField] bool listenToVariableChanges = true;
    
    public UnityEvent<string> response;

    [Tooltip("Enable this if you want to represent a countdown")]
    [SerializeField] private bool _takeCeilingOfSeconds = true;
    
    void OnEnable()
    {
        if (listenToVariableChanges)
        {
            globalFloat.onChange.AddResponse(FormatString);
        }
        
        FormatString();
    }

    public void FormatString()
    {
        float totalSeconds = _takeCeilingOfSeconds ? Mathf.Ceil(globalFloat) : globalFloat;
        TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);

        string formattedString = timeSpan.ToString(timeSpanFormatString);

        if (!string.IsNullOrEmpty(outputFormatString))
        {
            formattedString = string.Format(outputFormatString, formattedString);
        }
        
        response.Invoke(formattedString);
    }

    void OnDisable()
    {
        if (listenToVariableChanges)
        {
            globalFloat.onChange.RemoveResponse(FormatString);
        }
    }
}
