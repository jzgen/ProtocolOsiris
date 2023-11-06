using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Weapon Stats Values")]
	public string Name;
	public enum WeaponCategory
	{
		Rifle,
		Subfusil,
		Pistola
	}
	public WeaponCategory Category;
	public float damage;
	public float FireRate;
	public float bulletSpeed;
	public int magazineCapacity;
	public float RecoilAmount;
	public float reloadSpeed;

	//References
	[HideInInspector] public Transform gunEnd;
	[HideInInspector] public ParticleSystem muzzleFlash;

	[Header("VFX Components")]
	public GameObject bulletPrefab;

	private void Start()
	{
		muzzleFlash = GetComponentInChildren<ParticleSystem>();
		gunEnd = transform.Find("GunEnd").GetComponent<Transform>();

		muzzleFlash.Stop();
	}
	public void ShootBullet(Vector3 direction)
	{
		muzzleFlash.Emit(1);

		Bullet bulletInstiated = Instantiate(bulletPrefab, gunEnd.position, Quaternion.identity).GetComponent<Bullet>();
		if (bulletInstiated.currentWeapon == null)
		{
			bulletInstiated.currentWeapon = gameObject.GetComponent<Weapon>();
		}

		var BulletRigidBody = bulletInstiated.GetComponent<Rigidbody>();
		BulletRigidBody.velocity = bulletSpeed * direction;
	}

}
