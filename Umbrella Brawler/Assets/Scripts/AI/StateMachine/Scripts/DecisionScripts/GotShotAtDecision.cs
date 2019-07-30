using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


[CreateAssetMenu(menuName = "AI/StateMachine/Decisions/GotShotAtDecision")]
public class GotShotAtDecision : Decision {

//--------------------------------------------------------------------------METHODS:

	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On shot at");
		bool shotAt = detectShot(stateController);
		return shotAt;
	}

	///<Summary>
    /// Uses a SphereCast to detect if there is a bottom boundary beneath player
	///</Summary>
	private bool detectShot(StateController stateController){

			Collider[] hitColliders = Physics.OverlapSphere(stateController.transform.position, 10f); // make into local ai var
	
		for (int i = 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].CompareTag("projectile")){
				Debug.Log("GOT HIT BY BOOLET");
				Vector3 normvec = Vector3.Normalize(stateController.transform.position - hitColliders[i].transform.position);
				Debug.Log("this is the unit verctor: " + normvec);
				stateController.LastShotFromNormVector = normvec;
				// this is supposed to be the other way round				
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
