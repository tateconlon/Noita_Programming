using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShopNodeReceiver : MonoBehaviour, IReleasedNodeReceiver
{
    [Required]
    [SerializeField] private SkillShopInstance _shop;
    [Required]
    [SerializeField] private SkillInventoryInstance _inventory;
    
    public void ReceiveReleasedNode(SkillNode releasedNode)
    {
        if (releasedNode.graph == _shop.BaseValue) return;
        
        _shop.Value.SellSkill(releasedNode);
        
        if (releasedNode.graph is SkillTree sourceSkillTree)  // Sold an equipped skill
        {
            List<SkillNode> nodesToMove = sourceSkillTree.GetDescendants(releasedNode);
            
            foreach (SkillNode nodeToMove in nodesToMove)
            {
                _inventory.Value.CopyNode(nodeToMove);
            }
            
            sourceSkillTree.RemoveNode(releasedNode);  // Will remove node and descendants
        }
        else
        {
            releasedNode.graph.RemoveNode(releasedNode);
        }
    }
}