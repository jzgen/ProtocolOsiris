using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class iAgentController : MonoBehaviour
{
	//Temporal variables
	public Transform targetPosition;
	public Transform player;
	public bool Ragdoll = false;

	//State Machine
	iBaseStates currentState;
	iPatrolState iPatrolState = new iPatrolState();
	iCombatState iCombatState = new iCombatState();

	//Components
	Animator animator;
	NavMeshAgent agent;
	HealthSystem healthSystem;

	//Walk direction
	public Vector2 directionXZ;

	void Start()
	{
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		healthSystem = GetComponent<HealthSystem>();
	}

	// Update is called once per frame
	void Update()
	{
		if (currentState != null)
		{
			currentState.UpdateState(this);
		}
	}
	public void MoveWhileAiming()
	{
		if (!healthSystem.isDeath)
		{
			//Move the agent to specified position
			agent.SetDestination(targetPosition.position);
			HandleWalk();

			//Adjust the rotation to look the player
			Vector3 lookDirection = player.position - transform.position;
			transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
		}
	}
	public void HandleWalk()
	{
		//Calculates the direction vector in local space XZ 
		Vector3 moveDirection = targetPosition.position - transform.position;
		Vector3 localDirection = transform.InverseTransformDirection(moveDirection);
		directionXZ = new Vector2(localDirection.x, localDirection.z).normalized;

		float movementThreshold = 0.1f;
		bool isMoving = localDirection.magnitude > movementThreshold;

		if (isMoving)
		{
			animator.SetBool("IsWalking", true);
			animator.SetFloat("AxisX", directionXZ.x);
			animator.SetFloat("AxisY", directionXZ.y);
		}
		else
		{
			animator.SetFloat("AxisX", 0);
			animator.SetFloat("AxisY", 0);
			animator.SetBool("IsWalking", false);
		}
	}
	public void SetState(iBaseStates state)
	{
		if (currentState != null)
		{
			currentState.ExitState(this);
		}

		currentState = state;
		currentState.EnterState(this);
	}
}