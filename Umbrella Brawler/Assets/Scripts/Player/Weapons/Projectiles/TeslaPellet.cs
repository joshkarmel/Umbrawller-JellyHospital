using UnityEngine;
using System.Collections;
using MyUtility; //DON'T FORGET THIS

public class TeslaPellet : Projectile
{
//------------------------------------------------------------------------CONSTANTS:

	private bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:

    [Tooltip("Length of fire damage in seconds")]
    public float statCooldown;
    public float DPS;
    public float slowdown;
	
//---------------------------------------------------------------------MONO METHODS:

//--------------------------------------------------------------------------METHODS:

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerInformation PInfo = other.gameObject.GetComponent<PlayerInformation>();
            DLog("Player Hit Team: " + PInfo.TeamNumber);
            if(PInfo.TeamNumber != PelletTeam && !PInfo.IsDead)
            {
                PInfo.ChangeHealth(-Damage(PInfo));
                PInfo.setTeslaStatus(DPS, statCooldown, slowdown);
                DLog("Player Hit Health: " + PInfo.GetHealth());
                if(PInfo.GetHealth() <= 0)
                {
                    PInfo.KilledByTeam = PelletTeam;
                    DLog(PelletTeam + " killed " + PInfo.TeamNumber);
                    if(GameManager.Instance.gameModeTracker != null)
                        GameManager.Instance.gameModeTracker.AddScore();
                }
            }
        }
        else if(other.gameObject.tag == "umbrella")
        {
            UmbrellaBase umbrella = other.gameObject.GetComponentInParent<UmbrellaBase>();
            PlayerInformation PInfo = other.gameObject.GetComponent<PlayerInformation>();
            if(PInfo.TeamNumber != PelletTeam)
            {
                umbrella.ModifyUmbrellaHP(-Damage(PInfo));
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            DLog("Hit " + other.gameObject.name);
        }
        rend.enabled = false;
        DestroyParticles.SetActive(true);
        Destroy(gameObject, 0.1f);
    }

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}