using Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    PlayerInput input;
    weaponStats currentWeapon;
    LineRenderer lineRenderer;

    public int totalAmmo;
    public int currentAmmo;

    public bool isRealoding;

    public Transform gunEnd;

    public CinemachineVirtualCamera standVC;
    public CinemachineVirtualCamera crouchVC;
    [HideInInspector] public CinemachineBasicMultiChannelPerlin noise;

    bool leftTrigger = false;
    private bool canShoot = true;

    public void Awake()
    {
        input = new PlayerInput();

        //Right trigger press and realese detector
        input.characterControls.Fire.performed += ctx => leftTrigger = true;
        input.characterControls.Fire.canceled += ctx => leftTrigger = false;
    }

    public void Start()
    {
        noise = standVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        searchWeapon();
        lineRenderer = GetComponent<LineRenderer>();

    }
    public void Update()
    {
        if(!isRealoding && totalAmmo > 0)
        {

            if (currentAmmo == 0 || input.characterControls.Reload.triggered && currentAmmo != currentWeapon.magazineCapacity)
            {
                StartCoroutine(Reaload());
            }
        }

        if (!isRealoding && currentAmmo > 0 )
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
        isRealoding=true;

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
        shakeCamera();
        raycastShoot();

        yield return new WaitForSeconds(1/currentWeapon.FireRate);
        shakeCameraOff();

        canShoot = true;
    }
    public void raycastShoot()
    {
        //Check and debug if all components are assigned
        checkComponents();

        if (Camera.main != null && lineRenderer != null && gunEnd != null)
        {
            RaycastHit hit;
            Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)); //Get the center of screen and return a Vector3
            Ray ray = Camera.main.ScreenPointToRay(screenCenter); //Create a ray from screen center
            ray.origin = gunEnd.position; //Reference to the cannon`s gun

            lineRenderer.SetPosition(0, ray.origin);

            //Raycast Shoot
            if (Physics.Raycast(screenCenter, Camera.main.transform.forward, out hit, 100))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, ray.origin + (Camera.main.transform.forward * 100));
            }
        }
    }
    public void shakeCamera()
    {
        noise.m_FrequencyGain = 1 / currentWeapon.FireRate;
        noise.m_AmplitudeGain = 1 / currentWeapon.RecoilAmount;
    }

    public void shakeCameraOff()
    {
        noise.m_FrequencyGain = 0;
        noise.m_AmplitudeGain = 0;
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
        if (Camera.main == null)
        {
            Debug.Log("Missing camera");
        }
        if (lineRenderer == null)
        {
            Debug.Log("Missing lineRenderer");
        }
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