using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using IconData = DiscreteAttributeIcon.IconData;

public class DiscreteAttributeHud : MonoBehaviour
{
    [SerializeField, RequiredIn(PrefabKind.InstanceInScene)]
    private AttributeComponent _targetAttribute;
    [SerializeField, RequiredIn(PrefabKind.InstanceInScene)]
    private AttributeComponent _targetMaxAttribute;
    
    [SerializeField] private CollectionBoundPrefabs<IconData, DiscreteAttributeIcon> _icons;
    
    private void OnEnable()
    {
        _targetAttribute.OnChangeCurValue += OnChangeTargetAttribute;
        _targetMaxAttribute.OnChangeCurValue += OnChangeTargetMaxAttribute;
        
        RefreshIcons();
    }
    
    private void OnChangeTargetAttribute(AttributeComponent.ChangeValueParams changeValueParams)
    {
        RefreshIcons();
    }
    
    private void OnChangeTargetMaxAttribute(AttributeComponent.ChangeValueParams changeValueParams)
    {
        RefreshIcons();
    }
    
    private void RefreshIcons()
    {
        List<IconData> iconFillValues = new();
        
        float curValue = _targetAttribute.curValue;
        float maxValue = _targetMaxAttribute.curValue;
        
        int numIcons = Mathf.CeilToInt(maxValue);
        float valueToDistribute = curValue;
        
        // Fills a list of floats to represent the fill value of each attribute icon, i.e.
        // [1, 1, 0.5, 0]
        for (int i = 0; i < numIcons; i++)
        {
            float iconFillValue = Mathf.Clamp01(valueToDistribute);
            iconFillValues.Add(new IconData(iconFillValue));
            
            valueToDistribute -= iconFillValue;
        }
        
        _icons.Bind(iconFillValues);
    }
    
    private void OnDisable()
    {
        _targetAttribute.OnChangeCurValue -= OnChangeTargetAttribute;
        _targetMaxAttribute.OnChangeCurValue -= OnChangeTargetMaxAttribute;
    }
}
