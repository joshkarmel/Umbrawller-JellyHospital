using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/SeePlayerDecision")]
public class SeePlayerDecision : Decision {

	public override bool Decide(StateController stateController){
		bool playerSeen = PlayerInSight(stateController);
		return playerSeen;
	}

	private bool PlayerInSight(StateController stateController){
		RaycastHit hit;

		Debug.DrawRay(stateController.FocusArea.position, 
		stateController.FocusArea.forward.normalized *
		stateController.LocalAIVariables.FocusAreaRange, 
		Color.green);

		if(Physics.SphereCast(stateController.FocusArea.position, 
		stateController.LocalAIVariables.FocusAreaRadius,
		stateController.FocusArea.forward, 
		out hit,
		stateController.LocalAIVariables.FocusAreaRange) && 
		hit.collider.CompareTag("Player")){

			stateController.GoToTarget = hit.transform; // now this is where that thingy gets set
			// statecontroller is gonna have to grow a lot
			return true;
		} else{
			return false;
		}


	}


}
