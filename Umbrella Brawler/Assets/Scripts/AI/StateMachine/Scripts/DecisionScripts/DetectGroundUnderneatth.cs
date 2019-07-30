using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/DetectGroundUnderneathDecision")]
public class DetectGroundUnderneatth : Decision {

//--------------------------------------------------------------------------METHODS:

	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On near bottom boundary");
		bool lavaDetected = GroundInSight(stateController);
		return lavaDetected;
	}

	///<Summary>
    /// Uses a SphereCast to detect if there is a bottom boundary beneath player
	///</Summary>
	private bool GroundInSight(StateController stateController){

	
		//DetectEnemy(stateController.transform.position, 5f);

			Collider[] hitColliders = Physics.OverlapSphere(stateController.transform.position, 5f);
	
		for (int i = 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].CompareTag("Ground")){
				Debug.Log("WE FINALLY FOUND THE GROUND!!!!!!!!!!!!!");
				return true;
			}
		}

		return false;
		
		// if(!stateController.PlayerInformationAI.IsDead){
		// RaycastHit hit;

		// 	Debug.DrawRay(stateController.FocusArea.position, 
		// 	-stateController.FocusArea.up.normalized *
		// 	stateController.LocalAIVariables.GroundDistanceAvoidance, 
		// 	Color.green);

		// 	if(Physics.SphereCast(stateController.FocusArea.position, 
		// 		stateController.LocalAIVariables.FocusAreaRadius,
		// 		-stateController.FocusArea.up, 
		// 		out hit,
		// 		stateController.LocalAIVariables.GroundDistanceAvoidance) && 
		// 		hit.collider.CompareTag("Ground")){ 
		// 		DLog("AI detected ground boundary below");
		// 		return true;
		// 	} 
		// }
		// else{
		// 	return false;
		// }
		// return false;
	}
	
//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
