using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ShootSystem : MonoBehaviour
{
    [Header("Ammo Values")]
    public int totalAmmo;
    public int currentAmmo;
    
    [Header("References")]
    public Transform gunEnd;
    public GameObject cameraFollower;

    [Header("VFX Components")]
    public GameObject muzzleFlashParticles;
    public GameObject impactParticles;

    public bool isReloading;

    private bool canShoot = true;

    public Weapon currentWeapon;
    GamepadHandler gamepad;

    VirtualCameraController cameraVC;

    public void Awake()
    {
        searchWeapon();
    }

    public void Start()
    {
        cameraVC = Camera.main.GetComponent<VirtualCameraController>();
        
        gamepad = GetComponent<GamepadHandler>();

        muzzleFlashParticles = GameObject.FindWithTag("MuzzleFlash");
        muzzleFlashParticles.SetActive(false);
        
    }
    public void HandleShot()
    {
        if (!isReloading && totalAmmo > 0)
        {
            //Handle Reload
            if (currentAmmo == 0 || gamepad.input.characterControls.Reload.triggered && currentAmmo != currentWeapon.magazineCapacity)
            {
                StartCoroutine(Reload());
            }
        }

        if (!isReloading && currentAmmo > 0)
        {
            //Shoot Handle
            if (gamepad.rightTrigger == true && canShoot)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(currentWeapon.reloadSpeed);

        if (totalAmmo < currentWeapon.magazineCapacity)
        {
            currentAmmo += totalAmmo;
            totalAmmo -= totalAmmo;
        }
        else
        {
            int xAmmo = currentWeapon.magazineCapacity - currentAmmo;
            currentAmmo += xAmmo;
            totalAmmo -= xAmmo;
        }
        isReloading = false;
    }
    public IEnumerator Shoot()
    {
        canShoot = false;
        
        currentAmmo -= 1; //Ammo substract
        
        raycastShoot();

        cameraVC.ApplyVibrationToCurrentCamera(0.5f);
        
        gamepad.vibrateController(true);

        muzzleFlashParticles.SetActive(true);

        yield return new WaitForSeconds(1 / currentWeapon.FireRate);

        gamepad.vibrateController(false);

        cameraVC.ApplyVibrationToCurrentCamera(0);

        muzzleFlashParticles.SetActive(false);

        canShoot = true;
    }
    public void raycastShoot()
    {
        //Check and debug if all components are assigned
        checkComponents();

        if (gunEnd != null)
        {
            RaycastHit hit;
            Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)); //Get the center of screen and return a Vector3
            Ray ray = Camera.main.ScreenPointToRay(screenCenter); //Create a ray from screen center
            ray.origin = gunEnd.position; //Reference to the cannon`s gun

            //Raycast Shoot
            if (Physics.Raycast(screenCenter, Camera.main.transform.forward, out hit, 100))
            {
                if (hit.collider != null)
                {

                    Debug.DrawLine(ray.origin, hit.point, Color.blue,5);

                    Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
            else
            {
                Debug.DrawLine(ray.origin, Camera.main.transform.forward * 100, Color.red, 0.1f, true);
            }
        }
    }
    public void searchWeapon()
    {
        currentWeapon = GetComponentInChildren<Weapon>();
        gunEnd = currentWeapon.transform.GetChild(0);
        
        if(muzzleFlashParticles == null)
        {
            muzzleFlashParticles = gunEnd.transform.GetChild(0).gameObject;
        }

        if (currentWeapon == null)
        {
            Debug.LogError("Weapon is missing");
        }
    }
    public void checkComponents()
    {
        if (gunEnd == null)
        {
            Debug.Log("Missing transform");
        }
    }
}
