using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/StateMachine/Actions/DivingAction")]
public class DivingAction : AIAction {

	public override void Act(StateController stateController){
		stateController.SetAIGravity(true);
		stateController.SetAICanMove(true);
		stateController.SetAIIsKinematic(false);
		Debug.Log("are we even calling diving action???????????????????/");
		Dive(stateController);
	}

	// i am gonna put in a stupid value. 
	private void Dive(StateController stateController){

		stateController.PlayerControllerAI.IsDiving(0.5f);
	}
}

