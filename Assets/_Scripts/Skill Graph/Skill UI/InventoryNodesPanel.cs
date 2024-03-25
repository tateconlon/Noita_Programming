using Sirenix.OdinInspector;
using UnityEngine;

public class InventoryNodesPanel : MonoBehaviour
{
    [SerializeField] private SkillInventoryInstance _skillInventory;
    [SerializeField] private CollectionBoundPrefabs<SkillNode, SkillIcon> _skillIcons;

    private void OnEnable()
    {
        _skillInventory.Value.OnModified += Refresh;
        
        Refresh();
    }

    [Button]
    private void Refresh()
    {
        _skillIcons.Bind(_skillInventory.Value);
    }
    
    private void OnDisable()
    {
        _skillInventory.Value.OnModified -= Refresh;
    }
}
