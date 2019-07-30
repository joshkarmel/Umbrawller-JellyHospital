using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


// Fields will be added as AI's complexity grows
// Needs to use PlayerController

[CreateAssetMenu(menuName = "AI/StateMachine/Global AI Variables")]
public class GlobalAIVariables : ScriptableObject {

//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "GlobalAIVariables";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:

		public float
    	GroundedCharacterSpeed  = 0,
		AimingSpeed			    = 0,
		JumpSpeed			    = 0,
		FallMultiplier		    = 0,
		MaxFallSpeed		    = 0,
		MaxDiveSpeed		    = 0,
		AirMovementSpeed	    = 0,
		UmbrellaGravity		    = 0,
		UmbrellaFallingSpeed    = 0,
		SpeedDecay				= 0,
		ExtraDivingSpeed		= 0,
		SpeedDecayTimer			= 0, 
		Gravity				    = 0,
		PlayerMaxHealth			= 0,
		UmbrellaMaxHealth		= 0,
		RespawnTime				= 0;

	public void GrabPlayerControllerGlobals(StateController stateController){
		
		PlayerController pcAI = stateController.PlayerControllerAI;
		PlayerInformation piAI = stateController.PlayerInformationAI;

		GroundedCharacterSpeed  = pcAI.GroundedCharacterSpeed;
		AimingSpeed			    = pcAI.AimingSpeed;	
		JumpSpeed			    = pcAI.JumpSpeed;	
		FallMultiplier		    = pcAI.FallMultiplier;
		MaxFallSpeed		    = pcAI.MaxFallSpeed;	
		MaxDiveSpeed		    = pcAI.MaxDiveSpeed;	
		AirMovementSpeed	    = pcAI.AirMovementSpeed;	
		UmbrellaGravity		    = pcAI.UmbrellaGravity;	
		UmbrellaFallingSpeed    = pcAI.UmbrellaFallingSpeed;	
		SpeedDecay				= pcAI.Gravity;
		ExtraDivingSpeed		= pcAI.ExtraDivingSpeed;	
		SpeedDecayTimer			= pcAI.SpeedDecay;
		Gravity				    = pcAI.SpeedDecayTimer;	
		PlayerMaxHealth			= piAI.PlayerMaxHealth;	
		UmbrellaMaxHealth		= pcAI.UmbrellaMaxHealth;	
		RespawnTime				= pcAI.RespawnTime;	
	}

//--------------------------------------------------------------------------HELPERS:

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
