using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    //Cover Camera Pivot
    [HideInInspector] public Transform gimbalX;
    [HideInInspector] public Transform gimbalY;

    //MovementController
    [Header("Camera Speed Adjusment")]
    public float rotationSensitivity;

    //Cover System Variables
    [Header("Cover System Adjustment")]
    public float rayDistance = 0.4f;
    public float distanceToCover = 0.5f;
    public Transform borderRayOrigin;

    bool coverClose;
    [HideInInspector] public float coverDirection;

    Vector3 hitPos; //Temporal, for DrawGizmos

    [HideInInspector] public Vector3 fixedPosition;
    [HideInInspector] public Vector3 lastPosition;
    [HideInInspector] public Quaternion fixedRotation;
    [HideInInspector] public Quaternion lastRotation;

    //States
    public BaseStates lastSate;
    public BaseStates currentStates;
    public StandState standstate = new StandState();
    public CoverState coverstate = new CoverState();
    public DeathState deathstate = new DeathState();

    //Global Joysticks Values
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public Vector2 leftJoystick;
    [HideInInspector] public Vector2 rightJoystick;

    //Player Animator
    [HideInInspector] public Animator animator;

    AimSystem aimSystem;
    private GameObject hitReferenceObject;

    void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        aimSystem = GetComponent<AimSystem>();

        gimbalY = aimSystem.coverCameraFollower;
        gimbalX = aimSystem.coverCameraFollower.GetChild(0);

        hitReferenceObject = new GameObject("Hit_obj");

        //Set stand state as inital state
        SetState(standstate);
    }
    void Update()
    {
        CoverSystem(); //Dont move!!!

        //Change between cover and stand system
        if (input.characterControls.Cover.triggered && currentStates != coverstate && coverClose)
        {
            //Udate the aim state to cover Aim
            aimSystem.isCover = true;

            //Store the target rotation and position to move behind the cover
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            
            //Restore the pivot camera rotation before enter to Cover Sate
            gimbalY.localRotation = Quaternion.identity;
            gimbalX.localRotation = Quaternion.identity;

            //Activate the cover Transition Animation with the aniamtion event SwitchSates()
            animator.SetBool("IsCover", true);
            animator.applyRootMotion = false; //Desactivate to avoid problems with retargetting behind the cover
        }
        else if(input.characterControls.Cover.triggered && currentStates == coverstate)
        {
            //Activate the cover Transition Animation with the aniamtion event SwitchSates()
            animator.SetBool("IsCover", false);
            animator.applyRootMotion = true; //Activate to allow thw player movement
        }

        //Execute UpdateState for the current state
        if (currentStates != null)
        {
            currentStates.UpdateState(this);
        }
    }

    void CoverSystem()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 rayDirection = transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance) && hit.collider.tag == "LowCover")
        {
            //Temporal, for DrawGizmos
            hitPos = hit.point;
            coverClose = true;

            //Position a empty object in the hit point
            hitReferenceObject.SetActive(true);

            //Align empty object position and rotation to cover
            hitReferenceObject.transform.position = hit.point;
            hitReferenceObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            //Get the player position relative to the empty object local space
            Vector3 playerPosition = transform.position;
            Vector3 positionRelativeToOrigin = hitReferenceObject.transform.InverseTransformPoint(playerPosition);

            //Store the player relative position along the Axis X and set it to aniamtor
            coverDirection = positionRelativeToOrigin.x;
            animator.SetFloat("playerDirection", coverDirection);

            //Get the position to move player behind the cover
            fixedPosition = (hit.point + hit.normal * distanceToCover) + (Vector3.up * -0.5f);

            //Get the rottation to look forward the cover
            fixedRotation = Quaternion.LookRotation(-hit.normal);

            Debug.DrawLine(rayOrigin, hit.point, Color.blue);
        }
        else
        {
            coverClose = false;
            Debug.DrawLine(rayOrigin, rayOrigin + transform.forward * rayDistance, Color.red);
        }
    }

    //Animation Event - CoverLo_Idle2CoverL, CoverLo_Idle2Cover, CoverLo_CoverL2Idle, CoverLo_CoverR2Idle
    void SwitchSates()
    {
        if (currentStates != coverstate) //Switch to Cover
        {
            SetState(coverstate);
        }
        else
        {
            SetState(standstate); //Switch To Stand
        }
    }

    //Make the changes between states
    public void SetState(BaseStates state)
    {
        if (currentStates != null)
        {
            lastSate = currentStates;
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
    //Debug tools
    private void OnDrawGizmos()
    {
        if (coverClose && currentStates != coverstate)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hitPos, 0.1f);
            Gizmos.DrawSphere(fixedPosition, 0.2f);
        }
    }
}
