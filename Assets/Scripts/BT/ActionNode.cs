using System;
using UnityEngine;

public sealed class ActionNode : INode
{
    Func<INode.ENodeState> _onUpdate = null;

    public ActionNode(Func<INode.ENodeState> onUpdate)
    {
        _onUpdate = onUpdate;
    }

    public INode.ENodeState Evaluate() => _onUpdate?.Invoke() ?? INode.ENodeState.Failure;
}