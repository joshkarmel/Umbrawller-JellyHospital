using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/IsNearTargetDecision")]
public class IsNearTargetDecision : Decision {
// TODO: combine detect ground, top, and this as too near decisions
// when makin a scriptable object, you can set the public vars and name them accordingly.
// OR JUST CREATE AI UTILS


// public theThingYouWannaDetect;

//--------------------------------------------------------------------------METHODS:

	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On too near");
		bool shotAt = detectTooNear(stateController);
		return shotAt;
	}


	
	///<Summary>
    /// Uses a SphereCast to detect if there is a bottom boundary beneath player
	///</Summary>
	private bool detectTooNear(StateController stateController){

			Collider[] hitColliders = Physics.OverlapSphere(stateController.transform.position, 10f); // make into local ai var
	
		for (int i = 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].CompareTag("Player")){
				Debug.Log("target too near");
				Vector3 normvec = Vector3.Normalize(hitColliders[i].transform.position - stateController.transform.position);
				Debug.Log("this is the unit verctor (player): " + normvec);
				stateController.LastDirTargetTooClose = normvec;
				
				return true;
			}
		}

		return false;
	}
	
//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
