using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When all the child nodes are success, the Sequence node are success
/// It will be failure once one of childs is failure
/// </summary>
public class Sequence : BTNode
{
    protected List<BTNode> nodes = new List<BTNode>();

    public Sequence(List<BTNode> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        bool isAnyNodeRunning = false;
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyNodeRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;
                default:
                    break;
            }
        }
        _nodeState = isAnyNodeRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return _nodeState;
    }
}
