using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility;
 
public class MapCircuitAI : MonoBehaviour{

//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
    

    [HideInInspector] public float MaxVelocity = 10.0f;
    [HideInInspector] public float MinVelocity = 2.0f;
    [HideInInspector] public float Acceleration = 0.8f;
    [HideInInspector] public float Inertia = 0.9f;
    [HideInInspector] public float RotaionDamping = 3.0f;
    [HideInInspector] public float WaypointStoppingDistance = 2.0f;
    [HideInInspector] public float StopDelay = 1.0f;
    
    public bool LookRotation = true;
    [HideInInspector] public bool StopAtWaypoints = false;
    ///////////////////

    private Vector3 currentVector; // this is the position of wp
    private Transform currentTransform; // this is the new waypoint
    private Transform currentPlayerTransform;
    private float currentVelocity = 0.0f; 
    public bool isChasing;

    public GameObject thisAI; // you must set this from outside

    

    [HideInInspector] public bool accelerating;
    [HideInInspector] public SphereCollider stoppingCollider;
    

//---------------------------------------------------------------------MONO METHODS:
   
    void Awake(){

        
        // initial waypoint upon awake
        isChasing = false;
        addVector3ToNewTransform();
        addSphereColliderToTransform(currentTransform);
        
    }

    private void addVector3ToNewTransform(){
        GameObject emptyGO = new GameObject();
        currentTransform = emptyGO.transform;
        if(GameManager.Instance.m_AIOn){
            currentVector = getVector3FromScene();
        }
        currentTransform.position = currentVector;
        currentTransform.gameObject.tag = "EnvObjWayPoint";
    }

    public void addVector3ToPlayerTransform(Vector3 playerPosition){

        currentTransform.position = playerPosition;
        //currentTransform.gameObject.tag = "Player"; // it should already be set
    }

    // temp: only purpose is to reset when bumped into?
    public void SetPlayerAsCurrentTransform(Transform playerTransform){
        currentPlayerTransform = playerTransform;
    }

    public void SetIsChasing(bool chase){
        isChasing = chase;
    }

    private void addSphereColliderToTransform(Transform transform){
        if(currentTransform != null){
            stoppingCollider = currentTransform.gameObject.AddComponent<SphereCollider>();
            stoppingCollider.center = Vector3.zero;
            stoppingCollider.radius = WaypointStoppingDistance;
            stoppingCollider.isTrigger = true;
        }
    }

    private Vector3 getVector3FromScene(){
        return GameManager.Instance.createVector(GameManager
        .Instance.CircuitArea);
    }

    void Start(){
        
        accelerating = true;
    }

 
    // void Update(){
    // //    /// MIGHT NEED TO GO 
    // //          stoppingCollider.radius = WaypointStoppingDistance;
       
    // //     if (accelerating){
    // //         Accelerate();
    // //     }
    // //     if (!accelerating){
    // //         if(StopAtWaypoints){
    // //             StartCoroutine(Decelerate());
    // //         }else{
    // //             Accelerate();
    // //         }
    // //     } 
    // //     ///MIGHT NEED TO GO

    // }

    public void setThisAI(GameObject GO){
        thisAI = GO;
    }

    void OnTriggerEnter(Collider other){
        if (!enabled) return;

        if(!isChasing){
            if(other.tag == "Player"){ // if it IS chasing
                return;
            }
            if(other.tag == "EnvObjWayPoint"){
                Destroy(other.gameObject); 
                accelerating = false;
                Debug.Log("Circuit AI passed through waypoint");     
                addVector3ToNewTransform(); // STOP THIS WHEN is chasing
                addSphereColliderToTransform(currentTransform);
                
            }

            
        }

        if(isChasing){

            if(other.tag == "EnvObjWayPoint"){
                return;
                
            }
            if(other.tag == "Player"){ // if it IS chasing
            return;
            // Debug.Log("Circuit AI passed through OR COLLIDED WITH PLAYER!");
            // Destroy(other.gameObject);
            // Debug.Log("collided but is still chasing, generate new thingy");
            // addVector3ToPlayerTransform(currentTransform.position);
            // addSphereColliderToTransform(currentTransform);
            
        }
        
        
            // else{
            //     Destroy(other.gameObject); 
            //     accelerating = false;
            //     Debug.Log("hit player, but is no longer chasing");     
            //     addVector3ToNewTransform(); // STOP THIS WHEN is chasing
            //     addSphereColliderToTransform(currentTransform);
            // }
            
        }
        // else{
        //     if(other.tag == "EnvObjWayPoint"){
        //         Destroy(other.gameObject); 
        //         accelerating = false;
        //         Debug.Log("Circuit AI passed through waypoint");     
        //         addVector3ToNewTransform(); // STOP THIS WHEN is chasing
        //         addSphereColliderToTransform(currentTransform);
                
        //     }
        // }
      
    }

//--------------------------------------------------------------------------METHODS:
 
    public void Accelerate(){
       
        if (currentTransform){
                currentVelocity = currentVelocity + Mathf.Pow(Acceleration, 2);
            if (LookRotation){
                stablizeMaxVelocity();

                Vector3 vectorZeroCorrection = new Vector3(0.00001f, 0.00001f, 0.00001f);
                // Vector3 vectorZeroCorrection = currentTransform.position - thisAI.transform.position;
                var rotation = Quaternion.LookRotation(currentTransform.position - thisAI.transform.position + vectorZeroCorrection);
                thisAI.transform.rotation = Quaternion.Slerp(thisAI.transform.rotation, rotation, Time.deltaTime * RotaionDamping);  
                thisAI.transform.Translate(0, 0, Time.deltaTime * currentVelocity);
            }else{
                stablizeMaxVelocity();
                 thisAI.transform.position = Vector3.MoveTowards(thisAI.transform.position, 
                                                            currentTransform.position, 
                                                            Time.deltaTime * currentVelocity);
            }
        }     
    }

    public IEnumerator Decelerate(){
         currentVelocity = currentVelocity * Inertia;
        if(LookRotation){
       
            thisAI.transform.Translate (0, 0, Time.deltaTime * currentVelocity);
            if(currentVelocity <= MinVelocity){ // there is a bug here. sometimes it never reaches minimum velocity
                currentVelocity = 0.0f;
                yield return new WaitForSeconds(StopDelay);
                accelerating = true;
            }
        }else if(!LookRotation){  

            currentVelocity = 0.0f;
            thisAI.transform.position = Vector3.MoveTowards(thisAI.transform.position,
                                                    currentTransform.position,
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