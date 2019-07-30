using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/TooLongInStateDecision")]
public class TooLongInStateDecision : Decision {
//--------------------------------------------------------------------------METHODS:
	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public float time; // set if you want
	public override bool Decide(StateController stateController){
		return checkTimeElapsed(stateController);
	}

	private bool checkTimeElapsed( StateController stateController){
		bool timeElapsed = stateController.CheckTimeElapsedInState(stateController.LocalAIVariables.DefaultTooLongInState);

		Debug.Log("look, this is BOOL TIME ELAPSED: " + timeElapsed);
		return timeElapsed;
	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}