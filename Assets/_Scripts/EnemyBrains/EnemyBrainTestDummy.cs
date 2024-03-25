using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(HealthV2))]
public class EnemyBrainTestDummy : MonoBehaviour
{
    [SerializeField, Required] private HealthV2 _health;
    [SerializeField, Required] private KnockbackReceiver _knockbackReceiver;
    
    private const float BaseHealth = 1000000.0f;
    private const float BaseSize = 1.0f;
    private const float BaseKnockbackMult = 1.0f;
    
    private void OnEnable()
    {
        _health.Init(BaseHealth);
        transform.localScale = BaseSize * Vector3.one;
        _knockbackReceiver.KnockbackMultiplier = BaseKnockbackMult;
        
        gameObject.AddHTag(HTags.Enemy);
    }
    
    private void Start()
    {
        GameManager.Instance.Testing.OnSetIsActive += OnActivateTesting;
        OnActivateTesting(GameManager.Instance.Testing.IsActive);
    }
    
    private void OnActivateTesting(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.Testing.OnSetIsActive -= OnActivateTesting;
    }
}
