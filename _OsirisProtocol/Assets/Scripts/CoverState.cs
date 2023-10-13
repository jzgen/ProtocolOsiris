using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverState : BaseStates
{
    bool canMove = false;
    float speed = 4;
    float borderOffset = 0.5f;

    //Interpolation variables
    Vector3 lastPosition;
    Vector3 fixedPosition;
    Quaternion lastRotation;
    Quaternion fixedRotation;

    //Interpolate time
    float duration = 0.4f;
    float timeElapsed = 0;

    // Joysticks Input stores
    Vector2 leftJoystick;
    Vector2 rightJoystick;

    //Camera rotation stores
    public float rotationX;
    public float rotationY;

    CinemachineVirtualCamera virtualCamera;
    AimSystem aimSystem;

    public override void EnterState(playerCtrl player)
    {
        aimSystem = player.GetComponent<AimSystem>();
        aimSystem.standWeightConstraint = 0;

        //Store the last raycast info and player transforms to avoid variations on the interpolation
        fixedPosition = player.fixedPosition;
        lastPosition = player.lastPosition;

        fixedRotation = player.fixedRotation;
        lastRotation = player.lastRotation;

        //Initialize the time in 0 to avoid conflicts
        timeElapsed = 0;

        //Change the StandVC to CoverVC 
        virtualCamera = GameObject.Find("CoverVC").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Priority = 3;

    }
    public override void UpdateState(playerCtrl player)
    {
        leftJoystick = player.leftJoystick;
        rightJoystick = player.rightJoystick;

        if (aimSystem.isAiming)
        {
            speed = 1;
        }
        else
        {
            speed = 4;
        }

        if (!canMove)
        {
            leftJoystick = Vector2.zero;
            rightJoystick = Vector2.zero;
            Interpolate(player);
        }
        else
        {
            handleRotation(player);
            MoveAlongCover(player);
        }
    }
    public override void ExitState(playerCtrl player)
    {
        rotationY = 0;

        //Return the current VC  to StandVC
        virtualCamera.Priority = 0;

        //Clean the variable that allow the playerMove
        canMove = false;
    }

    //AllowPlayer Move Along the Cover
    void MoveAlongCover(playerCtrl player)
    {
        bool borderReached = false;

        //Change border direction
        Vector3 rayOrigin = player.borderRayOrigin.transform.position;
        if (leftJoystick.x > 0.1f)
        {
            rayOrigin += player.borderRayOrigin.transform.TransformDirection(Vector3.right) * borderOffset;
        }
        else if (leftJoystick.x < -0.1f)
        {
            rayOrigin += player.borderRayOrigin.transform.TransformDirection(Vector3.left) * borderOffset;
        }

        //Detect the border 
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, player.transform.forward, out hit, 1))
        {
            //Add the surface oreintation void
        }
        else
        {
            borderReached = true;
        }

        //Allow or block the walk along the cover
        if (borderReached)
        {
            player.animator.SetFloat("AxisX", 0);
        }
        else
        {
            Vector3 move = Vector3.right * leftJoystick.x * Time.deltaTime * speed;
            player.transform.Translate(move, Space.Self);
            player.animator.SetFloat("AxisX", leftJoystick.x);
        }
    }

    //Handle the camera cover rotation
    void handleRotation(playerCtrl player)
    {
        rotationY += rightJoystick.x * player.rotationSensitivity * Time.deltaTime;
        player.gimbalY.localRotation = Quaternion.Euler(0, rotationY, 0);

        if (aimSystem.isAiming)
        {
            //Horizontal Aim Rotation
            aimSystem.coverWeightConstraint = 1;
            rotationY = Mathf.Clamp(rotationY, -75, 75);

            //Horizontal Aim Rotation
            rotationX -= rightJoystick.y * player.rotationSensitivity * Time.deltaTime;
            player.gimbalX.localRotation = Quaternion.Euler(rotationX, 0, 0);
            rotationX = Mathf.Clamp(rotationX, -55, 10);

            aimSystem.coverAimController.localRotation = player.gimbalY.localRotation * player.gimbalX.localRotation;

        }
        else
        {
            aimSystem.coverWeightConstraint = 0;
        }
    }

    //Move the the player from last position to the initial cover position
    void Interpolate(playerCtrl player)
    {
        timeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(timeElapsed / duration);

        Vector3 positionInterpolated = Vector3.Lerp(lastPosition, fixedPosition, t);
        Quaternion rotationInterpolated = Quaternion.Slerp(lastRotation, fixedRotation, t);

        player.transform.position = positionInterpolated;
        player.transform.rotation = rotationInterpolated;

        if (t >= 1)
        {
            player.transform.position = fixedPosition;
            player.transform.rotation = fixedRotation;
            canMove = true;
        }
    }
}