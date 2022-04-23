using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It will select all the child nodes, when one of them is suceess, the Selector will be success.
/// It will be failure when all of them are failure.
/// </summary>
public class Selector : BTNode
{
    protected List<BTNode> nodes = new List<BTNode>();

    public Selector(List<BTNode> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    _nodeState = NodeState.RUNNING;
                    return _nodeState;
                case NodeState.SUCCESS:
                    _nodeState = NodeState.SUCCESS;
                    return _nodeState;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        _nodeState = NodeState.FAILURE;
        return _nodeState;
    }
}
