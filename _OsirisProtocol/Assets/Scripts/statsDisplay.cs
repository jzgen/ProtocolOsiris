using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class statsDisplay : MonoBehaviour
{
    public Text displayState;
    public Text diplayAmmoStats;
    public GameObject player;
    public GameObject reloadBar;
    public Image barfill;

    float maxTime;
    float timeRemaining;

    ShootSystem playerWeapon;
    playerCtrl playerState;

    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = player.GetComponent<ShootSystem>();
        playerState = player.GetComponent<playerCtrl>();

        if (playerWeapon != null)
            Debug.Log("Shot system component finded");

        if (playerState != null)
            Debug.Log("Player ctrl component finded");

        maxTime = playerWeapon.currentWeapon.reloadSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState != null)
            displayState.text = "Current state: " + playerState.currentStates.ToString();

        if (playerWeapon != null)
            diplayAmmoStats.text = playerWeapon.currentAmmo + "/" + playerWeapon.totalAmmo;

        bool reloading = playerWeapon.isRealoding;

        if (reloading)
        {
            reloadBar.SetActive(true);
            DisplayReloadStatus();
        }
        else
        {
            reloadBar.SetActive(false);
            timeRemaining = 0;
        }
    }

    public void DisplayReloadStatus()
    {
        if (timeRemaining < maxTime)
        {
            timeRemaining += Time.deltaTime;
            barfill.fillAmount = timeRemaining / maxTime;
        }
    }
}
