using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility;
 
public class MapCircuitObject : MonoBehaviour{

//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
    
    
    [Range(1, 50)] public float MaxVelocity = 10.0f;
    [Range(1, 50)] public float MinVelocity = 2.0f;
    [Range(0, 20)] public float Acceleration = 0.8f;
    [Range(0, 1)] public float Inertia = 0.9f;
    [Range(0, 10)] public float RotaionDamping = 3.0f;
    [Range(1, 50)] public float StoppingDistance = 2.0f;
    [Range(0.1f, 60)] public float StopDelay = 1.0f;
    
    public bool LookRotation = true;
    public bool StopAtWaypoints = false;
    public List<Transform> Waypoints = new List<Transform>();

    
    private float currentVelocity = 0.0f;
    private int waypointIndex;
    private bool accelerating;
    private Transform currentWaypoint;
    private List<SphereCollider> stoppingColliders = new List<SphereCollider>();

//---------------------------------------------------------------------MONO METHODS:
   
    void Awake(){
        if(Waypoints.Count > 0){
            for(int i = 0; i < Waypoints.Count; i++){  
                stoppingColliders.Add(Waypoints[i].gameObject.AddComponent<SphereCollider>());
                stoppingColliders[i].center = Vector3.zero;
                stoppingColliders[i].radius = StoppingDistance;
                stoppingColliders[i].isTrigger = true;
            }
        }
    }

    void Start(){
        accelerating = true;
    }
 
    void Update(){
        for(int i = 0; i < Waypoints.Count; i++){  
             stoppingColliders[i].radius = StoppingDistance;
        }
       
        if (accelerating){
            Accelerate();
        }
        if (!accelerating){
            if(StopAtWaypoints){
                StartCoroutine(Decelerate());
            }else{
                Accelerate();
            }
        } 
        currentWaypoint = Waypoints[waypointIndex]; 
    }

    void OnTriggerEnter(Collider other){

        if(other.tag == "EnvObjWayPoint"){
            accelerating = false;
            waypointIndex = (waypointIndex + 1) % Waypoints.Count;
            Debug.Log("Circuit object passed through waypoint");     
        }

        // if(other.tag == "Player"){
        //     other.gameObject.transform.parent = this.transform;
        //     Debug.Log("Player is CHILDED to circuit object");
        // }

        // Debug.Log("SOMETHING HIT THE CIRCUIT OBJECT");
      
    }

    // void OnTriggerExit(Collider other){
    //     if(other.tag == "Player"){
    //         other.gameObject.transform.parent = null;
    //         Debug.Log("Player is not a child. Player is a BIG, STRONG, POWERFUL MAN!");
    //     }
    // }

//--------------------------------------------------------------------------METHODS:
 
    private void Accelerate(){
       
        if (currentWaypoint){
                currentVelocity = currentVelocity + Mathf.Pow(Acceleration, 2);
            if (LookRotation){
                stablizeMaxVelocity();
                var rotation = Quaternion.LookRotation(currentWaypoint.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotaionDamping);  
                transform.Translate(0, 0, Time.deltaTime * currentVelocity);
            }else{
                stablizeMaxVelocity();
                 transform.position = Vector3.MoveTowards(transform.position, 
                                                            currentWaypoint.position, 
                                                            Time.deltaTime * currentVelocity);
            }
        }     
    }

    private IEnumerator Decelerate(){
         currentVelocity = currentVelocity * Inertia;
        if(LookRotation){
       
            transform.Translate (0, 0, Time.deltaTime * currentVelocity);
            if(currentVelocity <= MinVelocity){ // there is a bug here. sometimes it never reaches minimum velocity
                currentVelocity = 0.0f;
                yield return new WaitForSeconds(StopDelay);
                accelerating = true;
            }
        }else if(!LookRotation){  

            currentVelocity = 0.0f;
            transform.position = Vector3.MoveTowards(transform.position,
                                                    currentWaypoint.position,
                                                    Time.deltaTime * currentVelocity); 
            yield return new WaitForSeconds(StopDelay);
            accelerating = true;   
        } 
    }

     private void stablizeMaxVelocity(){
        if(currentVelocity >= MaxVelocity){
            currentVelocity = MaxVelocity;
        } 
    }

    // Not in use yet. Gives the path a more natural curve.
    private Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t) {
         return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
    }

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
 
}