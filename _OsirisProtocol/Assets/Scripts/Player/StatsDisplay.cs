using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    public Text ammoDisplay;
    public Text stateDisplay;
    public GameObject ReloadBar;

    Image reloadBarFill;

    ShootSystem p_ShootSystem;
    PlayerCharacterController p_characterController;
    
    float reloadTime;
    float timeRemaining = 0;

    void Start()
    {
        p_ShootSystem = GameObject.FindWithTag("Player").GetComponent<ShootSystem>();
        p_characterController = p_ShootSystem.gameObject.GetComponent<PlayerCharacterController>();
        reloadBarFill = ReloadBar.transform.GetChild(1).GetComponent<Image>();
    }
    void Update()
    {
        ammoDisplay.text = p_ShootSystem.currentAmmo + "/" + p_ShootSystem.totalAmmo;
        stateDisplay.text = p_characterController.currentStates.ToString();

        if (p_ShootSystem.isReloading)
        {
            reloadTime = p_ShootSystem.currentWeapon.reloadSpeed;
            ReloadBar.SetActive(true);
            displayReloadFill();

        }
        else
        {
            ReloadBar.SetActive(false);
            timeRemaining = 0;
        }
    }

    void displayReloadFill()
    {
        if(timeRemaining < reloadTime)
        {
            timeRemaining += Time.deltaTime;
            reloadBarFill.fillAmount = timeRemaining / reloadTime;
        }
    }

}
