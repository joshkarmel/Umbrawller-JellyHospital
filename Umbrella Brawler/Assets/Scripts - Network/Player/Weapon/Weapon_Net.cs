using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS
using UnityEngine.Networking;
public abstract class Weapon_Net : NetworkBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:
	private const string LOG_TAG = "Weapon";
	public bool VERBOSE = false;

//---------------------------------------------------------------PUBLIC FIELDS:
    public enum AmmoType 
    {
        BIRDSHOT, 
        BUCKSHOT, 
        SLUG
    }

    [Header("Ammo Values")]
    public AmmoType Ammo;
    public int 
        MaxMags,
        PelletsPerShot,
        ShotsPerMag,
        PelletsPerMag;
    [Header("Pellet Values")]
    public float PelletSize;
    public float
        PelletDamage,
        PelletSpeed,
        PelletDrop,
        DamageDrop;

    [Header("Weapon Values")]
    public string WeaponName;
    public bool 
        AutoReload,
        ChargeRecoil,
        ChargeDamage;
    public float
        Range,
        Spread,
        RecoilPower,
        Cocktime,
        DelayRange,
        MaxChargeTime;

    [Header("Assigned GameObects")]
    public Transform ParentWeapon;
    public GameObject
        Player,
        Gun,
        ForwardBarrel,
        BackwardBarrel,
        Projectile,
        Reticle,
        Umbrella;
	public AudioClip ShotSound;
    
    [System.NonSerialized]
    public List<Quaternion> PelletList;

    public delegate void AmmoChanged();
    public event AmmoChanged OnAmmoChanged;

    //---------------------------------------------------------------PRIVATE FIELDS:
    protected int CurrentTotalPellets
    {
        get
        {
            return currentTotalPellets;
        }
        set
        {
            currentTotalPellets = value;
        }
    }

    protected int CurrentShotsLeftInMag
    {
        get
        {
            return currentShotsLeftInMag;
        }
        set
        {
            currentShotsLeftInMag = value;
            if (OnAmmoChanged != null)
            {
                OnAmmoChanged();
            }
        }
    }

    protected float tempCooldown, tempRecoil, currentCharge;
    protected bool activeCooldown, activeRecoil, isShooting;
    protected int isCharging; //0 = false, 1 = forward, 2 = backward
    
    protected int recoilType;//0 = false, 1 = forward, 2 = backward

    [Header("(DO NOT CHANGE)")]
    [SerializeField]
    protected int currentTotalPellets;
    [SerializeField]
    protected int currentShotsLeftInMag;
    [SerializeField]
    protected float CurrentMagsLeft;
    //[System.NonSerialized]
    public Axes.Action ShootForwardButton;
    //[System.NonSerialized]
    public Axes.Action ShootBackwardButton;
    [System.NonSerialized]
    public Axes.Action ReloadButton;
    protected PlayerController_Net PCtrl;
    protected PlayerInformation_Net PInfo;
    protected Rigidbody PlayerRb;
    protected AudioSource audioSource;

