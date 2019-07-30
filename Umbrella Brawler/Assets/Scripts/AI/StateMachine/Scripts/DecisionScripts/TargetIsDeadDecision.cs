using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/TargetIsDeadDecision")]
public class TargetIsDeadDecision : Decision {
	public override bool Decide(StateController stateController){
		if(stateController.GoToTarget != null){
		bool pursueTargetIsDead = stateController.GoToTarget.transform.GetComponent<PlayerInformation>().IsDead;
		if(stateController.GoToTarget != null){
			return pursueTargetIsDead;
		}else{
			return false;
		}
	}
	return false;
	}
}