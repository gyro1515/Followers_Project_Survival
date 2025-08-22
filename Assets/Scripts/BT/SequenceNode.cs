using System.Collections.Generic;
using UnityEngine;

public sealed class SequenceNode : INode
{
    private List<INode> _children;
    public SequenceNode(List<INode> children)
    {
        _children = children;
    }
    
    public INode.ENodeState Evaluate()
    {
        if (_children == null || _children.Count == 0)
            return INode.ENodeState.Failure;

        foreach (var child in _children)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    continue;
                case INode.ENodeState.Failure:
                    return INode.ENodeState.Failure;
            }
        }
        return INode.ENodeState.Success;
    }
}