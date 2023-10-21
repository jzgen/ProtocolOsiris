using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    public Text AmmoDisplay;
    public GameObject ReloadBar;

    Image reloadBarFill;

    ShootSystem p_ShootSystem;
    float reloadTime;
    float timeRemaining = 0;

    void Start()
    {
        p_ShootSystem = GameObject.FindWithTag("Player").GetComponent<ShootSystem>();
        reloadBarFill = ReloadBar.transform.GetChild(1).GetComponent<Image>();
    }
    void Update()
    {
        AmmoDisplay.text = p_ShootSystem.currentAmmo + "/" + p_ShootSystem.totalAmmo;

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
