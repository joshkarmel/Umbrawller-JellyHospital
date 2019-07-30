using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/StateMachine/Actions/CircuitPatrol")]
public class CircuitPatrolAction : AIAction {

	

	public override void Act(StateController stateController){

	//	bool setOnce = stateController.setVariablesOnce;
//		if(!setOnce){
			SetCircuitVariables(stateController);
		//	setOnce = true;
	//	}
		stateController.SetAIGravity(false);
		stateController.SetAICanMove(false);
		stateController.SetAIIsKinematic(true);
		Patrol(stateController);
		Debug.Log("hi we're performing a circuit patrol action");
	}

	// remember do this only once
	private void SetCircuitVariables(StateController stateController){
		MapCircuitAI MCAI = stateController.MapCircuitAI;
		LocalAIVariables LAV = stateController.LocalAIVariables;
		MCAI.SetIsChasing(false); // stop chasing player if prev state is chasing
		MCAI.MaxVelocity = LAV.MaxVelocity;
		MCAI.MinVelocity = LAV.MinVelocity;
		MCAI.Acceleration = LAV.Acceleration;
		MCAI.Inertia = LAV.Inertia;
		MCAI.RotaionDamping = LAV.RotaionDamping;
		MCAI.WaypointStoppingDistance = LAV.WaypointStoppingDistance;
		MCAI.StopDelay = LAV.StopDelay;
		MCAI.LookRotation = LAV.LookRotation;
		MCAI.StopAtWaypoints = LAV.StopAtWaypoints;	
	}


	private void Patrol(StateController stateController){

//		stateController.MapCircuitAI.KillAIIfDisobeyLawsOfPhysics();
		//stateController.MapCircuitAI.SetIsChasing(false);
		stateController.MapCircuitAI.stoppingCollider.radius = stateController.LocalAIVariables.WaypointStoppingDistance;
       
        if (stateController.MapCircuitAI.accelerating){
            stateController.MapCircuitAI.Accelerate();
        }
        if (!stateController.MapCircuitAI.accelerating){
            if(stateController.MapCircuitAI.StopAtWaypoints){
                stateController.StartCoroutine(stateController.MapCircuitAI.Decelerate());
            }else{
                stateController.MapCircuitAI.Accelerate();
            }
        } 
	}
}
