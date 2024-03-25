using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[Serializable, CreateAssetMenu(fileName = "Skill Tree", menuName = "ScriptableObject/Skills/Skill Tree")]
public class SkillTree : NodeGraph
{
    public event Action OnModified;
    
    public ActionNode Root
    {
        get
        {
            foreach (Node node in nodes)
            {
                if (node is not ActionNode actionNode) continue;
                if (actionNode.HasParent) continue;

                return actionNode;
            }
            
            return null;
        }
    }
    
    public int GetNodeDepth(SkillNode targetNode)
    {
        SkillNode root = Root;

        if (root == null) return 0;
        
        Queue<(SkillNode skillNode, int depth)> childDepthQueue = new();
        childDepthQueue.Enqueue((root, 0));

        while (childDepthQueue.Count > 0)
        {
            (SkillNode curNode, int depth) = childDepthQueue.Dequeue();

            if (curNode == targetNode) return depth;

            foreach (SkillNode childNode in curNode.ChildrenBase)
            {
                childDepthQueue.Enqueue((childNode, depth + 1));
            }
        }
        
        // TODO: could add support for multiple roots in a tree
        // Debug.LogException(new ArgumentException($"{targetNode.name} not found in {name}"));
        return 0;
    }
    
    public List<SkillNode> GetNodeAndDescendants(SkillNode targetNode)
    {
        List<SkillNode> children = GetDescendants(targetNode);
        children.Insert(0, targetNode);
        
        return children;
    }
    
    public List<SkillNode> GetDescendants(SkillNode targetNode)
    {
        List<SkillNode> children = new();

        Queue<SkillNode> childQueue = new();
        childQueue.Enqueue(targetNode);
        
        while (childQueue.Count > 0)
        {
            SkillNode curNode = childQueue.Dequeue();
            
            foreach (SkillNode childNode in curNode.ChildrenBase)
            {
                children.Add(childNode);
                childQueue.Enqueue(childNode);
            }
        }
        
        return children;
    }
    
    public override Node CopyNode(Node original)
    {
        Node newNode = base.CopyNode(original);
        
        MaintainEmptyLeafNodes();
        
        OnModified?.Invoke();

        return newNode;
    }

    public override void RemoveNode(Node node)
    {
        foreach (SkillNode nodeToRemove in GetNodeAndDescendants(node as SkillNode))
        {
            base.RemoveNode(nodeToRemove);
        }
        
        MaintainEmptyLeafNodes();
        
        OnModified?.Invoke();
    }

    private void MaintainEmptyLeafNodes()
    {
        if (nodes.Count == 0)
        {
            AddNode<ActionNode>();  // Fallback in case the root was just removed
        }
        
        foreach (SkillNode targetNode in GetNodesWithoutMaxChildren())
        {
            Debug.Assert(!targetNode.IsEmpty);
            
            int numEmptyChildrenToAdd = targetNode.maxNumChildren - targetNode.ChildrenBase.Length;
            
            for (int i = 0; i < numEmptyChildrenToAdd; i++)
            {
                switch (targetNode)
                {
                    case ActionNode actionNode:
                        TriggerNode emptyTrigger = AddNode<TriggerNode>();
                        emptyTrigger.Parent = actionNode;
                        emptyTrigger.IsEmpty = true;
                        break;
                    case TriggerNode triggerNode:
                        ActionNode emptyAction = AddNode<ActionNode>();
                        emptyAction.Parent = triggerNode;
                        emptyAction.IsEmpty = true;
                        break;
                    default:
                        Debug.LogException(new InvalidCastException($"Invalid type of {targetNode.name}"));
                        break;
                }
            }
        }
    }
    
    private List<SkillNode> GetNodesWithoutMaxChildren()
    {
        List<SkillNode> targetNodes = new();
        
        Queue<SkillNode> childQueue = new();
        childQueue.Enqueue(Root);
        
        while (childQueue.Count > 0)
        {
            SkillNode curNode = childQueue.Dequeue();

            SkillNode[] curNodeChildren = curNode.ChildrenBase;

            if (curNodeChildren.Length < curNode.maxNumChildren)
            {
                targetNodes.Add(curNode);
            }
            
            foreach (SkillNode childNode in curNodeChildren)
            {
                childQueue.Enqueue(childNode);
            }
        }
        
        return targetNodes;
    }
}