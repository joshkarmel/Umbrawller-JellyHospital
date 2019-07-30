using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/TargetNearCrosshairDecision")]
public class TargetNearCrosshairDecision : Decision {

//---------------------------------------------------------------------------FIELDS:

	// i hate this script too
	[SerializeField] private bool playerInSight;
	float startTime;
	float randVertAngle;
	float randHorizAngle;
	void OnEnable(){
		randVertAngle = Random.Range(0, 0);
		randHorizAngle = Random.Range(0, 0);
		startTime = Time.time;
		}

//--------------------------------------------------------------------------METHODS:
	
	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On target is near crosshair");
		bool targetNear = targetNearCrosshair(stateController);
		return targetNear;
	}

   	///<Summary>
	/// Horrible
	///</Summary>
	private bool targetNearCrosshair(StateController stateController){


		// this hould be a shared AI util script
		stateController.MapCircuitAI.LookRotation = false;
	
		float dist =
		Vector3.Distance(stateController.PlayerControllerAI.Cam.transform.position, stateController.GoToTarget.position);
		Debug.DrawLine(stateController.PlayerControllerAI.Cam.transform.position, stateController.GoToTarget.position, Color.white);

		float verticalDist = stateController.PlayerControllerAI.Cam.transform.position.y - stateController.GoToTarget.position.y;
		Debug.DrawLine(stateController.PlayerControllerAI.Cam.transform.position, stateController.PlayerControllerAI.Cam.transform.position + Vector3.up, Color.magenta);
		float vertAngleRad = Mathf.Atan(verticalDist/dist);
 		float vertAngleDeg = vertAngleRad * Mathf.Rad2Deg;


		float horizontalDist = stateController.PlayerControllerAI.Cam.transform.position.x - stateController.GoToTarget.position.x;
		Debug.DrawLine(stateController.PlayerControllerAI.Cam.transform.position, stateController.PlayerControllerAI.Cam.transform.position + Vector3.right, Color.cyan);
		float horizAngleRad = Mathf.Atan(horizontalDist/dist);
 		float horizAngleDeg = horizAngleRad * Mathf.Rad2Deg;


		// // Either this or the bottom
		// stateController.PlayerControllerAI.CamCon.CamVertAngle = vertAngleDeg;
		// stateController.PlayerControllerAI.CamCon.CamHorAngle = horizAngleDeg ;


		float fracComplete = (Time.time - startTime) / stateController.LocalAIVariables.angleTransitionTime;

		stateController.PlayerControllerAI.CamCon.CamVertAngle = Mathf.SmoothStep(vertAngleDeg, vertAngleDeg + randVertAngle, fracComplete);
		stateController.PlayerControllerAI.CamCon.CamHorAngle = Mathf.SmoothStep(horizAngleDeg, horizAngleDeg + randHorizAngle, fracComplete);
        
		// Debug.Log("horizontal angle from player: " + Mathf.RoundToInt(horizAngleDeg));
		// Debug.Log("vertical angle from player: " + Mathf.RoundToInt(vertAngleDeg));

        Vector3 targetDir = stateController.GoToTarget.position - stateController.PlayerControllerAI.Cam.transform.position;
        float angle = Vector3.Angle(targetDir, stateController.PlayerControllerAI.Cam.transform.position);
	//	Debug.Log("thsi is the angle between cam vec and focus vec: " + Mathf.RoundToInt(angle));
        if (angle < 10.0f && angle > -10.0f){
			stateController.WeaponAI.AICheckShootWrapper(false, true);
         //   Debug.Log("OH MY GOD YOU COULD HAVE SHOT HIM HERE ALSO!!!!!");
		}


		/////////////////////////////////////////////
		Vector3 camToP = stateController.PlayerControllerAI.Cam.transform.position - stateController.GoToTarget.position;
		Vector3 FToP = stateController.FocusArea.transform.position - stateController.GoToTarget.position;
		float dot = Vector3.Dot(camToP, FToP);
		dot = dot / (camToP.magnitude * FToP.magnitude);
		float acos = Mathf.Acos(dot);
		float Dangle = acos * 180 / Mathf.PI;
	//	Debug.Log("DANGLE: " + Mathf.RoundToInt(Dangle));
		if(Dangle <= 15){
			stateController.WeaponAI.AICheckShootWrapper(false, true);
		}


		 if((Mathf.RoundToInt(horizAngleDeg) <= 5 ) && (Mathf.RoundToInt(vertAngleDeg) <= 5 )){
//Debug.Log("OH MY GOD YOU COULD HAVE SHOT HIM!");
			stateController.WeaponAI.AICheckShootWrapper(false, true);
 
			return true;
		 }
		 else{
			return false;
		 }

	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
