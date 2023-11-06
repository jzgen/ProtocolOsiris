using UnityEngine;
public abstract class BaseStates
{
	public abstract void EnterState(PlayerCharacterController player);
	public abstract void UpdateState(PlayerCharacterController player);
	public abstract void ExitState(PlayerCharacterController player);
}
public abstract class iBaseStates
{
	public abstract void EnterState(iAgentController iAgent);
	public abstract void UpdateState(iAgentController iAgent);
	public abstract void ExitState(iAgentController iAgent);
}