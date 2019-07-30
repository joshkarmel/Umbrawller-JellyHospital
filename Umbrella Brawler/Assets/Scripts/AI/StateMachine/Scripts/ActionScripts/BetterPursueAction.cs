using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/StateMachine/Actions/BetterPursue")]
public class BetterPursueAction : AIAction {

	//private bool variablesSet = false;

	// NOTE: you're gonna have to use mapcircuitai
	public override void Act(StateController stateController){
		stateController.SetAIGravity(false);
		stateController.SetAICanMove(false);
		stateController.SetAIIsKinematic(true);
		//bool setOnce = stateController.setVariablesOnce;
		//if(!setOnce){
			SetCircuitVariables(stateController);
			//setOnce = true;
		//}
		Pursue(stateController);
	}

	private void SetCircuitVariables(StateController stateController){
		Debug.Log("we set the pursue variables");
		MapCircuitAI MCAI = stateController.MapCircuitAI;
		LocalAIVariables LAV = stateController.LocalAIVariables;
		stateController.MapCircuitAI.SetIsChasing(true);
		MCAI.MaxVelocity = LAV.PursueMaxVelocity;
		MCAI.MinVelocity = LAV.PursueMinVelocity;
		MCAI.Acceleration = LAV.PursueAcceleration;
		MCAI.Inertia = LAV.PursueInertia;
		MCAI.RotaionDamping = LAV.PursueRotaionDamping;
		MCAI.WaypointStoppingDistance = LAV.PursueWaypointStoppingDistance;
		MCAI.StopDelay = LAV.PursueStopDelay;
		MCAI.LookRotation = LAV.PursueLookRotation;
		MCAI.StopAtWaypoints = LAV.PursueStopAtWaypoints;	
	}

	private void Pursue(StateController stateController){

		Debug.Log("we are in the pursue action");
		// stateController.MapCircuitAI.stoppingCollider.radius = stateController.LocalAIVariables.PursueWaypointStoppingDistance;
		
		// needs to be called to update goto transofrm
		//stateController.MapCircuitAI.KillAIIfDisobeyLawsOfPhysics();
		/////////// this below might be the problem
		stateController.MapCircuitAI.SetIsChasing(true);
		stateController.MapCircuitAI.addVector3ToPlayerTransform(stateController.GoToTarget.position);
       
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




