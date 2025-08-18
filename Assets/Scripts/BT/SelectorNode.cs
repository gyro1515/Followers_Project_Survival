using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SelectorNode : INode
{
    List<INode> _children;

    public SelectorNode(List<INode> children)
    {
        _children = children;
    }

    INode.ENodeState INode.Evaluate()
    {
        if (_children == null)
            return INode.ENodeState.Failure;

        foreach (var child in _children)
        {
            switch (child.Evaluate()) { 
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    return INode.ENodeState.Success;
            }
        }

        return INode.ENodeState.Failure;
    }

}
