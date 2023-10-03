using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class statsDisplay : MonoBehaviour
{
    public Text displayState;
    public GameObject player;

    playerCtrl playerState;

    // Start is called before the first frame update
    void Start()
    {
        playerState = player.GetComponent<playerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState != null)
            displayState.text = "Current state: " + playerState.currentStates.ToString();

    }
}
