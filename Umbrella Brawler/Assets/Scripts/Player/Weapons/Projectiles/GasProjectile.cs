using UnityEngine;
using System.Collections;
using MyUtility; //DON'T FORGET THIS

public class GasProjectile : Projectile
{
//------------------------------------------------------------------------CONSTANTS:

//---------------------------------------------------------------------------FIELDS:
    public float lifetime;
    protected float timer = 30;
    
//---------------------------------------------------------------------MONO METHODS:

    private void Start()
    {
        // transform.rotation = PCtrl.transform.rotation;    
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerInformation PInfo = other.gameObject.GetComponent<PlayerInformation>();
            DLog("Player Hit Team: " + PInfo.TeamNumber);

            if(timer < 0)
            {
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
                timer = 30;
            }
            else
            {
                timer--;
            }
        }
    }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
}