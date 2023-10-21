using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class iAgentController : MonoBehaviour
{
    public Transform targetPosition;
    public Transform player;
    public Vector2 directionXZ;

    Animator animator;
    NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = player.position - transform.position;

        // Calcula el vector de dirección en el espacio local XZ
        Vector3 moveDirection = targetPosition.position - transform.position;
        Vector3 localDirection = transform.InverseTransformDirection(moveDirection);
        directionXZ = new Vector2(localDirection.x, localDirection.z).normalized;

        // Asigna la dirección al agente para que se mueva en el plano XZ
        agent.SetDestination(targetPosition.position);

        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        float movementThreshold = 0.1f;  // Puedes ajustar este valor según tus necesidades
        bool isMoving = localDirection.magnitude > movementThreshold;

        if (isMoving)
        {
            animator.SetBool("IsWalking", true);
            animator.SetFloat("AxisX", directionXZ.x);
            animator.SetFloat("AxisY",directionXZ.y);
        }
        else
        {
            animator.SetFloat("AxisX", 0);
            animator.SetFloat("AxisY", 0);
            animator.SetBool("IsWalking", false);
        }

    }
}
