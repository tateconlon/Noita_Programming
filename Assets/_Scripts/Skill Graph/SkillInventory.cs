using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[Serializable, CreateAssetMenu(fileName = "Skill Inventory", menuName = "ScriptableObject/Skills/Skill Inventory")]
public class SkillInventory : NodeGraph, IEnumerable<SkillNode>
{
    public event Action OnModified;
    
    public override Node CopyNode(Node original)
    {
        // Ignore empty nodes and never add them to the inventory
        if (original is SkillNode skillNode && skillNode.IsEmpty) return null;
        
        Node newNode = base.CopyNode(original);
        
        OnModified?.Invoke();

        return newNode;
    }

    public override void RemoveNode(Node node)
    {
        base.RemoveNode(node);
        
        OnModified?.Invoke();
    }

    public IEnumerator<SkillNode> GetEnumerator()
    {
        return nodes.ConvertAll(node => node as SkillNode).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}