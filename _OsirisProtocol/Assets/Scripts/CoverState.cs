using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoverState : BaseStates
{
    bool isCover = false;
    float currentTime;
    public override void EnterState(playerCtrl player)
    {
        player.animator.SetBool("IsCover", true);
        currentTime = 0;
        currentTime = Time.time;
        isCover = false;
    }
    public override void UpdateState(playerCtrl player)
    {
        float waitTime = 0.5f;
        float targetTime = currentTime + waitTime;

        if (player.isCover)
        {
            player.transform.rotation = Quaternion.Euler(0, player.yTargetRotation, 0);
            player.transform.position = player.fixedPosition;
        }

        if (Time.time >= targetTime)
        {
            isCover = true;
        }

        if (player.input.characterControls.Cover.triggered && isCover)
        {
            Debug.Log("LB triggered");
            player.SetState(player.standstate);
        }

    }
    public override void ExitState(playerCtrl player)
    {
        player.animator.SetBool("IsCover", false);
        isCover = false;

    }
}
