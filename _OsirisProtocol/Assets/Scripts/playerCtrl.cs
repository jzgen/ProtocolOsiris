using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCtrl : MonoBehaviour
{
    BaseStates currentStates;
    public WalkingState walkingState = new WalkingState();

    public Transform cameraPivot;
    public float rotationSpeed = 100f;

    public bool onWalking;
    public bool onCrouch;

    void Awake()
    {
        SetState(walkingState);

        if (currentStates != null)
        {
            currentStates.AwakeState(this);
        }
    }
    void Start()
    {

    }
    void Update()
    {
        if (currentStates != null)
        {
            currentStates.UpdateState(this);
        }
    }
    public void SetState(BaseStates state)
    {
        if (currentStates != null)
        {
            currentStates.ExitState(this);
        }

        currentStates = state;
        currentStates.EnterState(this);
    }
}