//---------------------------------------------------------------------MONO METHODS:
    public void Start () {
		

		PCtrl           = Player.GetComponent<PlayerController_Net>();
        PInfo           = Player.GetComponent<PlayerInformation_Net>();
        PlayerRb        = Player.GetComponent<Rigidbody>();
        tempCooldown    = 0f;
        tempRecoil      = 0f;
        activeCooldown  = false;
        activeRecoil    = false;
        recoilType      = 0;
        //ParentWeapon    = this.transform.parent;
        PelletList      = new List<Quaternion>(PelletsPerShot);
        PelletsPerMag   = PelletsPerShot * ShotsPerMag;
        CurrentTotalPellets = PelletsPerMag * MaxMags;
        CurrentShotsLeftInMag = ShotsPerMag;
        audioSource = this.GetComponent<AudioSource>();
        currentCharge = 0;
        isCharging = 0;

        switch(PInfo.PlayerNumber)
        {
            case Enums.Player.Player1:
                ShootForwardButton = Axes.Action.ShootForwardPlayerOne;
                ShootBackwardButton = Axes.Action.ShootBackwardPlayerOne;
                break;
            case Enums.Player.Player2:
                ShootForwardButton = Axes.Action.ShootForwardPlayerTwo;
                ShootBackwardButton = Axes.Action.ShootBackwardPlayerTwo;
                break;
            case Enums.Player.Player3:
                ShootForwardButton = Axes.Action.ShootForwardPlayerThree;
                ShootBackwardButton = Axes.Action.ShootBackwardPlayerThree;
                break;
            case Enums.Player.Player4:
                ShootForwardButton = Axes.Action.ShootForwardPlayerFour;
                ShootBackwardButton = Axes.Action.ShootBackwardPlayerFour;
                break;
            default: break;
        }

        for(int i=0; i<PelletsPerShot; i++)
        {
            PelletList.Add(Quaternion.Euler(Vector3.zero));
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(!Player.GetComponent<NetworkIdentity>().isLocalPlayer)	return;
		
        if(PCtrl.CanShoot()){
            CheckShoot();
            CheckRecoil();
        }
        
        if(activeCooldown)
            PerformCooldown();
        else checkCock();

        CheckReload();

        CurrentMagsLeft = CurrentTotalPellets / PelletsPerMag;
        ShotsPerMag = PelletsPerMag / PelletsPerShot;

        // if (Input.anyKey)
        //     DLog("True");
	}

//----------------------------------------------------------------ABSTRACT METHODS:
    protected abstract void Shoot(bool isForward);
    protected abstract void CheckShoot();
    protected abstract void CheckRecoil();

//--------------------------------------------------------------METHODS:

    //checks if the player cocked the gun and cannot shoot to reload
    protected void checkCock()
    {
        if(!PCtrl.CanShoot() && currentShotsLeftInMag < ShotsPerMag)
        {
            DLog("Reload!");
            Cock();
        }
    }

    //resets cooldown and enables the PlayerController to shoot
    protected void Cock()
    {
        tempCooldown = 0;
        PCtrl.EnableWeapon();
    }

    //disables the player to shoot for the PlayerController
    protected void DisableShoot()
    {
        PCtrl.DisableWeapon();
    }

    //checks in update in player reloads
    protected void CheckReload()
    {
        if(currentShotsLeftInMag <= 0)
        {
            if(AutoReload || Input.GetButtonDown(Axes.toStr[ReloadButton]))
            {
                DLog("Before Reload: " + currentShotsLeftInMag + "/" + ShotsPerMag);
                ReloadByShots();
            }
        }
    }

    public void ReloadByShots()
    {
        DisableShoot();
        StartCooldown();
        CurrentTotalPellets = PelletsPerShot * ShotsPerMag;
        CurrentShotsLeftInMag = ShotsPerMag;
        DLog("After Reload: " + currentShotsLeftInMag + "/" + ShotsPerMag);
    }

    protected void StartCooldown()
    {
        SetTempCooldown(Cocktime);
        SetCooldown(true);
    }

    //Counts upwards to cooldown limit and sets if cooldown is active until limit met
    protected void PerformCooldown()
    {
        if(tempCooldown > 0)
        {
            tempCooldown--;
            // DLog("Cooldown: "+ tempCooldown);
        }
        else
        {
            SetCooldown(false);
            PCtrl.EnableWeapon();
            Cock();
            DLog("Cooldown inactive");
        }
    }

    //Activates the recoil
    protected void ActivateRecoil(bool isForward)
    {
        activeRecoil = true;
        if(recoilType < 3)
            tempRecoil   = RecoilPower * 10f * (isForward ? -1 : 1);
        // DLog("TempRecoil: " + tempRecoil);
    }

    protected void DeactivateRecoil()
    {
        activeRecoil = false;
        recoilType = 0;
    }

    //Aims weapon where the reticle is pointing at
    protected void AimWeapon()
    {
        Transform retTransform = Reticle.transform;
        RaycastHit hit;
        Ray ray = new Ray(retTransform.position, retTransform.forward);
        LayerMask ignoreLayer = (1 << Umbrella.layer);
        if(Physics.Raycast(retTransform.position, retTransform.forward, out hit, ignoreLayer))
        {
            Vector3 newPlayerRotLoc = new Vector3(hit.point.x, Player.transform.position.y, hit.point.z);
            Player.transform.LookAt(newPlayerRotLoc);
            transform.LookAt(hit.point);
        }
    }

    // Getters and Setters for Private Variables
    protected virtual void SetCooldown(bool status)
    {
        activeCooldown = status;
    }

    protected virtual void SetTempCooldown(float tmp)
    {
        tempCooldown = tmp;
    }

    protected virtual void SetTempRecoil(float newRecoil)
    {
        tempRecoil = newRecoil;
    }

    protected virtual void SetActiveRecoil(bool status)
    {
        activeRecoil = status;
    }

    public bool GetCooldown()
    {
        return activeCooldown;
    }

    public float GetTempCooldown()
    {
        return tempCooldown;
    }

    public float GetTempRecoil()
    {
        return tempRecoil;
    }

    public bool GetActiveRecoil()
    {
        return activeRecoil;
    }

    public int GetCurrentPellets()
    {
        return CurrentTotalPellets;
    }

    public float GetCurrentMagsLeft()
    {
        return CurrentMagsLeft;
    }

    public float GetCurrentShotsLeftInMag()
    {
        return currentShotsLeftInMag;
    }

    public void setShootForwardButton(Axes.Action a)
    {
        ShootForwardButton = a;
    }

    public void setShootBackwardButton(Axes.Action a)
    {
        ShootBackwardButton = a;
    }

    public void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
