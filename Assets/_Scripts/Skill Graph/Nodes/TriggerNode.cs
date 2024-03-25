using System;
using System.Linq;
using UnityEngine;
using XNode;

public class TriggerNode : SkillNode
{
    [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Inherited)]
    [SerializeField] private TriggerNode _parent;
    
    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
    [SerializeField] private ActionNode _children;
    
    public override bool HasParent => GetPort(nameof(_parent)).IsConnected;
    
    public override SkillNode ParentBase
    {
        get
        {
            if (!HasParent) return null;
            
            return GetPort(nameof(_parent)).Connection.node as SkillNode;
        }
    }
    
    public ActionNode Parent
    {
        get => GetPort(nameof(_parent)).Connection.node as ActionNode;
        
        // TODO: hack using _children instead of ActionNode._children
        set => GetPort(nameof(_parent)).Connect(value.GetPort(nameof(_children)));
    }
    
    public ActionNode[] Children
    {
        get
        {
            return GetPort(nameof(_children)).GetConnections().Select(port => port.node as ActionNode).ToArray();
        }
    }
    
    public override SkillNode[] ChildrenBase => Array.ConvertAll(Children, actionNode => actionNode as SkillNode);
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == nameof(_children))
        {
            return port.GetInputValues<ActionNode>();
        }

        return null;
    }
}