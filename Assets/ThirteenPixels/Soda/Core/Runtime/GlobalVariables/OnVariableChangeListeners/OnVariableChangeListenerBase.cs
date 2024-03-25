using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;

public abstract class OnVariableChangeListenerBase<T> : MonoBehaviour
{
    [SerializeField] GlobalVariableBase<T> listenVariable;
    [SerializeField] UnityEvent<T> response;
    [SerializeField] bool invokeOnEnable = true;
    
    [DisableIf(nameof(ShouldDisableCheckbox))]
    [SerializeField] bool listenWhileDisabled = false;  // Shouldn't be able to modify at runtime

    bool ShouldDisableCheckbox => Application.isPlaying;

    void Awake()
    {
        if (listenWhileDisabled)
        {
            if (invokeOnEnable)
            {
                listenVariable.onChange.AddResponseAndInvoke(OnValueChanged);
            }
            else
            {
                listenVariable.onChange.AddResponse(OnValueChanged);
            }
        }
    }

    void OnEnable()
    {
        if (!listenWhileDisabled)
        {
            if (invokeOnEnable)
            {
                listenVariable.onChange.AddResponseAndInvoke(OnValueChanged);
            }
            else
            {
                listenVariable.onChange.AddResponse(OnValueChanged);
            }
        }
    }
    
    void OnDisable()
    {
        if (!listenWhileDisabled)
        {
            listenVariable.onChange.RemoveResponse(OnValueChanged);
        }
    }

    void OnDestroy()
    {
        if (listenWhileDisabled)
        {
            listenVariable.onChange.RemoveResponse(OnValueChanged);
        }
    }

    void OnValueChanged(T newValue)
    {
        response.Invoke(newValue);
    }
}