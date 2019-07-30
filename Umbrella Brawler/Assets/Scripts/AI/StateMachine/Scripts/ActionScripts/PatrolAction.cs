using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/StateMachine/Actions/Patrol")]
public class PatrolAction : AIAction {

	public override void Act(StateController stateController){
		Patrol(stateController);
		Debug.Log("Hi this is patrol action on enable");
		
	}


	private void Patrol(StateController stateController){

		stateController.NavMeshAgent.speed = stateController.GlobalAIVariables.GroundedCharacterSpeed;
		stateController.NavMeshAgent.stoppingDistance = stateController.LocalAIVariables.StoppingDistance;

		stateController.NavMeshAgent.destination = 
		stateController.WayPointList[stateController.NextWayPoint].position;
		stateController.NavMeshAgent.isStopped = false;

	

		if(stateController.NavMeshAgent.remainingDistance <= 
		stateController.NavMeshAgent.stoppingDistance && 
		!stateController.NavMeshAgent.pathPending){
			stateController.NextWayPoint = (stateController.NextWayPoint + 1) % 
			stateController.WayPointList.Count;
		}
	}
}
