using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility; //DON'T FORGET THIS

public class Projectile : MonoBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	protected const string LOG_TAG = "Projectile";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public float PelletDamage;
    public float damageBoostMultiplier;
    public Renderer rend;
    public Enums.Team PelletTeam;
    public Enums.Player PlayerID;
    public GameObject PCtrl;
    public HitMarker HitMarker;
    public GameObject DestroyParticles;
    public GameObject PlayerDamageParticles;
    public Transform pdParticleSpawner;

//---------------------------------------------------------------------------PRIVATE FIELDS:
    protected float range, delayedRange;
    protected Vector3 startPos, delayedVelocity;
//---------------------------------------------------------------------MONO METHODS:

	protected void Start() 
	{
        startPos = transform.position;
        rend = GetComponent<Renderer>();
        damageBoostMultiplier = 1.5f;
	}
		
	protected void Update()
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
            DLog("Changed Vel");
            gameObject.GetComponent<Rigidbody>().velocity = delayedVelocity;
        }
        if(distance > range)
        {
            DLog("Range destroy");
            Destroy(gameObject);
        }
    }

    public float Damage(PlayerInformation pInfo)
    {
        if (pInfo.isDamageBoostActive())
            return PelletDamage * damageBoostMultiplier;
        else
            return PelletDamage;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerInformation PInfo = other.gameObject.GetComponent<PlayerInformation>();
            Instantiate(PlayerDamageParticles, pdParticleSpawner.position, pdParticleSpawner.rotation);
            DLog("Player Hit Team: " + PInfo.TeamNumber);
            if(PInfo.TeamNumber != PelletTeam && !PInfo.IsDead)
            {
                PInfo.ChangeHealth(-Damage(PInfo));
                DLog("Player Hit Health: " + PInfo.GetHealth());
                HitMarker.HitEnemy();
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

    public void SetBulletPlayerID(Enums.Player player)
    {
        PlayerID = player;
    }

    public void SetDelayedVel(Vector3 newvel)
    {
        delayedVelocity = newvel;
    }

    public void SetDelayedRange(float newdist)
    {
        delayedRange = newdist;
    }

    public void SetHitMarker(HitMarker reticle)
    {
        HitMarker = reticle;
    }

//--------------------------------------------------------------------------HELPERS:

    protected void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}