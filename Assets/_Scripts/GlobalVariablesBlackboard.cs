using System;
using NodeCanvas.Framework;
using ThirteenPixels.Soda;
using UnityEngine;

[RequireComponent(typeof(GlobalBlackboard))]
public class GlobalVariablesBlackboard : MonoBehaviour
{
    [SerializeField] private GlobalBlackboard _globalBlackboard;

    [SerializeField] private BlackboardVarsToGlobalVars _blackboardVarsToGlobalVars = new();
    [Serializable] private class BlackboardVarsToGlobalVars : UnitySerializedDictionary<string, GlobalVariableBase<int>> { }


    private void Awake()
    {
        foreach ((string variableName, GlobalVariableBase<int> globalVariable) in _blackboardVarsToGlobalVars)
        {
            _globalBlackboard.GetVariable<int>(variableName).BindGetSet(
                () => globalVariable.value, go => globalVariable.value = go);
        }
    }

    private void OnDestroy()
    {
        foreach ((string variableName, GlobalVariableBase<int> _) in _blackboardVarsToGlobalVars)
        {
            _globalBlackboard.GetVariable<int>(variableName).UnBind();
        }
    }
    
    private void Reset()
    {
        _globalBlackboard = GetComponent<GlobalBlackboard>();
    }
}
