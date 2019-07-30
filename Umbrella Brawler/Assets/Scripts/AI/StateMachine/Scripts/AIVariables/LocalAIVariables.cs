using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


// Fields will be added as AI's complexity grows
// Needs to use PlayerController
// this is the initial Local variables.
// this is localized to each player
// so we can change the vars (and make a new script obj) to make a weak ai,
// or we can change the vars to make an OP ai

[CreateAssetMenu(menuName = "AI/StateMachine/Local Enemy Variables")]
public class LocalAIVariables : ScriptableObject {

//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "LocalAIVariables";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:


	///// Obsolete vars ////////////////
	public float FocusAreaRange = 100f;
	public float FocusAreaRadius = 1f;
	public float SearchDuration = 4f;
	public float SearchingTurnSpeed = 120f;
	public float StoppingDistance = 2f;
	//////////////////////////////////


	/////See decision vars///////
	[Range(0,999)]
	public float MaxViewDistance = 50f;
	[Range(0,360)]
	public float fieldOfViewDegrees = 90f;
	[Range(0,999)]
	public float minPlayerDetectDistance = 10f;
	public Color GizmoColor = Color.green;
	/////////////////////////////


	/// NOTE: The final AI should not rely on local vars for navigation/attacking/etc
	// AI should follow Global Variables just like any other playeer
	///////circuit patrol vars//////
	[Range(1, 50)] public float MaxVelocity = 10.0f;
    [Range(1, 50)] public float MinVelocity = 2.0f;
    [Range(0, 20)] public float Acceleration = 0.8f;
    [Range(0, 1)] public float Inertia = 0.9f;
    [Range(0, 10)] public float RotaionDamping = 3.0f;
    [Range(1, 50)] public float WaypointStoppingDistance = 2.0f;
    [Range(0.1f, 60)] public float StopDelay = 1.0f;
    public bool LookRotation = true;
    public bool StopAtWaypoints = false;
	//////////////////////////


	/// These too should follow global instead
	//////// circuit pursue vars//////
	[Range(1, 50)] public float PursueMaxVelocity = 12.0f;
    [Range(1, 50)] public float PursueMinVelocity = 4.0f;
    [Range(0, 20)] public float PursueAcceleration = 0.8f;
    [Range(0, 1)] public float PursueInertia = 0.9f;
    [Range(0, 10)] public float PursueRotaionDamping = 3.0f;
    [Range(1, 50)] public float PursueWaypointStoppingDistance = 5.0f;
    [Range(0.1f, 60)] public float PursueStopDelay = 1.0f;
	public float SecondsLosingSightOfPlayer = 3.0f;
    public bool PursueLookRotation = true;
    public bool PursueStopAtWaypoints = false;
	/////////////////////////

	/// Hazard Avoidance /////
	[Range(1, 10)] public float GroundDistanceAvoidance = 5f;
	[Range(1, 10)] public float TopDistanceAvoidance = 10f;
	[Range(1,60)] public float UmbrellaOpenDuration = 1f;
	[Range(20,50)] public float UpwardForceThreshold = 20f;

	///Other
	public float DefaultTooLongInState = 5f;
	[Range(0,360)] public float VertAimbotOffsetAngle = 10f;

	// this gets divided by 2 because we ofsett negatively and positively
	[Range(0,360)] public float HorizAimbotOffsetAngle = 10f;
	public float angleTransitionTime = 0.5f;



	///////////////////GUN VARS which are obsolete
	// grab from weapon type
	// cooldown, range;
	// might have to go through the ole statescontroller again
	public float WeaponCooldown = 0;
	public float WeaponRange = 0;

	public void GrabWeaponVariables(StateController stateController){

	// Weapon Weapon;
	// GameObject WeaponContainer;
			
	// 	WeaponContainer = stateController.transform.Find("Weapon").gameObject;
	// 	Weapon =  (Weapon)FindObjectOfType(typeof(Weapon));
	// 	WeaponCooldown = Weapon.Cocktime;
	// 	WeaponRange = /* Weapon.Range;*/ 5;
	}

//--------------------------------------------------------------------------HELPERS:

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
