using System;
using UnityEngine;
using XNode;

public static class NodeGraphExtensions
{
    public static void SwapNodes(SkillNode nodeA, SkillNode nodeB)
    {
        Debug.Assert(CanBeSwapped(nodeA, nodeB));

        // Create a new nodeA and swap out ports
        Node newNodeA = nodeB.graph.CopyNode(nodeA);

        if (newNodeA != null)
        {
            foreach (NodePort portB in nodeB.Ports)
            {
                portB.SwapConnections(newNodeA.GetPort(portB.fieldName));
            }
        }
        
        // Create a new nodeB and swap out ports
        Node newNodeB = nodeA.graph.CopyNode(nodeB);

        if (newNodeB != null)
        {
            foreach (NodePort portA in nodeA.Ports)
            {
                portA.SwapConnections(newNodeB.GetPort(portA.fieldName));
            }
        }
        
        // Remove old nodes
        nodeA.graph.RemoveNode(nodeA);
        nodeB.graph.RemoveNode(nodeB);
    }
    
    public static bool CanBeSwapped(this SkillNode nodeA, SkillNode nodeB)
    {
        if (nodeA == null || nodeB == null) return false;   //Sometimes this fires on unpopulated icons
        
        bool nodesAreUnique = nodeA != nodeB;
        bool nodesAreSameType = nodeA.GetType() == nodeB.GetType();
        bool canMoveNodeA = !(nodeB.graph is SkillTree && nodeA.IsEmpty) && nodeA.canBeMoved;
        bool canMoveNodeB = !(nodeA.graph is SkillTree && nodeB.IsEmpty) && nodeB.canBeMoved;
        
        return nodesAreUnique && nodesAreSameType && canMoveNodeA && canMoveNodeB;
    }
}