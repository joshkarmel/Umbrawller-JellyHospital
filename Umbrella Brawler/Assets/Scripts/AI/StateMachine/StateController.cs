using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MyUtility;

public class StateController : MonoBehaviour
{
//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "StateController";
    public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	
	public State CurrentState;
	public State StayInState;
	public Transform FocusArea;
	public LocalAIVariables LocalAIVariables;
	public GlobalAIVariables GlobalAIVariables;
	public GameObject CurrentTarget;
	public Weapon WeaponAI;


	[HideInInspector] public NavMeshAgent NavMeshAgent; // We will not be using navmesh in the final game
	[HideInInspector] public PlayerController PlayerControllerAI;
	[HideInInspector] public PlayerInformation PlayerInformationAI;
	 

	[HideInInspector] public List<Transform> WayPointList; // might not need this anymore
	[HideInInspector] public int NextWayPoint; // or this
	[HideInInspector] public Transform GoToTarget;
	[HideInInspector] public float TimeInState;
	[HideInInspector] public float timer;
	[HideInInspector] public bool doActionOnce, setVariablesOnce; // wont need these
	

	[HideInInspector] public MapCircuitAI MapCircuitAI;
	[HideInInspector] public PlayerManager PlayerManagerAI; // remember use this to modify canmove
	[HideInInspector] public List<PlayerManager> otherPlayersInScene = new List<PlayerManager>(); 
	[HideInInspector] public List<GameObject> otherPlayersGO = new List<GameObject>();
	[HideInInspector] public Vector3 LastShotFromNormVector;
	[HideInInspector] public Vector3 LastDirTargetTooClose;
	private bool isAIOn;

//---------------------------------------------------------------------MONO METHODS:

	void Awake() 
	{
		timer = 0f;
		doActionOnce = false; //by default
		setVariablesOnce = false;
		PlayerControllerAI = this.transform.GetComponent<PlayerController>();
		PlayerInformationAI = GetComponent<PlayerInformation>();
		NavMeshAgent = GetComponent<NavMeshAgent>();
		MapCircuitAI = GetComponent<MapCircuitAI>();
	}

	void Start(){
		otherPlayersInScene = GameManager.Instance.Players;
		MapCircuitAI.setThisAI(this.gameObject);
		WeaponAI = PlayerControllerAI.PlayerWeapon;

		for(int i = 0; i < otherPlayersInScene.Count; i++){
			if(!otherPlayersInScene[i].isAI()){
				otherPlayersGO.Add(otherPlayersInScene[i].m_PlayerInstance);
			}
		}
		GlobalAIVariables.GrabPlayerControllerGlobals(this);
		LocalAIVariables.GrabWeaponVariables(this);
	}
		
	void Update()
    {
		if(!isAIOn){
			return;
		}
		CurrentState.UpdateState(this);
		
		if(GoToTarget != null){
			CurrentTarget = GoToTarget.gameObject;
		}
    }

//--------------------------------------------------------------------------METHODS:

	public void SetupAI(bool PlayerManagerIsAIOn, List<Transform> PlayerManagerWayPointList){

		isAIOn = PlayerManagerIsAIOn;
		WayPointList = PlayerManagerWayPointList;
		
		if(isAIOn){
			NavMeshAgent.enabled = false;
			MapCircuitAI.enabled = true;
		}
		else{
			NavMeshAgent.enabled = false;
			MapCircuitAI.enabled = false;
		}
		
	}

	void OnDrawGizmos(){
		if(this.enabled){
			// the sphere in which you know whether the player is there
			Transform transform = this.FocusArea.transform;
			Gizmos.color = CurrentState.SceneFocusAreaColor;
			Gizmos.DrawWireSphere(transform.position, this.LocalAIVariables.minPlayerDetectDistance);
			float maxView = this.LocalAIVariables.MaxViewDistance;


			/// the area of sight cone thing
			float halfFOV = this.LocalAIVariables.fieldOfViewDegrees / 2.0f;

			Quaternion forwardRayRotation = Quaternion.AngleAxis(0, Vector3.up );

			Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, new Vector3(0, 1, 0));
			Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, new Vector3(0, 1, 0));
			Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV, new Vector3(1, 0, 0));
			Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV, new Vector3(1, 0, 0));

			Quaternion topLeftRayRotation = Quaternion.AngleAxis(-halfFOV, new Vector3(-1, 1, 0));
			Quaternion topRightRayRotation = Quaternion.AngleAxis(halfFOV, new Vector3(-1, 1, 0));
			Quaternion bottomLeftRayRotation = Quaternion.AngleAxis(-halfFOV, new Vector3(1, 1, 0));
			Quaternion bottomRightRayRotation = Quaternion.AngleAxis(halfFOV, new Vector3(1, 1, 0));

			Vector3 forwardRayDireaction = forwardRayRotation * transform.forward;

			Vector3 leftRayDireaction = leftRayRotation * transform.forward;
			Vector3 rightRayDireation = rightRayRotation * transform.forward;
			Vector3 upRayDireaction = upRayRotation * transform.forward;
			Vector3 downRayDireation = downRayRotation * transform.forward;

			Vector3 topLeftRayDireaction = topLeftRayRotation * transform.forward;
			Vector3 topRightRayDireation = topRightRayRotation * transform.forward;
			Vector3 bottomLeftRayDireaction = bottomLeftRayRotation * transform.forward;
			Vector3 bottomRightRayDireation = bottomRightRayRotation * transform.forward;

			Gizmos.DrawRay(transform.position, forwardRayDireaction * maxView);

			Gizmos.DrawRay(transform.position, leftRayDireaction * maxView);
			Gizmos.DrawRay(transform.position, rightRayDireation * maxView);
			Gizmos.DrawRay(transform.position, upRayDireaction * maxView);
			Gizmos.DrawRay(transform.position, downRayDireation * maxView);

			Gizmos.DrawRay(transform.position, topLeftRayDireaction * maxView);
			Gizmos.DrawRay(transform.position, topRightRayDireation * maxView);
			Gizmos.DrawRay(transform.position, bottomLeftRayDireaction * maxView);
			Gizmos.DrawRay(transform.position, bottomRightRayDireation * maxView);
		}
	}

	public void TransitionToNextState(State nextState){
		if(nextState != StayInState){
			CurrentState = nextState;
			OnExitState();
		}
	}

	public bool CheckTimeElapsedInState(float duration){
		TimeInState += Time.deltaTime;
		return(TimeInState >= duration);
	}

	public bool CheckTimeDotTimeElapsedInState(float duration){
		TimeInState += Time.time;
		return(TimeInState >= duration);
	}


	public void SetAIGravity(bool gravity){
		PlayerControllerAI.GravityEnabled(gravity);
	}

	public void SetAICanMove(bool movement){
		PlayerControllerAI.CanMove = movement;
	}

	public void SetAIIsKinematic(bool kinematic){
		PlayerControllerAI.KinematicEnabled(kinematic);
	}


	private void OnExitState(){
		timer = 0;
		TimeInState = 0;
		setVariablesOnce = false;
		doActionOnce = false;
		// below is specific to one action, and should not be here
		// this switching on and off thing, might have to use events and delegates
		MapCircuitAI.LookRotation = true;
	}


//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}