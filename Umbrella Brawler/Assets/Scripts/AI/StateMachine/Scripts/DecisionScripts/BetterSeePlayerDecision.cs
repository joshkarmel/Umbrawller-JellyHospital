using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/BetterSeePlayerDecision")]
public class BetterSeePlayerDecision : Decision {

//---------------------------------------------------------------------------FIELDS:

	[SerializeField] private bool playerInSight;

//--------------------------------------------------------------------------METHODS:
	
	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On spotting enemy");
		bool playerSeen = isPlayerInSight(stateController);
		return playerSeen;
	}

   	///<Summary>
    /// Creates a bunch of rays in a cone (AI FOV) coming out from AI's Focus Area
	/// Creates a sphere for detecting nearby enemies
	/// Returns true when an enemy is within sight
	///</Summary>
	private bool DetectPlayer(StateController stateController){

		bool isHitPlayerTrue;
		RaycastHit hit;
		List<Vector3> rayDirections = new List<Vector3>();
		List<float> distancesToPlayers = new List<float>();
		float maxView = stateController.LocalAIVariables.MaxViewDistance;


		foreach(GameObject player in stateController.otherPlayersGO){
			rayDirections.Add( player.transform.position - stateController.FocusArea.transform.position);
			distancesToPlayers.Add(Vector3.Distance(stateController.FocusArea.transform.position, player.transform.position));
		}

		foreach(GameObject player in stateController.otherPlayersGO){
			for(int i = 0; i < rayDirections.Count; i++){
				Debug.DrawRay(stateController.FocusArea.transform.position, rayDirections[i], Color.yellow);
				if(Physics.Raycast (stateController.FocusArea.transform.position, rayDirections[i], out hit)){ 
					if((hit.transform.tag == "Player") && (distancesToPlayers[i] <= stateController.LocalAIVariables.minPlayerDetectDistance)){
						isHitPlayerTrue = (hit.transform.CompareTag("Player"));
						stateController.GoToTarget = hit.transform;
						return isHitPlayerTrue;
					}
				}
				else{
					return false;
				}
			}

		}

		for(int i = 0; i < rayDirections.Count; i++){
			if ((Vector3.Angle(rayDirections[i], stateController.FocusArea.transform.forward)) <= stateController.LocalAIVariables.fieldOfViewDegrees * 0.5f){
				if (Physics.Raycast(stateController.FocusArea.transform.position, rayDirections[i], out hit, maxView))
				{	isHitPlayerTrue = (hit.transform.CompareTag("Player"));
					stateController.GoToTarget = hit.transform;
					return isHitPlayerTrue;
				}
			}
		}
		return false;	 
    }

	///<Summary>
    /// Tells the decision to return true or not
	///</Summary>
	public bool isPlayerInSight(StateController stateController){
		bool seen = DetectPlayer(stateController);

		if(seen){
			playerInSight = true;
		}
		else{
			playerInSight = false;
		}
		return DetectPlayer(stateController);
	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
