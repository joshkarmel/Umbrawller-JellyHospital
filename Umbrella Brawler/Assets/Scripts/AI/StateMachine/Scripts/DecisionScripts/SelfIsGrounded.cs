using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/SelfIsGroundedDecision")]
public class SelfIsGroundedDecision : Decision {
	public override bool Decide(StateController stateController){
		if(stateController.PlayerControllerAI != null){
		bool selfIsGrounded = stateController.PlayerControllerAI.IsGrounded();
		if(stateController.PlayerControllerAI != null){
			Debug.Log("target is grounded!!!!!!!!!!!!");
			return selfIsGrounded;
		}else{
			return false;
		}
	}
	return false;
	}
}