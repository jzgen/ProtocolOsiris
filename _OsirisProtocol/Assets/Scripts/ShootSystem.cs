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

    [Header("VFX Components")]
    public GameObject muzzleFlashParticles;
    public GameObject impactParticles;

    public bool isRealoding;

    bool leftTrigger = false;
    private bool canShoot = true;

    PlayerInput input;
    weaponStats currentWeapon;

    public void Awake()
    {
        input = new PlayerInput();

        //Right trigger press and realese detector
        input.characterControls.Fire.performed += ctx => leftTrigger = true;
        input.characterControls.Fire.canceled += ctx => leftTrigger = false;

        searchWeapon();
    }

    public void Start()
    {
        muzzleFlashParticles = GameObject.FindWithTag("MuzzleFlash");
        if (muzzleFlashParticles != null ) { Debug.Log("Finded"); }
        muzzleFlashParticles.SetActive(false);
        
    }
    public void Update()
    {
        if (!isRealoding && totalAmmo > 0)
        {

            if (currentAmmo == 0 || input.characterControls.Reload.triggered && currentAmmo != currentWeapon.magazineCapacity)
            {
                StartCoroutine(Reaload());
            }
        }

        if (!isRealoding && currentAmmo > 0)
        {
            //Shoot Handle
            if (leftTrigger == true && canShoot)
            {
                StartCoroutine(Shoot());
            }

        }
    }

    public IEnumerator Reaload()
    {
        isRealoding = true;

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
        isRealoding = false;
    }
    public IEnumerator Shoot()
    {
        canShoot = false;
        currentAmmo -= 1; //Ammo substract
        raycastShoot();

        muzzleFlashParticles.SetActive(true);

        yield return new WaitForSeconds(1 / currentWeapon.FireRate);

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
                    Debug.DrawLine(ray.origin, hit.point, Color.green, 0.1f);
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
        currentWeapon = GetComponentInChildren<weaponStats>();

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

    public void OnEnable()
    {
        input.characterControls.Enable();
    }

    public void OnDisable()
    {
        input?.characterControls.Disable();
    }
}
