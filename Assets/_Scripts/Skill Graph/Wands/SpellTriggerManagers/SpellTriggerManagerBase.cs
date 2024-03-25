using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SpellTriggerManagerBase<T> : MonoBehaviour where T : SpellTriggerManagerBase<T>
{
    [SerializeField, Required] protected AddTriggerSpellDefinition _targetTrigger;
    
    private static T _instance;
    
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogException(new UnityException($"There should only be one active {GetType().Name} at a time"), this);
            Destroy(this);
            return;
        }
        
        _instance = this as T;
    }
}
