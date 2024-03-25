using System.Collections.Generic;
using UnityEngine;

public class HealthHud : MonoBehaviour
{
    [SerializeField] private CollectionBoundPrefabs<HealthIcon.IconData, HealthIcon> _icons;
    
    private void Start()
    {
        PlayerControllerV2.instance._health.OnHpChanged += OnHpChanged;
        RefreshIcons(PlayerControllerV2.instance._health);
    }
    
    private void OnHpChanged(HealthV2.HpChangeParams hpChangeParams)
    {
        RefreshIcons(hpChangeParams.Victim);
    }
    
    private void RefreshIcons(HealthV2 healthV2)
    {
        List<HealthIcon.IconData> iconFillValues = new();
        
        int numIcons = Mathf.CeilToInt(healthV2.MaxHp);
        float valueToDistribute = healthV2.Hp;
        
        // Fills a list of floats to represent the fill value of each attribute icon, i.e.
        // [1, 1, 0.5, 0]
        for (int i = 0; i < numIcons; i++)
        {
            float iconFillValue = Mathf.Clamp01(valueToDistribute);
            iconFillValues.Add(new HealthIcon.IconData(iconFillValue));
            
            valueToDistribute -= iconFillValue;
        }
        
        _icons.Bind(iconFillValues);
    }
    
    private void OnDestroy()
    {
        PlayerControllerV2.instance._health.OnHpChanged -= OnHpChanged;
    }
}
