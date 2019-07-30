using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/LostTargetDecision")]
public class LostTargetDecision : Decision {

//---------------------------------------------------------------------------FIELDS:
		[SerializeField] private bool playerInSight;


//--------------------------------------------------------------------------METHODS:

	public override bool Decide(StateController stateController){
		Debug.Log("AI Deciding: On losing sight of enemy");
		bool playerSeen = isPlayerInSight(stateController);
		bool timeElapsed = false;
		if(!playerSeen){
			Debug.Log("Player out of FOV");
			return DelayReturningDetectionLoss(timeElapsed, stateController);
		}
		Debug.Log("Player still in FOV");
		return false;
		//return !playerSeen;
	}

	// NOTE: This is a horrible implementation for lost target
	// TODO: extract the better see player decision stuff similar to this
	// make it its own script and then call it here
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

		//////
		foreach(GameObject player in stateController.otherPlayersGO){
				// Detect in radius around AI
			for(int i = 0; i < rayDirections.Count; i++){ // there might be something weong here with iteration of d
				Debug.DrawRay(stateController.FocusArea.transform.position, rayDirections[i], Color.yellow);
				if(Physics.Raycast (stateController.FocusArea.transform.position, rayDirections[i], out hit)){ 
					if((hit.transform.tag == "Player") && (distancesToPlayers[i] <= stateController.LocalAIVariables.minPlayerDetectDistance)){
						isHitPlayerTrue = (hit.transform.CompareTag("Player"));
						stateController.GoToTarget = hit.transform;
						return isHitPlayerTrue;
					}
				}
				else{
					// If player is near ai, don't bother checking whether they're in FOV
					return false;
				}
			}

		}
	 ////////////// might be something wrong here too
		// Detect in FOV cone in front of AI
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

	public bool DelayReturningDetectionLoss(bool timeElapsed, StateController stateController){
		Debug.Log("delaying return of lost sighgt of player");

		timeElapsed = stateController.CheckTimeElapsedInState(stateController.LocalAIVariables.SecondsLosingSightOfPlayer);

		if(timeElapsed){
			return true;
		}

		return false;
	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }

}