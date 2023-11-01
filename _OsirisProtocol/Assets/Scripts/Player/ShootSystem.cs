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
    
    GamepadHandler gamePad;
    Animator animator;

    VirtualCameraController cameraVC;

    public void Awake()
    {
        searchWeapon();
    }

    public void Start()
    {
        cameraVC = Camera.main.GetComponent<VirtualCameraController>();
        gamePad = GetComponent<GamepadHandler>();

        muzzleFlashParticles = GameObject.FindWithTag("MuzzleFlash");
        muzzleFlashParticles.SetActive(false);

        animator = GetComponent<Animator>();
        
    }
    public void HandleShot()
    {
        if (!isReloading && totalAmmo > 0)
        {
            //Handle Reload
            if (currentAmmo == 0 || gamePad.input.characterControls.Reload.triggered && currentAmmo != currentWeapon.magazineCapacity)
            {
                StartCoroutine(Reload());
            }
        }

        if (!isReloading && currentAmmo > 0)
        {
            //Shoot Handle
            if (gamePad.rightTrigger == true && canShoot)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    public void HandleReload()
    {
        if (!isReloading && totalAmmo > 0)
        {
            //Handle Reload
            if (currentAmmo == 0 || gamePad.input.characterControls.Reload.triggered && currentAmmo != currentWeapon.magazineCapacity)
            {
                StartCoroutine(Reload());
            }
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        animator.SetLayerWeight(2, 1);
        animator.SetTrigger("Reload");

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
        
        animator.SetLayerWeight(2, 0);
        isReloading = false;
    }
    public IEnumerator Shoot()
    {
        canShoot = false;
        
        currentAmmo -= 1; //Ammo substract
        
        raycastShoot();

        cameraVC.ApplyVibrationToCurrentCamera(0.5f);
        
        gamePad.vibrateController(true);

        muzzleFlashParticles.SetActive(true);

        yield return new WaitForSeconds(1 / currentWeapon.FireRate);

        gamePad.vibrateController(false);

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
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2); //Get the center of screen and return a Vector3
            Ray ray = Camera.main.ScreenPointToRay(screenCenter); //Create a ray from screen center
            Vector3 origin = gunEnd.position; //Reference to the cannon`s gun

            //Raycast Shoot
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider != null)
                {
                    HealthSystem healthSystem = hit.collider.GetComponentInParent<HealthSystem>(); //Check if the collider is an enemy with health system  
                    if (healthSystem != null)
                    {
                        string impactZone = hit.collider.name; //Get impact zone of the ragdoll colliders
                        healthSystem.ApplyDamage(currentWeapon.damage, impactZone); //Apply damage to health of the collider and return the impact zone

                        Rigidbody rb = hit.collider.GetComponent<Rigidbody>(); //Get rigidbody component od the collider
                        Vector3 forceDirection = -hit.normal; //Get the direction to apply force
                        float force = 100; //Amoutn of force to apply
                        rb.AddForce(forceDirection * force, ForceMode.Impulse); //Apply a impulse force in the hit point of the collider

                        Debug.DrawLine(origin, hit.point, Color.green, 5); //Debug function
                    }
                    else
                    {
                        Debug.DrawLine(origin, hit.point, Color.blue, 5); //Debug function
                    }

                    Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal)); //Create impact particles on the hit point
                }
            }
            else
            {
                Debug.DrawLine(ray.origin, Camera.main.transform.forward * 100, Color.red, 0.1f, true); //Debug function
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
