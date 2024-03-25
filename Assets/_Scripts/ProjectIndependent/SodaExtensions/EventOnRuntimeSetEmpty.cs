using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;

public class EventOnRuntimeSetEmpty : MonoBehaviour
{
    [SerializeField] private RuntimeSetBase _set;
    [SerializeField] public UnityEvent onSetEmpty;

    private void OnEnable()
    {
        _set.onElementCountChange.AddResponse(OnSetCountChange);
    }

    private void OnSetCountChange(int numElements)
    {
        if (numElements == 0 && !this.IsSceneUnloading())
        {
            onSetEmpty.Invoke();
        }
    }
    
    private void OnDisable()
    {
        _set.onElementCountChange.RemoveResponse(OnSetCountChange);
    }
}
