using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float fullHP;
    public  float lowHP;

    public  float chaseRange;
    public  float fireRange;
    public Transform playerTransform;
   
    private Material material;
    // private Transform bestCoverSpot;
    private NavMeshAgent agent;

    private BTNode topNode;

	public float curHP;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponentInChildren<MeshRenderer>().material;
    }

    private void Start()
    {
        curHP = fullHP;
        playerTransform = GameObject.Find("AgentChan").transform;
        ConstructBehahaviourTree();
    }

    /// <summary>
    /// Initialize the Be-Trees
    /// </summary>
    private void ConstructBehahaviourTree()
    {
        HPNode healthNode = new HPNode(this, lowHP, agent);
        
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this);
        RangeNode chaseRangeNode = new RangeNode(chaseRange, playerTransform, transform);
        RangeNode fireRangeNode = new RangeNode(fireRange, playerTransform, transform);
        FireNode fireNode = new FireNode(agent, this, playerTransform);

        Sequence chaseSequence = new Sequence(new List<BTNode> { chaseRangeNode, chaseNode });
        Sequence shootSequence = new Sequence(new List<BTNode> { fireRangeNode, fireNode });
        // Selector deadSelector = new Selector(new List<BTNode> { healthNode });

        topNode = new Selector(new List<BTNode> {shootSequence, chaseSequence, healthNode});
    }

    /// <summary>
    /// Recover the HP when the game on
    /// Set the red color to represent stop state
    /// </summary>
    private void Update()
    {
        if(curHP <= 0)
        {
            SetColor(Color.black);
            agent.isStopped = true;
            return;
        }

        topNode.Evaluate();
        if(topNode.nodeState == NodeState.FAILURE)
        {
            SetColor(Color.grey);
            agent.isStopped = true;
        }
    }

    /// <summary>
    /// Down the HP when take damage
    /// </summary>
    public void TakeDamage(float damage)
    {
        curHP -= damage;
    }

    /// <summary>
    /// Change the color to visualize the state changing 
    /// </summary>
    public void SetColor(Color color)
    {
        material.color = color;
    }

}
