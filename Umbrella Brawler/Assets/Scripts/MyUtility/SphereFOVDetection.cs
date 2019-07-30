using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public class SphereFOVDetection : MonoBehaviour {

//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "StateController";
    public bool VERBOSE = false;


//---------------------------------------------------------------------------FIELDS:

	[SerializeField] private bool playerInSight;

//--------------------------------------------------------------------------METHODS:
	



	public List<GameObject> DetectableObjects;
	public Transform Center;
	public string TargetTag;
	public float MaxConeView;
	public float FOVDegrees;
	public float sphereRadius;

	public Color GizmoColor = Color.green;

	public List<GameObject> PlayersDetected = new List<GameObject>();



	void Start(){

	}

	void Update(){
		if(DetectPlayer()){
			GizmoColor = Color.red;
		}
		else{
			GizmoColor = Color.green;
		}
	}

	public SphereFOVDetection(List<GameObject> c_DetectableObjects,
							Transform c_Center, 
							string c_TargetTag, 
							float c_MaxConeView, 
							float c_FOVDegrees, 
							float c_SphereRadius){

       DetectableObjects = c_DetectableObjects;
	   Center = c_Center;
	   TargetTag = c_TargetTag;
	   MaxConeView = c_MaxConeView;
	   FOVDegrees = c_FOVDegrees;
	   sphereRadius = c_SphereRadius;
	}

	
	public void setSphere(List<GameObject> c_DetectableObjects,
							Transform c_Center, 
							string c_TargetTag, 
							float c_MaxConeView, 
							float c_FOVDegrees, 
							float c_SphereRadius,
							Color gizmoC){

       DetectableObjects = c_DetectableObjects;
	   Center = c_Center;
	   TargetTag = c_TargetTag;
	   MaxConeView = c_MaxConeView;
	   FOVDegrees = c_FOVDegrees;
	   sphereRadius = c_SphereRadius;
	   GizmoColor = gizmoC;
	}

   	///<Summary>
    /// Creates a bunch of rays in a cone (AI FOV) coming out from Focus Area
	/// Creates a sphere for detecting nearby enemies
	/// Returns true when an enemy is within sight
	///</Summary>
	public bool DetectPlayer(){

		bool isHitPlayerTrue;
		RaycastHit hit;
		List<Vector3> rayDirections = new List<Vector3>();
		List<float> distancesToPlayers = new List<float>();

		foreach(GameObject player in DetectableObjects){
			rayDirections.Add( player.transform.position - Center.position);
			distancesToPlayers.Add(Vector3.Distance(Center.position, player.transform.position));
		}

		foreach(GameObject player in DetectableObjects){
			for(int i = 0; i < rayDirections.Count; i++){
				// Debug.DrawRay(Center.position, rayDirections[i], Color.yellow);
				if(Physics.Raycast (Center.position, rayDirections[i], out hit)){ 
					if((hit.transform.tag == TargetTag) && (distancesToPlayers[i] <= sphereRadius)){
						isHitPlayerTrue = (hit.transform.CompareTag(TargetTag));
						// DLog("" + PlayersDetected.Count);
						if(!PlayersDetected.Contains(hit.transform.gameObject))
							PlayersDetected.Add(hit.transform.gameObject);
						return isHitPlayerTrue;
					}
				}
				else{
					return false;
				}
			}

		}

		for(int i = 0; i < rayDirections.Count; i++){
			if ((Vector3.Angle(rayDirections[i], Center.forward)) <= FOVDegrees * 0.5f){
				if (Physics.Raycast(Center.position, rayDirections[i], out hit, MaxConeView))
				{	isHitPlayerTrue = (hit.transform.CompareTag(TargetTag));
					PlayersDetected.Add(hit.transform.gameObject);
					return isHitPlayerTrue;
				}
			}
		}
		return false;	 
    }

	public List<GameObject> GrabDetectedGameObjects(){
		return PlayersDetected;
	}


	void OnDrawGizmos(){
		if(this.enabled){
			// the sphere in which you know whether the player is there
			Transform transform = Center.transform;
			Gizmos.color = GizmoColor;
			Gizmos.DrawWireSphere(transform.position, sphereRadius);

			/// the area of sight cone thing
			float halfFOV = FOVDegrees / 2.0f;

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

			Gizmos.DrawRay(transform.position, forwardRayDireaction * MaxConeView);

			Gizmos.DrawRay(transform.position, leftRayDireaction * MaxConeView);
			Gizmos.DrawRay(transform.position, rightRayDireation * MaxConeView);
			Gizmos.DrawRay(transform.position, upRayDireaction * MaxConeView);
			Gizmos.DrawRay(transform.position, downRayDireation * MaxConeView);

			Gizmos.DrawRay(transform.position, topLeftRayDireaction * MaxConeView);
			Gizmos.DrawRay(transform.position, topRightRayDireation * MaxConeView);
			Gizmos.DrawRay(transform.position, bottomLeftRayDireaction * MaxConeView);
			Gizmos.DrawRay(transform.position, bottomRightRayDireation * MaxConeView);
		}
	}


//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
