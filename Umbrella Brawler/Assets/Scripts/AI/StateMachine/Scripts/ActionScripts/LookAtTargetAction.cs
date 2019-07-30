
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// i hate this script
[RequireComponent(typeof(Weapon))]
[CreateAssetMenu(menuName = "AI/StateMachine/Actions/LookAtTargetAction")]
public class LookAtTargetAction : AIAction {

	float startTime;
	float randVertAngle;
	float randHorizAngle;
	void OnEnable(){
		randVertAngle = Random.Range(0, 0);

		randHorizAngle = Random.Range(0, 0);
		

		
		
		
		startTime = Time.time;}

	public override void Act(StateController stateController){
		
		LookAtTarget(stateController);
	
	}
	private void LookAtTarget(StateController stateController){

		stateController.MapCircuitAI.LookRotation = false;
	

		// float randVertAngle = Random.Range(-stateController.LocalAIVariables.VertAimbotOffsetAngle/2, stateController.LocalAIVariables.VertAimbotOffsetAngle/2);

		// float randHorizAngle = Random.Range(-stateController.LocalAIVariables.HorizAimbotOffsetAngle/2, stateController.LocalAIVariables.HorizAimbotOffsetAngle/2);

		

		// might not need in future
		// move to utility
		float dist =
		Vector3.Distance(stateController.PlayerControllerAI.Cam.transform.position, stateController.GoToTarget.position);

		float verticalDist = stateController.PlayerControllerAI.Cam.transform.position.y - stateController.GoToTarget.position.y;
		float vertAngleRad = Mathf.Atan(verticalDist/dist);
 		float vertAngleDeg = vertAngleRad * Mathf.Rad2Deg;


		float horizontalDist = stateController.PlayerControllerAI.Cam.transform.position.x - stateController.GoToTarget.position.x;
		float horizAngleRad = Mathf.Atan(horizontalDist/dist);
 		float horizAngleDeg = horizAngleRad * Mathf.Rad2Deg;

		 
	

	// ////////
		// stateController.PlayerControllerAI.CamCon.CamVertAngle = vertAngleDeg;
		// stateController.PlayerControllerAI.CamCon.CamHorAngle = horizAngleDeg ;


		float fracComplete = (Time.time - startTime) / stateController.LocalAIVariables.angleTransitionTime;

///////////
		stateController.PlayerControllerAI.CamCon.CamVertAngle = Mathf.SmoothStep(vertAngleDeg, vertAngleDeg + randVertAngle, fracComplete);
        

		stateController.PlayerControllerAI.CamCon.CamHorAngle = Mathf.SmoothStep(horizAngleDeg, horizAngleDeg + randHorizAngle, fracComplete);
        
		Debug.Log("horizontal angle from player: " + Mathf.RoundToInt(horizAngleDeg));
		 Debug.Log("vertical angle from player: " + Mathf.RoundToInt(vertAngleDeg));

		//  if((Mathf.RoundToInt(horizAngleDeg) <= 10) && (Mathf.RoundToInt(vertAngleDeg) <= 10)){
		// 	Debug.Log("OH MY GOD YOU COULD HAVE SHOT HIM!");
		//  }

 
	}

	private void DelayRotation(bool timeElapsed, StateController stateController){

			timeElapsed = stateController.CheckTimeElapsedInState(stateController.LocalAIVariables.UmbrellaOpenDuration);

		if(timeElapsed){
			LookAtTarget(stateController);
		}

	}

}