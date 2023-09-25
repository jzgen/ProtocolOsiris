using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerCtrl : MonoBehaviour
{
    PlayerInput input;

    public Transform cameraPivot;
    public float rotationSpeed = 100f;
    public float xRotation = 0f;
    
    Vector2 leftJoystick;
    Vector2 rightJoystick;
    
    Animator animator;
    void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //Handle Rotation
        float joystickR_AxisX = rightJoystick.x * rotationSpeed * Time.deltaTime;
        float joystickR_AxisY = rightJoystick.y * rotationSpeed * Time.deltaTime;

        xRotation -= joystickR_AxisY;
        xRotation = Mathf.Clamp(xRotation, -35f, 40f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, -7f, 0f);
        transform.Rotate(Vector3.up, joystickR_AxisX);

        animator.SetFloat("ViewY", xRotation);


        //Handle movement
        if(leftJoystick != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
            animator.SetFloat("AxisY", leftJoystick.y);
            animator.SetFloat("AxisX", leftJoystick.x);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
    private void OnEnable()
    {
        input.characterControls.Enable();
    }

    private void OnDisable()
    {
        input?.characterControls.Disable();
    }
}
