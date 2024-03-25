using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AttributeComponent))]
public abstract class AttributeToComponentProperty<T> : MonoBehaviour where T : Component
{
    [SerializeField, Required] protected AttributeComponent _targetAttribute;
    [SerializeField, Required] protected T _targetComponent;
    
    protected abstract bool autoFindTargetAttribute { get; }
    
    protected virtual void OnEnable()
    {
        _targetAttribute.OnChangeCurValue += OnChangeCurValue;
        OnChangeCurValue(new AttributeComponent.ChangeValueParams(_targetAttribute));
    }
    
    protected abstract void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams);

    protected virtual void OnDisable()
    {
        _targetAttribute.OnChangeCurValue -= OnChangeCurValue;
    }
    
    private void Reset()
    {
        _targetAttribute = GetComponent<AttributeComponent>();
        
        if (autoFindTargetAttribute)
        {
            _targetComponent = GetComponentInParent<T>();
        }
    }
}
