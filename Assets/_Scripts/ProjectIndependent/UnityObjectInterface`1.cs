using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class UnityObjectInterface<T> where T : class
{
    [HideLabel]
    [ValidateInput(nameof(ValidateSerializedObject))]
    // TODO: improve searching with [SearchContext]
    [SerializeField] private Object _serializedObject;
    
    private T _castValue = null;
    private bool _hasCastValue = false;
    
    public T Value
    {
        get
        {
            if (!_hasCastValue)
            {
                TryCastValue();
                
                _hasCastValue = true;
            }
            
            return _castValue;
        }
    }
    
    private void TryCastValue()
    {
        if (_serializedObject is T castValue)
        {
            _castValue = castValue;
        }
        else
        {
            Debug.LogException(new InvalidCastException($"{_serializedObject.name} does not implement interface {typeof(T).Name}"));
        }
    }

    private bool ValidateSerializedObject(Object serializedObject, ref string errorMessage)
    {
        if (!typeof(T).IsInterface)
        {
            errorMessage = $"{typeof(T).Name} must be an Interface type";
            return false;
        }
        
        if (serializedObject != null && serializedObject is not T)
        {
            errorMessage = $"{serializedObject.name} must implement interface {typeof(T).Name}";
            return false;
        }
        
        return true;
    }
}
