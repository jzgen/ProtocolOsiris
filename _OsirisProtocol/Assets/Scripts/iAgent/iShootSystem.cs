using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iShootSystem : MonoBehaviour
{
	[Header("IA Skills")]
	public float accuracy;

	[Header("Weapon-Ammo")]
	public int currentAmmo;
	public bool canShoot = true;
	public bool isReloading = false;
	Weapon currentWeapon;

	[Header("Target References")]
	public Transform player;
	public Transform playerHitArea;
	Vector3 aimDirection;

	[Header("Aim Componets")]
	public Transform rigTorsoController;

	// AI Agent Components
	Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();

		SearchWeapon();

		currentAmmo = currentWeapon.magazineCapacity;
	}
	void Update()
	{
		aimDirection = ((playerHitArea.position - currentWeapon.gunEnd.position) / 2).normalized;
	}
	void HandleShot()
	{
		if (!isReloading && currentAmmo <= 0)
		{
			StartCoroutine(Reload());
		}

		if (!isReloading && currentAmmo > 0 && canShoot)
		{
			StartCoroutine(Shoot());
		}
	}
	IEnumerator Shoot()
	{
		canShoot = false;
		currentAmmo -= 1;
		ShootRaycast();
		yield return new WaitForSeconds(1 / currentWeapon.FireRate);
		canShoot = true;
	}
	IEnumerator Reload()
	{
		Debug.Log("Recarga iniciada");
		isReloading = true;
		animator.SetLayerWeight(2, 1);
		animator.SetTrigger("Reload");
		yield return new WaitForSeconds(currentWeapon.reloadSpeed);
		currentAmmo += currentWeapon.magazineCapacity;
		animator.SetLayerWeight(2, 0);
		isReloading = false;
		Debug.Log("Recarga finalizada");
	}
	void ShootRaycast()
	{
		Vector3 rayOrigin = currentWeapon.gunEnd.position;
		Vector3 adjustedDirection = aimDirection + (Random.insideUnitSphere / accuracy);

		if (Physics.Raycast(rayOrigin, adjustedDirection, out RaycastHit hit, 100))
		{
			Vector3 shootDirection = (hit.point - rayOrigin).normalized;
			currentWeapon.ShootBullet(shootDirection);
			Debug.DrawLine(rayOrigin, hit.point, Color.blue, 0.1f);
		}

	}

	//Using the rigging controller aim the torso to the player 
	void TorsoAim()
	{
		rigTorsoController.rotation = Quaternion.LookRotation(aimDirection);
		Ray aimRay = new Ray(rigTorsoController.position, (aimDirection));
		float distance = Vector3.Distance(transform.position, player.position);
	}
	//Search and assign the the weapon attached to this game object
	public void SearchWeapon()
	{
		currentWeapon = GetComponentInChildren<Weapon>();

		if (currentWeapon == null)
		{
			Debug.LogError("Weapon is missing");
		}
	}

}
