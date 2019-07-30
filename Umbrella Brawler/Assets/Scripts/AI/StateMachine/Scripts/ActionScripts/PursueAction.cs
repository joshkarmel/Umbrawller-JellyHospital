using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/StateMachine/Actions/Pursue")]
public class PursueAction : AIAction {

	public override void Act(StateController stateController){
		Pursue(stateController);
	}

	private void Pursue(StateController stateController){
		stateController.NavMeshAgent.destination =
		stateController.GoToTarget.position;
		FaceTarget(stateController);
		stateController.NavMeshAgent.isStopped = false;
	}

	private void FaceTarget(StateController stateController){

	Transform t = stateController.NavMeshAgent.transform;

	Vector3 lookPos = stateController.GoToTarget.position - t.position;
	lookPos.y = 0;
	if (stateController.NavMeshAgent.desiredVelocity != Vector3.zero){
		Quaternion rotation = Quaternion.LookRotation(lookPos);
		t.rotation = Quaternion.Slerp(t.rotation, rotation,stateController.NavMeshAgent.angularSpeed);  
		}
	}

	private void FaceMovementDirection(StateController stateController){
		
    }
}




