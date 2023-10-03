using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class playerCtrl : MonoBehaviour
{
    //Cover Global Variables
    bool coverClose;
    public bool isFlipped;
    public Vector3 rayOriginOffset;
    public float rayDistance = 4f;
    public float distanceToCover;
    Vector3 hitPos; //Temporal, for DrawGizmos
    [HideInInspector] public float lengthCollider;
    [HideInInspector] public float positionRelativeToCollider;
    [HideInInspector] public Vector3 fixedPosition;
    [HideInInspector] public Quaternion fixedRotation;

    //MovementController
    public float rotationSensitivity;
    public float speed;

    //States
    public BaseStates currentStates;
    public StandState standstate = new StandState();
    public CoverState coverstate = new CoverState();

    //Global Joysticks Values
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public Vector2 leftJoystick;
    [HideInInspector] public Vector2 rightJoystick;

    //Player Animator
    [HideInInspector] public Animator animator;

    void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();

        //Set stand state as inital state
        SetState(standstate);
    }
    void Update()
    {
        CoverDetector();

        if (input.characterControls.Cover.triggered && currentStates != coverstate && coverClose)
        {
            animator.SetBool("IsCover", true);
            animator.applyRootMotion = false;
        }
        else if(input.characterControls.Cover.triggered && currentStates == coverstate)
        {
            animator.SetBool("IsCover", false);
            animator.applyRootMotion = true;
        }

        //Execute UpdateState for the current state
        if (currentStates != null)
        {
            currentStates.UpdateState(this);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entró en el trigger es el que quieres.
        if (other.CompareTag("Border"))
        {
            // Hacer algo cuando se detecta la colisión con el trigger.
            Debug.Log("CharacterController entró en el trigger.");
            Vector3 lastPosition = transform.position;
            transform.position = lastPosition;
        }
    }
    void SetCover()
    {
        if(currentStates !=coverstate)
        {
            SetState(coverstate);
        }
        else
        {
            SetState(standstate);
        }
    }
    void CoverDetector()
    {
        Vector3 rayOrigin = transform.position + rayOriginOffset;
        Vector3 rayDirection = transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance) && hit.collider.tag == "LowCover")
        {
            //Temporal, for DrawGizmos
            hitPos = hit.point;
            coverClose = true;

            //Determiante if the cover is flipped and store the rotation to be align to the cover
            GetFaceOrientation(hit);

            //Get the position to move player behind the cover
            fixedPosition = (hit.point + hit.normal * distanceToCover) + (Vector3.up * -0.5f);

            //Get the current position relative to the cover
            SetPositionRelativeToCover(hit);

            Debug.DrawLine(rayOrigin, hit.point, Color.blue);
        }
        else
        {
            coverClose = false;
            Debug.DrawLine(rayOrigin, rayOrigin + transform.forward * rayDistance, Color.red);
        }
    }
    public void SetPositionRelativeToCover(RaycastHit hit)
    {
        //Get the size along the X axis of the cover collider
        if (hit.collider is BoxCollider boxCollider)
        {
            lengthCollider = boxCollider.size.x;
        }

        //Tranform the hit world position to the current cover local space
        Vector3 localHit = hit.collider.transform.InverseTransformPoint(hitPos);
        positionRelativeToCollider = localHit.x;
    }
    private void GetFaceOrientation(RaycastHit hit)
    {
        //Tranform the local normal values to world values 
        Vector3 normal = hit.normal;
        Matrix4x4 globalRotation = hit.transform.localToWorldMatrix;
        Vector3 globalNormal = globalRotation.MultiplyVector(normal);

        //Store the hit rotation in Axis Y
        float colliderRotation = hit.collider.transform.rotation.eulerAngles.y;

        //If Game Object is on this angle rotation range 45-135 angles && 225-315 need multiply negative one to get the correc orientation
        if (colliderRotation > 45 && colliderRotation < 135 || colliderRotation > 225 && colliderRotation < 315)
        {
            globalNormal.z *= -1;
        }

        //Based on the normal position relative to world we can know the face cover orientation and modify the rotation
        if (globalNormal.z > 0) //True = back face
        {
            isFlipped = true;
            fixedRotation = Quaternion.LookRotation(-hit.transform.forward);
        }
        else if (globalNormal.z < 0) //Else = front face 
        {
            isFlipped = false;
            fixedRotation = Quaternion.LookRotation(hit.transform.forward);
        }
    }
    private void OnDrawGizmos()
    {
        if (coverClose && currentStates!= coverstate)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hitPos, 0.1f);
            Gizmos.DrawSphere(fixedPosition, 0.2f);
        }
    }
    //Make the changes between states
    public void SetState(BaseStates state)
    {
        if (currentStates != null)
        {
            currentStates.ExitState(this);
        }

        currentStates = state;
        currentStates.EnterState(this);
    }
    void OnEnable()
    {
        input.characterControls.Enable();
    }
    void OnDisable()
    {
        input.characterControls.Disable();
    }
}
