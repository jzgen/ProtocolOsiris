using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.Rendering.HableCurve;

public class playerCtrl : MonoBehaviour
{
    AimSystem aimSystem;

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
    public Vector3 rayOriginOffset;
    bool coverClose;
    private float xDirection;
    private GameObject hitReferenceObject;
    Vector3 hitPos; //Temporal, for DrawGizmos
    [HideInInspector] public bool isFlipped;
    [HideInInspector] public float lengthCollider;
    [HideInInspector] public float positionRelativeToCollider;
    [HideInInspector] public Vector3 fixedPosition;
    [HideInInspector] public Vector3 lastPosition;
    [HideInInspector] public Quaternion fixedRotation;
    [HideInInspector] public Quaternion lastRotation;

    //States
    public BaseStates currentStates;
    public BaseStates lastSate;
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
        aimSystem = GetComponent<AimSystem>();
        hitReferenceObject = new GameObject("Hit_obj");

        gimbalY = aimSystem.coverCameraFollower;
        gimbalX = aimSystem.coverCameraFollower.GetChild(0);

        //Set stand state as inital state
        SetState(standstate);
    }
    void Update()
    {
        CoverSystem(); //Dont move!!!

        //Change between cover and stand system
        if (input.characterControls.Cover.triggered && currentStates != coverstate && coverClose)
        {
            aimSystem.isCover = true;

            //Store the target rotation and position to move behind the cover
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            
            //Restore the pivot camera rotation before enter to Cover Sate
            gimbalY.localRotation = Quaternion.identity;
            gimbalX.localRotation = Quaternion.identity;

            //Activate the cover Transition Animation with the aniamtion event SwitchSates()
            animator.SetBool("IsCover", true);
            animator.applyRootMotion = false; //Desactivate to avoid problems with retargetting behind th cover
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
    //Animation Event - CoverLo_Idle2CoverL, CoverLo_Idle2Cover, CoverLo_CoverL2Idle, CoverLo_CoverR2Idle
    void SwitchSates()
    {
        if(currentStates !=coverstate) //Switch to Cover
        {
            SetState(coverstate);
        }
        else
        {
            SetState(standstate); //Switch To Stand
        }
    }

    void CoverSystem()
    {
        Vector3 rayOrigin = transform.position + rayOriginOffset;
        Vector3 rayDirection = transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance) && hit.collider.tag == "LowCover")
        {
            //Temporal, for DrawGizmos
            hitPos = hit.point;
            coverClose = true;

            //Get the direction between the cover and the player and seth anmator to right or left animation
            GetDirectionPlayerToCover(hit);

            //Determiante if the cover is flipped and store the rotation to be align to the cover
            GetFaceOrientation(hit);

            //Get the position to move player behind the cover
            fixedPosition = (hit.point + hit.normal * distanceToCover) + (Vector3.up * -0.5f);

            //Get the current position relative to the cover
            GetPositionRelativeToCover(hit);

            Debug.DrawLine(rayOrigin, hit.point, Color.blue);
        }
        else
        {
            xDirection = 0f;
            hitReferenceObject.SetActive(false);
            coverClose = false;
            Debug.DrawLine(rayOrigin, rayOrigin + transform.forward * rayDistance, Color.red);
        }
    }

    public void GetPositionRelativeToCover(RaycastHit hit)
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
    public void GetDirectionPlayerToCover(RaycastHit hit)
    {
        //Position a empty object in the hit point
        hitReferenceObject.SetActive(true);
        hitReferenceObject.transform.position = hit.point;
        hitReferenceObject.transform.rotation = Quaternion.LookRotation(hit.normal);

        //Get the player position relative to the empty object local space
        Vector3 playerPosition = transform.position;
        Vector3 positionRelativeToOrigin = hitReferenceObject.transform.InverseTransformPoint(playerPosition);

        //Store the player realtive position along the Axis X and set it to aniamtor
        xDirection = positionRelativeToOrigin.x;
        animator.SetFloat("playerDirection", xDirection);
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
