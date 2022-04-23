using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Invert the state success to failure or failure to success 
/// </summary>
public class Inverter : BTNode
{
	protected BTNode node;

	public Inverter(BTNode node)
	{
		this.node = node;
	}
	public override NodeState Evaluate()
	{
		switch (node.Evaluate())
		{
			case NodeState.RUNNING:
				_nodeState = NodeState.RUNNING;
				break;
			case NodeState.SUCCESS:
				_nodeState = NodeState.FAILURE;
				break;
			case NodeState.FAILURE:
				_nodeState = NodeState.SUCCESS;
				break;
			default:
				break;
		}
		return _nodeState;
	}
}
