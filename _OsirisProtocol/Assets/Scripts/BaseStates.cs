using UnityEngine;

public abstract class BaseStates
{
    public abstract void AwakeState(playerCtrl player);
    public abstract void EnterState(playerCtrl player);
    public abstract void UpdateState(playerCtrl player);
    public abstract void ExitState(playerCtrl player);
}
