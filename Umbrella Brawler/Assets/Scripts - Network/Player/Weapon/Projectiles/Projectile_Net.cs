using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility; //DON'T FORGET THIS
using UnityEngine.Networking;
public class Projectile_Net : NetworkBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	protected const string LOG_TAG = "Projectile_Net";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public float PelletDamage;
    public Enums.Team PelletTeam;
    public GameObject PCtrl;

//---------------------------------------------------------------------------PRIVATE FIELDS:
    private float range, delayedRange;
    private Vector3 startPos, delayedVelocity;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
        startPos = transform.position;
	}
		
	[ServerCallback]
	void Update()
    {
        if(range > 0)
            CheckRange();
    }

//--------------------------------------------------------------------------METHODS:

    public void CheckRange()
    {
        float distance = Vector3.Distance(startPos, transform.position);
        if(distance > delayedRange && gameObject.GetComponent<Rigidbody>().velocity != delayedVelocity)
        {
            VERBOSE = true;
            DLog("Changed Vel");
            gameObject.GetComponent<Rigidbody>().velocity = delayedVelocity;
            VERBOSE = false;
        }
        if(distance > range)
        {
            DLog("Range destroy");
            NetworkServer.Destroy(gameObject);
        }
    }

    public float Damage()
    {
        return PelletDamage;
    }

    private void OnCollisionEnter(Collision other) 
    {
		//If this is not the server, leave.
		if(!isServer)
			return;

        if(other.gameObject.tag == "Player")
        {
            PlayerInformation_Net PInfo = other.gameObject.GetComponent<PlayerInformation_Net>();
            DLog("Player Hit Team: " + PInfo.TeamNumber);
            if(PInfo.TeamNumber != PelletTeam)
            {
                PInfo.ChangeHealth(-PelletDamage);
                DLog("Player Hit Health: " + PInfo.GetHealth());
                if(PInfo.GetHealth() <= 0 )
                {
                    PInfo.KilledByTeam = PelletTeam;
                    DLog(PelletTeam + " killed " + PInfo.TeamNumber);
                }
            }
        }
        else if(other.gameObject.tag == "umbrella")
        {
            Umbrella_Net umbrella = other.gameObject.GetComponentInParent<Umbrella_Net>();
            PlayerInformation_Net PInfo = other.gameObject.GetComponent<PlayerInformation_Net>();

            if(PInfo.TeamNumber != PelletTeam)
            {
                umbrella.ModifyUmbrellaHP(-PelletDamage);
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            DLog("Hit " + other.gameObject.name);
        }
        NetworkServer.Destroy(gameObject);
    }

//--------------------------------------------------------------------------GETTERS/SETTERS:
    public void SetTeam(Enums.Team team)
    {
        PelletTeam = team;
    }

    public void SetDamage(float damage)
    {
        PelletDamage = damage;
    }

    public void SetRange(float range)
    {
        this.range = range;
    }

    public void SetPlayer(GameObject pctrl)
    {
        pctrl = PCtrl;
    }

    public void SetDelayedVel(Vector3 newvel)
    {
        delayedVelocity = newvel;
    }

    public void SetDelayedRange(float newdist)
    {
        delayedRange = newdist;
    }

//--------------------------------------------------------------------------HELPERS:

    protected void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}