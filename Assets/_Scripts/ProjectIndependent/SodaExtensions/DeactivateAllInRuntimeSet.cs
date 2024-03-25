using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

public class DeactivateAllInRuntimeSet : MonoBehaviour
{
    [SerializeField, Required] private RuntimeSetGameObject _runtimeSetGameObject;
    
    [Button]
    public void DeactivateAll()
    {
        _runtimeSetGameObject.ForEach(go => go.SetActive(false));
    }
}
