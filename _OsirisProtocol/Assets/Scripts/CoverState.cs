using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Cinemachine;
using UnityEngine;
using Unity.Mathematics;

public class CoverState : BaseStates
{   
    Quaternion targetRotation;
    Vector3 targetPosition;
    
    float startTime;
    float moveSpeed = 0.06f;

    float rotationX = 0;

    Transform coverPivotX;

    CinemachineVirtualCamera virtualCamera;
    CinemachineBrain cameraBrain;

    public override void EnterState(playerCtrl player)
    {
        coverPivotX = player.coverCamPivot.GetChild(0);
        coverPivotX.localRotation = Quaternion.identity;
        rotationX = 0;

        player.coverCamPivot.localRotation = Quaternion.identity;

        CharacterController characterComponentt = player.GetComponent<CharacterController>();
        characterComponentt.enabled = false;

        player.animator.SetBool("IsCover", true);

        if (player.isFlipped)
        { 
            targetRotation = Quaternion.Euler(0, player.yTargetRotation, 0) * Quaternion.Euler(0, 180, 0); 
        }
        else
        { 
            targetRotation = Quaternion.Euler(0, player.yTargetRotation, 0); 
        }

        targetPosition = player.fixedPosition;
        startTime = Time.time;

        virtualCamera = GameObject.Find("Cover VC").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Priority = 1;

        cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        cameraBrain.m_DefaultBlend.m_Time = 1f;

    }
    public override void UpdateState(playerCtrl player)
    {
        if(!player.isCover)
        {
            moveToCoverPosition(player);
        }
        else
        {
            handleRotation(player);
        }
    }
    public override void ExitState(playerCtrl player)
    {
        CharacterController characterComponentt = player.GetComponent<CharacterController>();
        characterComponentt.enabled = true;

        virtualCamera.Priority = 0;
        cameraBrain.m_DefaultBlend.m_Time = 1.5f;

        player.isCover = false;
        player.animator.SetBool("IsCover", false);
    }
    public void handleRotation(playerCtrl player)
    {
        float joystickR_AxisX = player.rightJoystick.x * player.rotationSpeed * Time.deltaTime;
        float joystickR_AxisY = -player.rightJoystick.y * player.rotationSpeed * Time.deltaTime;

        rotationX += joystickR_AxisY;
        rotationX = Mathf.Clamp(rotationX, -25f, 60f);

        coverPivotX.localRotation = Quaternion.Euler(rotationX, 0, 0);
        player.coverCamPivot.Rotate(Vector3.up, joystickR_AxisX);
    }
    public void moveToCoverPosition(playerCtrl player)
    {
        float journeyLength = Vector3.Distance(player.transform.position, targetPosition);
        float distanceCovered = (Time.time - startTime) * moveSpeed;
        float fractionOfJourney = distanceCovered / journeyLength;

        // Interpolar la posición
        player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, fractionOfJourney);

        // Interpolar la rotación
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, fractionOfJourney);
    }
}
