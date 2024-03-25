using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class InventoryNodeReceiver : MonoBehaviour, IReleasedNodeReceiver
{
    [Required]
    [SerializeField] private SkillInventoryInstance _inventory;
    
    public void ReceiveReleasedNode(SkillNode releasedNode)
    {
        if (releasedNode.graph is SkillShop skillShop)
        {
            if (skillShop.TryBuySkill(releasedNode))
            {
                _inventory.BaseValue.CopyNode(releasedNode);
                releasedNode.graph.RemoveNode(releasedNode);
            }
            else
            {
                // Player doesn't have enough gold to buy skill
            }
        }
        else if (releasedNode.graph is SkillTree sourceSkillTree)  // Unequipped an equipped skill
        {
            List<SkillNode> nodesToMove = sourceSkillTree.GetNodeAndDescendants(releasedNode);
            
            foreach (SkillNode nodeToMove in nodesToMove)
            {
                _inventory.BaseValue.CopyNode(nodeToMove);
            }
            
            sourceSkillTree.RemoveNode(releasedNode);  // Will remove node and descendants
        }
        else
        {
            _inventory.BaseValue.CopyNode(releasedNode);
            releasedNode.graph.RemoveNode(releasedNode);
        }
    }
}
