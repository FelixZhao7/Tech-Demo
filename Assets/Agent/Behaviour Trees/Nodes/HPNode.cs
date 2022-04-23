using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HPNode : BTNode
{
    private EnemyAI ai;

    private NavMeshAgent agent;
    private float threshold;

    public HPNode(EnemyAI ai, float threshold, NavMeshAgent agent)
    {
        this.ai = ai;
        this.threshold = threshold;
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if(ai.curHP <= threshold){
            return NodeState.SUCCESS;
        }else
        {
            return NodeState.FAILURE;
        }
    }
}
