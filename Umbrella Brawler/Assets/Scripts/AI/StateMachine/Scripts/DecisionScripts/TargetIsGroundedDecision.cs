using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/TargetIsGroundedDecision")]
public class TargetIsGroundedDecision : Decision {
	public override bool Decide(StateController stateController){
		if(stateController.GoToTarget != null){
		bool pursueTargetIsGrounded = stateController.GoToTarget.transform.GetComponent<PlayerController>().IsGrounded();
		if(stateController.GoToTarget != null){
			Debug.Log("target is grounded!!!!!!!!!!!!");
			return pursueTargetIsGrounded;
		}else{
			return false;
		}
	}
	return false;
	}
}