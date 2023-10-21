using UnityEngine;
public abstract class BaseStates
{
    public abstract void EnterState(PlayerCharacterController player);
    public abstract void UpdateState(PlayerCharacterController player);
    public abstract void ExitState(PlayerCharacterController player);
}
