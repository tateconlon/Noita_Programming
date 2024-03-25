using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;

public class GlobalVariableStringFormatter : MonoBehaviour
{
    [InfoBox("This component does not listen to any GlobalVariables. " +
             "Add OnVariableChangeListeners to trigger this component's FormatString() method when any args change.",
        InfoMessageType.Warning)]
    
    public string formatString;
    [SerializeField] List<GlobalVariableBase> args = new();
    
    public UnityEvent<string> response;
    
    void OnEnable()
    {
        FormatString();
    }

    public void FormatString()
    {
        object[] argObjects = args.Select(arg => arg.valueObject).ToArray();
        
        string formattedString = string.Format(formatString, argObjects);
        
        response.Invoke(formattedString);
    }
}
