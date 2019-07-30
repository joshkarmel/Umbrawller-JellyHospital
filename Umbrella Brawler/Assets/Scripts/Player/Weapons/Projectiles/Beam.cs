using UnityEngine;
using System.Collections;
using MyUtility; //DON'T FORGET THIS

public class Beam : Projectile
{
//------------------------------------------------------------------------CONSTANTS:
	protected const string LOG_TAG = "Beam";

//---------------------------------------------------------------------------FIELDS:
	
//---------------------------------------------------------------------MONO METHODS:

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerInformation PInfo = other.gameObject.GetComponent<PlayerInformation>();
            DLog("Player Hit Team: " + PInfo.TeamNumber);
            if(PInfo.TeamNumber != PelletTeam && !PInfo.IsDead)
            {
                PInfo.ChangeHealth(-PelletDamage);
                DLog("Player Hit Health: " + PInfo.GetHealth());
                if(PInfo.GetHealth() <= 0 )
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
                umbrella.ModifyUmbrellaHP(-PelletDamage);
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            DLog("Hit " + other.gameObject.name);
        }
    }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
	
}