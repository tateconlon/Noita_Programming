using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeactivateWhenAttributesZero : MonoBehaviour
{
    [SerializeField, Required] private GameObject _targetGameObject;
    [SerializeField] private List<AttributeComponent> _targetAttributes;
    [SerializeField] private CheckBehavior _checkBehavior = CheckBehavior.And;
    
    private void OnEnable()
    {
        foreach (AttributeComponent targetAttribute in _targetAttributes)
        {
            targetAttribute.OnChangeCurValue += OnTargetAttributeChange;
        }
    }

    private void OnTargetAttributeChange(AttributeComponent.ChangeValueParams changeValueParams)
    {
        foreach (AttributeComponent targetAttribute in _targetAttributes)
        {
            if (targetAttribute.curValue <= 0)
            {
                if (_checkBehavior == CheckBehavior.Or)
                {
                    _targetGameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                if (_checkBehavior == CheckBehavior.And)
                {
                    return;
                }
            }
        }

        if (_checkBehavior == CheckBehavior.And)
        {
            _targetGameObject.SetActive(false);
        }
    }
    
    private void OnDisable()
    {
        foreach (AttributeComponent targetAttribute in _targetAttributes)
        {
            targetAttribute.OnChangeCurValue -= OnTargetAttributeChange;
        }
    }

    private enum CheckBehavior
    {
        And,
        Or
    }
}