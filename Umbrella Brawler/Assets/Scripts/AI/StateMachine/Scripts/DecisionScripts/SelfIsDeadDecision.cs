using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/SelfIsDeadDecision")]
public class SelfIsDeadDecision : Decision {

//--------------------------------------------------------------------------METHODS:
	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On enemy death");
		bool selfIsDead = stateController.PlayerInformationAI.IsDead;
		return selfIsDead;
	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }

}