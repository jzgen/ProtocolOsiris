using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerCtrl : MonoBehaviour
{
    PlayerInput input;

    public float rotationSpeed;
    public Vector2 leftJoystick;
    public Vector2 rightJoystick;

    Animator animator;
    void Awake()
    {
        input = new PlayerInput();
        input.characterControls.Movement.performed += ctx => leftJoystick = ctx.ReadValue<Vector2>();
        input.characterControls.View.performed += ctx => rightJoystick = ctx.ReadValue<Vector2>();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Handle Rotation
        transform.Rotate(Vector3.up, rightJoystick.x * rotationSpeed * Time.deltaTime);

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
