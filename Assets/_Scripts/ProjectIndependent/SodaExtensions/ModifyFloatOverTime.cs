using System.Collections;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;

public class ModifyFloatOverTime : MonoBehaviour
{
    [SerializeField] private ScopedVariable<float> _targetFloat;
    [SerializeField] private ScopedVariable<float> _minValue;
    [SerializeField] private ScopedVariable<float> _maxValue;
    [SerializeField] private ScopedVariable<float> _deltaPerSecond;
    [SerializeField] private float _tickInterval = 0f;
    [SerializeField] private bool _useUnscaledTime = false;

    [Header("Events")]
    [SerializeField] public UnityEvent OnReachMinValue;
    [SerializeField] public UnityEvent OnReachMaxValue;

    private Coroutine _coroutine;

    private void OnEnable()
    {
        _coroutine = StartCoroutine(ModifyFloat());
    }

    private IEnumerator ModifyFloat()
    {
        while (enabled)
        {
            float deltaTime;
            
            if (_tickInterval <= 0f)
            {
                yield return null;

                deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            else
            {
                yield return _useUnscaledTime
                    ? new WaitForSecondsRealtime(_tickInterval)
                    : new WaitForSeconds(_tickInterval);

                deltaTime = _tickInterval;
            }

            float delta = _deltaPerSecond.value * deltaTime;

            if (_targetFloat.value + delta <= _minValue.value)
            {
                _targetFloat.value = _minValue.value;
                OnReachMinValue.Invoke();
            }
            else if (_targetFloat.value + delta >= _maxValue.value)
            {
                _targetFloat.value = _maxValue.value;
                OnReachMaxValue.Invoke();
            }
            else
            {
                _targetFloat.value += delta;
            }
        }
    }

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
