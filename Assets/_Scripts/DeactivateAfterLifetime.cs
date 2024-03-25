using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class DeactivateAfterLifetime : MonoBehaviour
{
    [SerializeField, Required] private GameObject _targetGameObject;
    [SerializeField, Required] private Timer _timer;
    
    private void OnEnable()
    {
        _timer.OnTimeout += OnTimeout;
        
        _timer.Restart();
    }
    
    private void OnTimeout()
    {
        _targetGameObject.SetActive(false);
    }
    
    protected virtual void OnDisable()
    {
        _timer.OnTimeout -= OnTimeout;
    }
    
    private void Reset()
    {
        _timer = GetComponent<Timer>();
    }
}
