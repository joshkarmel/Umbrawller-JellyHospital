
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/StateMachine/Actions/DebugLog")]
public class DebugLogAction : AIAction {

	public override void Act(StateController stateController){
		stateController.SetAIGravity(true);
		stateController.SetAICanMove(true);
		stateController.SetAIIsKinematic(false);
		SaySomething(stateController);
	
	}
	private void SaySomething(StateController stateController){
		Debug.Log("Your decision returned true!");
		Debug.Log("Your action is being performed");
		Debug.Log("That means your're in the " + stateController.CurrentState.name + "state!");
	}
}