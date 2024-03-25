using MoreMountains.Tools;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

[RequireComponent(typeof(MMProgressBar))]
public class UpdateBarFromGlobalVariables : MonoBehaviour
{
    [SerializeField] private MMProgressBar _bar;
    [SerializeField] private ScopedVariable<float> _value;
    [SerializeField] private bool _valueIsNormalized = false;
    [SerializeField, HideIf(nameof(_valueIsNormalized))]
    private ScopedVariable<float> _minValue;
    [SerializeField, HideIf(nameof(_valueIsNormalized))]
    private ScopedVariable<float> _maxValue;

    private void OnEnable()
    {
        _value.onChangeValue.AddResponse(UpdateBar);
        _minValue.onChangeValue.AddResponse(UpdateBar);
        _maxValue.onChangeValue.AddResponse(UpdateBar);
        
        if (_valueIsNormalized)
        {
            _bar.SetBar01(_value.value);
        }
        else
        {
            _bar.SetBar(_value.value, _minValue.value, _maxValue.value);
        }
    }

    private void UpdateBar()
    {
        if (_valueIsNormalized)
        {
            _bar.UpdateBar01(_value.value);
        }
        else
        {
            _bar.UpdateBar(_value.value, _minValue.value, _maxValue.value);
        }
    }

    private void OnDisable()
    {
        _value.onChangeValue.RemoveResponse(UpdateBar);
        _minValue.onChangeValue.RemoveResponse(UpdateBar);
        _maxValue.onChangeValue.RemoveResponse(UpdateBar);
    }

    private void Reset()
    {
        _bar = GetComponent<MMProgressBar>();
    }
}
