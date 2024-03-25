using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[DefaultExecutionOrder(int.MaxValue)]  // Run this cleanup after everything else has been destroyed
public class CleanupGlobalVariables : MonoBehaviour
{
    [SerializeField] private List<GlobalVariableBase> _globalVariables;
    [SerializeField] private List<NodeGraphInstance> _nodeGraphInstances;

    private void OnDestroy()
    {
        foreach (GlobalVariableBase globalVariable in _globalVariables)
        {
            globalVariable.ResetValue();
        }

        foreach (NodeGraphInstance nodeGraphInstance in _nodeGraphInstances)
        {
            nodeGraphInstance.ResetValue();
        }
    }
}
