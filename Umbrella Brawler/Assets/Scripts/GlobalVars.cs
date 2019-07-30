using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.SceneManagement;
public class GlobalVars : Singleton<GlobalVars>
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "GlobalVars";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public float
		GroundedCharacterSpeed  =	6, //Player movement on ground
		AimingSpeed			    		=	0.5f,
		JumpSpeed			   			 	=	6,//9.3692f,//jumpSpeed = sqrt(2*gravity*jumpHeight)
		FallMultiplier		    	= 	2.5f, //Increase falling speed 
		MaxFallSpeed		    		=	30, //Max fall speed
		MaxDiveSpeed		    		=	70, //Max diving speed
		AirMovementSpeed	    	=	10, //Air movement speed
		UmbrellaGravity		    	=	4, //Falling acceleration while umbrella
		UmbrellaFallingSpeed    =	20, //Max falling speed while umbrella is open
		SpeedDecay							=	1, //Decrease the air Speed value
		ExtraDivingSpeed				=	5, //Extra Movement speed after diving
		SpeedDecayTimer					=	5, //Time until air speed starts to decay
		Gravity				    			=	9.81f,//36;
		PlayerMaxHealth					=	100,
		UmbrellaMaxHealth				=	100,
		FallDamageSpeedThreshold=  -20,
		RespawnTime							=	4f;

    public bool TrueMute = false; //Bool to mute all sounds
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		if(instance != this)
		{
			//If not the main instance, then grab their data and absorb it
			GlobalVars.instance.GroundedCharacterSpeed  =	GroundedCharacterSpeed; 
			GlobalVars.instance.AimingSpeed			    		=	AimingSpeed;
			GlobalVars.instance.JumpSpeed			   			 	=	JumpSpeed;
			GlobalVars.instance.FallMultiplier		    	= FallMultiplier;
			GlobalVars.instance.MaxFallSpeed		    		=	MaxFallSpeed;
			GlobalVars.instance.MaxDiveSpeed		    		=	MaxDiveSpeed;
			GlobalVars.instance.AirMovementSpeed	    	=	AirMovementSpeed;
			GlobalVars.instance.UmbrellaGravity		    	=	UmbrellaGravity;
			GlobalVars.instance.UmbrellaFallingSpeed    =	UmbrellaFallingSpeed;
			GlobalVars.instance.SpeedDecay							=	SpeedDecay; 
			GlobalVars.instance.ExtraDivingSpeed				=	ExtraDivingSpeed; 
			GlobalVars.instance.SpeedDecayTimer					=	SpeedDecayTimer;
			GlobalVars.instance.Gravity				    			=	Gravity;
			GlobalVars.instance.PlayerMaxHealth					=	PlayerMaxHealth;
			GlobalVars.instance.UmbrellaMaxHealth				=	UmbrellaMaxHealth;
			GlobalVars.instance.FallDamageSpeedThreshold= FallDamageSpeedThreshold;
			GlobalVars.instance.RespawnTime							=	RespawnTime;
            GlobalVars.instance.TrueMute                            = TrueMute;
			Destroy(gameObject);  
		}
	}
		
	void Update()
    {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
		{
			Debug.Log("resetting level");
			SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
		}
    }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
	
}