using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/IsMovingUpwardsDecision")]
public class IsMovingUpwardDecision : Decision {

	public override bool Decide(StateController stateController){
		bool isMovingUpwards = DetectUpwardForceAboveThreshold(stateController);
		return isMovingUpwards;
	}

	private bool DetectUpwardForceAboveThreshold(StateController stateController){
		if((stateController.PlayerControllerAI.RigidBodyValue.y >= stateController.LocalAIVariables.UpwardForceThreshold) /* && !stateController.PlayerControllerAI.Umbrella.IsUmbrellaOpen */){
			Debug.Log("ACE IS MOVIN ON UP!");
			return true;
		}
		else{
			return false;
		}
	}


}
