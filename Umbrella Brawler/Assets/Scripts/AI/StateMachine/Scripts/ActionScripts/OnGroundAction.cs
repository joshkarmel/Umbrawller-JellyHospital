
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/StateMachine/Actions/OnGround")]
public class OnGroundAction : AIAction {

	public override void Act(StateController stateController){
		stateController.SetAIGravity(true);
		stateController.SetAICanMove(true);
		stateController.SetAIIsKinematic(false);
		StayThere(stateController);
	
	}
	private void StayThere(StateController stateController){
		Debug.Log("Your decision returned true!");
		Debug.Log("Your action is being performed");
		Debug.Log("That means your're in the " + stateController.CurrentState.name + "state!");
	}
}