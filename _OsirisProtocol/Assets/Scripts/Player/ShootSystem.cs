using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    [Header("Weapon-Ammo")]
    public int totalAmmo;
    public int currentAmmo;
    public bool isReloading;
    private bool canShoot = true;
    public Weapon currentWeapon;
    LayerMask playerLayer;

    //Player Components
    GamepadHandler gamePad;
    Animator animator;
    VirtualCameraController cameraVC;

    public void Awake()
    {
        SearchWeapon();
        playerLayer = LayerMask.GetMask("Player");
    }

    public void Start()
    {
        cameraVC = Camera.main.GetComponent<VirtualCameraController>();
        gamePad = GetComponent<GamepadHandler>();

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

        RaycastShoot();

        cameraVC.ApplyVibrationToCurrentCamera();

        gamePad.vibrateController();

        yield return new WaitForSeconds(1 / currentWeapon.FireRate);

        canShoot = true;
    }
    public void RaycastShoot()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2); //Get the center of screen and return a Vector3
        Ray crosshairRay = Camera.main.ScreenPointToRay(screenCenter); //Create a ray from screen center

        //Raycast the crosshair ray
        if (Physics.Raycast(crosshairRay, out RaycastHit crosshariHit, 100, ~playerLayer))
        {
            //Vairables to create the adjusted ray
            Vector3 hitDirection = (crosshariHit.point - currentWeapon.gunEnd.transform.position).normalized;
            Vector3 rayOrigin = currentWeapon.gunEnd.transform.position;

            //Shoot the ray from gun end to hit point
            if (Physics.Raycast(rayOrigin, hitDirection, out RaycastHit adjustedHit, 100, ~playerLayer))
            {
                //Calculate the direction from Gun End to the hit point
                Vector3 direction = (adjustedHit.point - rayOrigin).normalized;
                //Shoot a physic bullet
                currentWeapon.ShootBullet(direction);

                //Debug.DrawLine(currentWeapon.gunEnd.position, adjustedHit.point, Color.blue, 5); //Debug function -----
            }

            //Debug.DrawLine(crosshairRay.origin, crosshariHit.point, Color.red, 5); //Debug function -----
        }
    }
    public void SearchWeapon()
    {
        currentWeapon = GetComponentInChildren<Weapon>();

        if (currentWeapon == null)
        {
            Debug.LogError("Weapon is missing");
        }
    }
}
