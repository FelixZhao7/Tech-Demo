using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
	public LayerMask mask;
    public float quakeRange;

	public float damage;

    private Camera mainCam;

    public NavMeshAgent playerAgent;

    public Animator animator;

    void Start()
    {
        mainCam = Camera.main;
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        UpdateQuake();

        UpdatePlayerMovement();
    }

    private void UpdateQuake()
    {
		if (Input.GetMouseButtonDown(0))
		{
			Quake();
		}
    }
    
    private void Quake()
	{
        Collider[] cols=Physics.OverlapSphere(this.transform.position, quakeRange, mask);
        if( cols.Length <= 0){
            return;
        }

        foreach (var item in cols)
        {
            Collider collider = item.gameObject.GetComponent<Collider>();
            EnemyAI ai = collider.gameObject.GetComponent<EnemyAI>();

            if(ai != null)
            {
                ai.TakeDamage(damage);
                Debug.Log("Hit !! " + damage);
                animator.Play("JUMP00");
            }
        }
	}

    private void UpdatePlayerMovement()
    {
        UpdateMoveAni();

        // Press the mouse right button to move the player 
        if(Input.GetMouseButtonDown(1))
        {
            Ray rayCast = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit castHit;

            if (Physics.Raycast(rayCast, out castHit))
            {
                MovePlayer(castHit.point);
            }
        }
    }

    private void UpdateMoveAni()
    {
        if(playerAgent == null){
            return;
        }

        //if the distance longer than 5, she needs run
        if(playerAgent.remainingDistance > 5f){
            animator.Play("RUN00_F");
        }
        //if the distance is less then 5, she can just walk
        else if(playerAgent.remainingDistance < 5f && playerAgent.remainingDistance > 0.1f)
        {
            animator.Play("WALK00_F");
        }
    }

    private void MovePlayer(Vector3 point)
    {
        playerAgent.SetDestination(point);
    }
}
