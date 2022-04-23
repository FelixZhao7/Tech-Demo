using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class of Behavior tree node
/// </summary>
[System.Serializable]
public abstract class BTNode
{
	protected NodeState _nodeState;
	public NodeState nodeState { get { return _nodeState; } }
	public abstract NodeState Evaluate();
}

/// <summary>
/// 3 states of Node
/// </summary>
public enum NodeState
{
	RUNNING,
	SUCCESS,
	FAILURE,
}
