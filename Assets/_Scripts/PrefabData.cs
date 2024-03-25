
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PrefabData : ScriptableObject
{
    [AssetsOnly]
    public GameObject prefab;
    
    public string displayName;
}
