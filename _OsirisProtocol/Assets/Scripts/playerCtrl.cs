using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class playerCtrl : MonoBehaviour
{
    //Cover test
    bool coverClose;
    Vector3 hitPos;
    public Vector3 fixedPosition;
    public float magnitude;
    public Vector3 rayOffset;
    [HideInInspector] public float yTargetRotation;

    //States
    public bool isCover = false;
    public BaseStates currentStates;
    public StandState standstate = new StandState();
    public CrouchState crouchstate = new CrouchState();
    public CoverState coverstate = new CoverState();

    //Movement adjustment
    public float rotationSpeed = 100f;

    //Virtual Camera Values
    public Transform cameraPivot;
    [HideInInspector] public float xRotation = 0f;
    [HideInInspector] public float yRotation = -8f;
    [HideInInspector] public CinemachineVirtualCamera crouchVC;

    //Global Joysticks Values
    public PlayerInput input;
    [HideInInspector] public Vector2 leftJoystick;
    [HideInInspector] public Vector2 rightJoystick;

    //Palyer Animator
    [HideInInspector] public Animator animator;

    void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.Crouch.performed += OnCrouchPerformed;
    }
    void Start()
    {
        crouchVC = GameObject.Find("Crouch VC").GetComponent<CinemachineVirtualCamera>();
        animator = GetComponent<Animator>();

        //Set stand state as inital state
        SetState(standstate);
    }
    void Update()
    {
        //Execute UpdateState for the current state
        if (currentStates != null)
        {
            currentStates.UpdateState(this);
        }

        coverDetector();
    }

    //Switch between cover and stand states when crouch action is performed
    void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        if (currentStates == standstate)
        {
            SetState(crouchstate);
        }
        else
        {
            SetState(standstate);
        }
    }
    void coverDetector()
    {
        Vector3 globalNormal;
        Vector3 rayOrigin = transform.position + rayOffset;
        Vector3 rayDirection = transform.forward;
        float maxDistance = 4f;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance) && currentStates != coverstate) 
        {   
            if(hit.collider.tag == "LowCover")
            {
                coverClose = true;
                hitPos = hit.point;
                Vector3 normal = hit.normal;
                Matrix4x4 golbalRotation = hit.transform.localToWorldMatrix;
                float colliderRotation = hit.collider.transform.rotation.eulerAngles.y;
                globalNormal = golbalRotation.MultiplyVector(normal);

                //If Game Object is on this angle rotation range 45-135 angles && 225-315 need multiply negative one to get the correc orientation
                if (colliderRotation > 45 && colliderRotation <135 || colliderRotation > 225 && colliderRotation < 315)
                {
                    globalNormal.z *= -1;
                }
                
                //Based on the normal position relative to world we can know the face cover orientation and modify the rotation
                if (globalNormal.z > 0)
                {

                }
                else if (globalNormal.z < 0)
                {

                }
                
                //Calculate the  initial position where we are going to move our character in the cover state mode
                fixedPosition = (hit.point + hit.normal * magnitude) + (Vector3.up * -0.5f);
                Debug.DrawLine(rayOrigin, hit.point, Color.blue);

                //Calculate de correct rotation 
                yTargetRotation = colliderRotation + 125;

                //Wait for the trigger input to enter in the cover state
                if (input.characterControls.Cover.triggered)
                {
                    SetState(coverstate);
                }
            }
        }
        else
        {
            coverClose = false;
            globalNormal = Vector3.zero;
        }
    }
    
    //Debug tools
    public void OnDrawGizmos()
    {
        if (coverClose)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hitPos, 0.1f);
            Gizmos.DrawSphere(fixedPosition, 0.2f);
        }
    }

    public void SetState(BaseStates state)
    {
        if (currentStates != null)
        {
            currentStates.ExitState(this);
        }

        currentStates = state;
        currentStates.EnterState(this);
    }

    public void setRotationToCover()
    {
        Debug.Log("Test animation Event");
        isCover = true;
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
