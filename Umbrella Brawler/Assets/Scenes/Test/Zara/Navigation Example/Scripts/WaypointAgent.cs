// This is just a test.
// We are not using this in the project

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]


public class WaypointAgent : MonoBehaviour {


    // Inspector Assiged
    public WaypointNetwork WaypointNetwork = null;
    public NavMeshPathStatus PathSatus = NavMeshPathStatus.PathInvalid;
    public AnimationCurve JumpArc = new AnimationCurve();
    public int CurrentIndex = 0;
    public bool HasPath = false;
    public bool PathPending = false;
    public bool IsPathStale = false;
    public bool AgentJumping = false;


    //Private
    private NavMeshAgent _waypointAgent = null;

    // Use this for initialization
    void Start () {
        
        _waypointAgent = GetComponent<NavMeshAgent>();

        if (WaypointNetwork == null) {
            return;
            }

        SetNextDestination(false);
	}

    void SetNextDestination(bool hasNextDest) {

        if (!WaypointNetwork) {
            return;
            }

        int increment = 0;
        int nextWayPoint = 0;
        Transform nextWaypointTransform = null;
        
        if(hasNextDest){
            increment = 1;
        }

        if (CurrentIndex + increment >= WaypointNetwork.Waypoints.Count){
            nextWayPoint = 0;
        } else{
            nextWayPoint = CurrentIndex + increment;
        }
    
        nextWaypointTransform = WaypointNetwork.Waypoints[nextWayPoint];

        if (nextWaypointTransform != null) {
            CurrentIndex = nextWayPoint;
            _waypointAgent.destination = nextWaypointTransform.position;
            return;
        }
        //didnt find a valid waypoint
        CurrentIndex++;
    }
	
	// Update is called once per frame
	void Update () {
        HasPath = _waypointAgent.hasPath;
        PathPending = _waypointAgent.pathPending;
        IsPathStale = _waypointAgent.isPathStale;
        PathSatus = _waypointAgent.pathStatus;

        if (_waypointAgent.isOnOffMeshLink)
        {
            if (!AgentJumping)
            {
                AgentJumping = true;
                StartCoroutine(Jump(1.0f));
            }
        }

        if (_waypointAgent.remainingDistance <= _waypointAgent.stoppingDistance && (!PathPending || PathSatus == NavMeshPathStatus.PathInvalid)){
            SetNextDestination(true);
        }
        else if (_waypointAgent.isPathStale) {
            SetNextDestination(false);
        }
        else{
            SetNextDestination(false);
        }
	}

    IEnumerator Jump (float duration){

        OffMeshLinkData offMeshLinkData = _waypointAgent.currentOffMeshLinkData;
        Vector3 startPos = _waypointAgent.transform.position;
        Vector3 endPos = offMeshLinkData.endPos + (_waypointAgent.baseOffset * Vector3.up);
        float time = 0.0f;

        while(time <= duration){
             
            float t = time/duration;
            _waypointAgent.transform.position = Vector3.Lerp(startPos, endPos, t) + (JumpArc.Evaluate(t) * Vector3.up);
            time = time + Time.deltaTime;
            yield return null;

        }
        _waypointAgent.CompleteOffMeshLink();
        AgentJumping = false;
    }

}
