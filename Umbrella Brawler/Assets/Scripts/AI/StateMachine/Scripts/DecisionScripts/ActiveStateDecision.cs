using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/ActiveStateDecision")]
public class ActiveStateDecision : Decision {
	public override bool Decide(StateController stateController){
		bool pursueTargetIsActive = stateController.GoToTarget.gameObject.activeSelf;
		return pursueTargetIsActive;
	}

}
