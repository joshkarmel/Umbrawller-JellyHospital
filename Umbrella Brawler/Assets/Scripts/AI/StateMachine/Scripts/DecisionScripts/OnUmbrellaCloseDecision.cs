using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu (menuName = "AI/StateMachine/Decisions/OnUmbrellaClose")]
public class OnUmbrellaCloseDecision : Decision {
//--------------------------------------------------------------------------METHODS:
	///<Summary>
    /// Abstract method from Decision.cs Scriptable Object
	///</Summary>
	public override bool Decide(StateController stateController){
		DLog("AI Deciding: On umbrella close");
		bool umbrellaIsOpen = stateController.PlayerControllerAI.Umbrella.IsUmbrellaOpen;
		return !umbrellaIsOpen;
	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}