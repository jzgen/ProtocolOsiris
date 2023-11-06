using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CoverState : BaseStates
{
	float speed = 4;
	float borderOffset = 0.5f;
	float rotationX;
	float rotationY;
	Transform borderRayOrigin;
	CoverSystem _coverSystem;
	ShootSystem _shootSystem;
	CinemachineFreeLook coverCamera;

	public override void EnterState(PlayerCharacterController player)
	{
		_coverSystem = player.GetComponent<CoverSystem>();
		borderRayOrigin = _coverSystem.borderRayOrigin;

		_shootSystem = player.GetComponent<ShootSystem>();

		//Camera Settings
		coverCamera = player.cameraVC.FreeLookCamera;
		coverCamera.Priority = 2;
		coverCamera.m_YAxis.Value = 0.35f;
		coverCamera.m_XAxis.Value = 0.35f;

		//Reset Torso Rotation
		player.torsoRotationController.localRotation = Quaternion.identity;
		rotationX = 0;
		rotationY = 0;
	}

	public override void UpdateState(PlayerCharacterController player)
	{
		if (player.gamePad.leftTrigger)
		{
			//Satnd Up aniamtion to aim
			player.animator.SetBool("IsAiming", true);

			//Allow the shoot
			_shootSystem.HandleShot();

			//Activate the Torso Aim Controller
			_coverSystem.rigWeight = 1;

			//Activate the Aim Layer
			player.animator.SetLayerWeight(1, 1);
			//Activate the Aim VC
			player.cameraVC.setCameraVC(2);

			//Desactivate the Free Look Camera
			coverCamera.Priority = 0;
			//Reset the Y axis to the Free Look Camera
			coverCamera.m_YAxis.Value = 0.35f;

			// Get the right joystick input values
			float joystickInputX = player.gamePad.rightJoystick.x;
			float joystickInputY = player.gamePad.rightJoystick.y;

			// Calculate the rotation along the axis X,Y
			rotationY += joystickInputX * player.rotationSpeed * Time.deltaTime;
			rotationX += -joystickInputY * player.rotationSpeed * Time.deltaTime;

		}
		else
		{
			//Allow the movement behind the cover
			MoveAlongCover(player);
			//Get back to cover animation
			player.animator.SetBool("IsAiming", false);
			//Allow to recharge behind the cover
			_shootSystem.HandleReload();
			//Turn off the rig aniamtion controller
			_coverSystem.rigWeight = 0;
			//Desactivate the Aim Layer in the animator
			player.animator.SetLayerWeight(1, 0);
			//Activate the Free Look Camera
			coverCamera.Priority = 2;

			//Store the camera rotation to align the aim view
			rotationY = Camera.main.transform.eulerAngles.y;
			rotationX = 0;

			//Handle the Free Look Camera movement
			HandleCamera(player);
		}

		// Rotate the torso rotation controller along the axis X,Y
		rotationX = Mathf.Clamp(rotationX, -60, 20);
		player.torsoRotationController.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
	}
	public override void ExitState(PlayerCharacterController player)
	{
		//Camera Settings
		coverCamera.m_Priority = 0;
	}
	void HandleCamera(PlayerCharacterController player)
	{
		float speedX = 120f;
		float speedY = 2f;
		coverCamera.m_XAxis.Value += player.gamePad.rightJoystick.x * speedX * Time.deltaTime;
		coverCamera.m_YAxis.Value += -player.gamePad.rightJoystick.y * speedY * Time.deltaTime;
	}

	void MoveAlongCover(PlayerCharacterController player)
	{
		bool borderReached = false;

		//Change border position based on the player direction in cover
		Vector3 rayOrigin = borderRayOrigin.transform.position;
		if (player.gamePad.leftJoystick.x > 0.1f)
		{
			rayOrigin += borderRayOrigin.transform.TransformDirection(Vector3.right) * borderOffset;
		}
		else if (player.gamePad.leftJoystick.x < -0.1f)
		{
			rayOrigin += borderRayOrigin.transform.TransformDirection(Vector3.left) * borderOffset;
		}

		//Detect the border 
		RaycastHit hit;
		if (Physics.Raycast(rayOrigin, player.transform.forward, out hit, 1))
		{
			//Add the surface orientation void
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
			Vector3 move = player.gamePad.leftJoystick.x * speed * Time.deltaTime * Vector3.right;
			player.transform.Translate(move, Space.Self);
			player.animator.SetFloat("AxisX", player.gamePad.leftJoystick.x);
		}
	}
}
