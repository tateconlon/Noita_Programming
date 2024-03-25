using UnityEngine;

[RequireComponent(typeof(SkillIcon))]
public class EquippedSkillIcon : MonoBehaviour, IBindable<SkillTreeBuilder.Vertex>, IReleasedNodeReceiver
{
    [SerializeField] private SkillIcon _skillIcon;
    [SerializeField] private SkillInventoryInstance _inventoryWhenUnequipped;
    [SerializeField] public RectTransform _leftEdgeConnection;
    [SerializeField] public RectTransform _rightEdgeConnection;
    
    public SkillTreeBuilder.Vertex BoundTarget { get; private set; }
    
    public void Bind(SkillTreeBuilder.Vertex target)
    {
        BoundTarget = target;
        
        _skillIcon.Bind(target.node);
        ((RectTransform)transform).anchoredPosition = target.position;
    }
    
    public void ReceiveReleasedNode(SkillNode releasedNode)
    {
        if (!_skillIcon.BoundTarget.CanBeSwapped(releasedNode)) return;
        
        // If we swapped nodes with the shop, move the node placed into the shop to the inventory
        if (releasedNode.graph is SkillShop skillShop)
        {
            if (skillShop.TryBuySkill(releasedNode))
            {
                // Move purchased node to inventory
                SkillNode purchasedNode = _inventoryWhenUnequipped.BaseValue.CopyNode(releasedNode) as SkillNode;
                skillShop.RemoveNode(releasedNode);
                
                // Then swap it with this equipped node
                NodeGraphExtensions.SwapNodes(_skillIcon.BoundTarget, purchasedNode);
            }
            else
            {
                // Player doesn't have enough gold to buy skill
            }
        }
        else
        {
            NodeGraphExtensions.SwapNodes(_skillIcon.BoundTarget, releasedNode);
        }
    }
    
    private void Reset()
    {
        _skillIcon = GetComponent<SkillIcon>();
    }
}
