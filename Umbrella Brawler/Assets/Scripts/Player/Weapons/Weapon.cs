using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS

public abstract class Weapon : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:
	private const string LOG_TAG = "Weapon";
	public bool VERBOSE = false;

//---------------------------------------------------------------PUBLIC FIELDS:
    public enum StatusType 
    {
        NORMAL, 
        FIRE, 
        ELECTRIC
    }

    [Header("Input Values")]
    public Axes.Action ShootForwardButton;
    public Axes.Action ShootBackwardButton;
    public Axes.Action ReloadButton;

    [Header("Ammo Values")]
    [Tooltip("Type of Ammo the gun uses. Fire, Electric, Normal.")]
    public StatusType AmmoType;
    
    [Tooltip("Maximum amount of Magazines")]
    public int MaxMags;

    [Tooltip("How many Pellets per shot")]
    public int PelletsPerShot;

    [Tooltip("How many Shots per Magazine")]
    public int ShotsPerMag;

    [Tooltip("How many Pellets per Magazine\n(used for calculations (also basically just 'max pellets' since most guns just have MaxMags of 1))")]
    public int PelletsPerMag;

    [Header("Pellet Values")]
    [Tooltip("The size of each pellet")]
    public float PelletSize;

    [Tooltip("The damage done from each pellet wihtout any charge")]
    public float MinPelletDamage;
    [Tooltip("The damage done from each pellet fully charged")]
    public float MaxPelletDamage;

    [Tooltip("The speed of each pellet (not velocity)")]
    public float PelletSpeed;

    // [Tooltip("The size of each pellet shot")]
    public float PelletDrop;

    // [Tooltip("The size of each pellet shot")]
    public float DamageDrop;

    [Header("Weapon Values")]
    [Tooltip("String name for the weapon\n(CURRENTLY UNUSED)")]
    public string WeaponName;

    [Tooltip("Boolean to use the Auto Reload functionality")]
    public bool AutoReload;

    [Tooltip("How far Pellets travel before detroying")]
    public float Range;

    [Tooltip("How large the spread of the pellets are. Maximum possible angles for each pellet. Each pellet is assigned a random direction within this spread.")]
    public float Spread;

    [Tooltip("How powerful player recoils when shooting gun without any charge. Maximum Recoil Power if Charge Recoil is active.")]
    public float MinRecoilPower;
    [Tooltip("How powerful player recoils when shooting gun fully charged. Maximum Recoil Power if Charge Recoil is active.")]
    public float MaxRecoilPower;

    [Tooltip("Amount of frames the gun takes between shots.")]
    public float Cocktime;

    [Tooltip("Distance pellets travel from barrel until spread.")]
    public float DelaySpreadRange;

    [Header("Charge Shot Values")]
    [Tooltip("Boolean to affect Recoil from the amount charged\n(Charge Time should be set to zero if no charge at all wanted)")]
    public bool ChargeRecoil;

    [Tooltip("Boolean to affect Damage per Pellet from the amount charged\n(Charge Time should be set to zero if no charge at all wanted)")]
    public bool ChargeDamage;

    [Tooltip("Maximum amount of charge time (seconds)\n(Set this to zero if no charge is wanted)")]
    public float MaxChargeTime;

    [Header("Assigned GameObects")]
    [Tooltip("Transform the weapon's transform is following")]
    public Transform ParentWeapon;

    [Tooltip("GameObject the weapon transform binds to. (eg. the model's hand)")]
    public GameObject WeaponBindLocation;

    [Tooltip(":3")]
    public GameObject Player;
    
    [Tooltip(">:)")]
    public GameObject Gun;
    
    [Tooltip("Empty GameObject used for forward shooting direction and projectile instantiating")]
    public GameObject ForwardBarrel;
    
    [Tooltip("Empty GameObject used for backward shooting direction and projectile instantiating")]
    public GameObject BackwardBarrel;
    
    [Tooltip("pew pew prefab")]
    public GameObject Projectile;
    
    [Tooltip("aim and shoot! UI image sometimes used for raycast for aiming but it keeps changing so idk")]
    public HitMarker Reticle;
    
    [Tooltip("Maywee Pwopens Pweefwab")]
    public GameObject Umbrella;

    [Tooltip("Boom")]
	public AudioClip ShotSound;

    [Tooltip("Layers used for aiming raycast. Deselect ignored layers.")]
    public LayerMask aimLayers;
    [Tooltip("Particle effect for the muzzle flash whenever the gun is fired.")]
    public GameObject muzzleFlash;
    
    [System.NonSerialized]
    public List<Quaternion> PelletList;


    // No Tooltip cause properties don't show up in the inspector but 
    // Handles access for weapon charge and, upon set, fires an event for PlayerCanvas
    // to update the UI accordingly
    public float CurrentCharge
    {
        get
        {
            return currentCharge;
        }
        protected set
        {
            currentCharge = value;
            if (OnChargeChanged != null)
            {
                OnChargeChanged();
            }
        }
    }

    // These delegates are assigned a corresponding function in PlayerCanvas
    // If the event occurs in Weapon, that function in PlayerCanvas will be called
    // i.e. the canvas will be updated based on new values
    public delegate void AmmoChanged();
    public delegate void ChargeChanged();
    public event AmmoChanged OnAmmoChanged;
    public event ChargeChanged OnChargeChanged;
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

    

    protected float tempCooldown, tempRecoil, currentCharge, recoilCharge;
    protected bool activeCooldown, activeRecoil, isShooting;
    protected int isCharging; //0 = false, 1 = forward, 2 = backward
    
    protected int recoilType;//0 = false, 1 = forward, 2 = backward

    [Header("Private Fields (DO NOT CHANGE)")]
    
    [SerializeField]
    [Tooltip("Current total amount of Pellets Left.\n(Protected)")]
    protected int currentTotalPellets;
    
    [SerializeField]
    [Tooltip("Current amount of shots left in magazine.\n(Protected)")]
    protected int currentShotsLeftInMag;
    
    [SerializeField]
    [Tooltip("Current amount of Magazines left. Usually zero because infinite.\n(Protected)")]
    protected float CurrentMagsLeft;

    
    
    protected PlayerController PCtrl;
    protected PlayerInformation PInfo;
    protected Rigidbody PlayerRb;
    protected AudioSource audioSource;
    private CameraController cam;


//---------------------------------------------------------------------MONO METHODS:
    public void Start () {
		PCtrl           = Player.GetComponent<PlayerController>();
        PInfo           = Player.GetComponent<PlayerInformation>();
        PlayerRb        = Player.GetComponent<Rigidbody>();
        tempCooldown    = 0f;
        tempRecoil      = 0f;
        activeCooldown  = false;
        activeRecoil    = false;
        recoilType      = 0;
        ParentWeapon    = this.transform.parent;
        PelletList      = new List<Quaternion>(PelletsPerShot);
        PelletsPerMag   = PelletsPerShot * ShotsPerMag;
        CurrentTotalPellets = PelletsPerMag * MaxMags;
        CurrentShotsLeftInMag = ShotsPerMag;
        audioSource = this.GetComponent<AudioSource>();
        CurrentCharge = 0;
        isCharging = 0;
        cam = PCtrl.CamCon;
        Reticle = PInfo.Reticle;
        //switch(PInfo.PlayerNumber)
        //{
        //    case Enums.Player.Player1:
        //        ShootForwardButton = Axes.Action.ShootForwardPlayerOne;
        //        ShootBackwardButton = Axes.Action.ShootBackwardPlayerOne;
        //        break;
        //    case Enums.Player.Player2:
        //        ShootForwardButton = Axes.Action.ShootForwardPlayerTwo;
        //        ShootBackwardButton = Axes.Action.ShootBackwardPlayerTwo;
        //        break;
        //    case Enums.Player.Player3:
        //        ShootForwardButton = Axes.Action.ShootForwardPlayerThree;
        //        ShootBackwardButton = Axes.Action.ShootBackwardPlayerThree;
        //        break;
        //    case Enums.Player.Player4:
        //        ShootForwardButton = Axes.Action.ShootForwardPlayerFour;
        //        ShootBackwardButton = Axes.Action.ShootBackwardPlayerFour;
        //        break;
        //    default: break;
        //}

        for(int i=0; i<PelletsPerShot; i++)
        {
            PelletList.Add(Quaternion.Euler(Vector3.zero));
        }
        //transform.SetParent(WeaponBindLocation.transform, false);
        transform.position += WeaponBindLocation.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if(PCtrl.CanShoot()){
            CheckShoot();
            AICheckShootWrapper();
            CheckRecoil();
        }
        
        if(activeCooldown)
            PerformCooldown();
        else checkCock();

        CheckReload();

        CurrentMagsLeft = CurrentTotalPellets / PelletsPerMag;
        ShotsPerMag = PelletsPerMag / PelletsPerShot;

        // Weapon follows bind location at player's hand
        transform.position = WeaponBindLocation.transform.position;
        
        // if (Input.anyKey)
        //     DLog("True");
	}

//----------------------------------------------------------------ABSTRACT METHODS:
    protected abstract void Shoot(bool isForward);
    protected abstract void CheckShoot(bool AIL2 = false, bool AIR2 = false);
    protected abstract void CheckRecoil();

//--------------------------------------------------------------METHODS:

    ///<summary>
    ///Checks if the player cocked the gun and cannot shoot to reload
    ///</summary>
    protected void checkCock()
    {
        if(!PCtrl.CanShoot() && currentShotsLeftInMag < ShotsPerMag)
        {
            // DLog("Reload!");
            Cock();
        }
    }

    ///<summary>
    ///Resets cooldown and enables the PlayerController to shoot
    ///</summary>
    protected void Cock()
    {
        tempCooldown = 0;
        
        muzzleFlash.SetActive(false);
    }

    ///<summary>
    ///Disables the player's ability to shoot in the PlayerController
    ///</summary>
    protected void DisableShoot()
    {
        PCtrl.DisableWeapon();
    }

    ///<summary>
    ///Checks in update in player reloads
    ///</summary>
    protected void CheckReload()
    {
        if(currentShotsLeftInMag <= 0)
        {
            if(AutoReload || Input.GetButtonDown(Axes.toStr[ReloadButton]))
            {
                // DLog("Before Reload: " + currentShotsLeftInMag + "/" + ShotsPerMag);
                ReloadByShots();
            }
        }
    }

    ///<summary>
    ///Reload method which calculates the amount of pellets by using the amount of shots needed
    ///</summary>
    public void ReloadByShots()
    {
        DisableShoot();
        StartCooldown();
        CurrentTotalPellets = PelletsPerShot * ShotsPerMag;
        CurrentShotsLeftInMag = ShotsPerMag;
        // DLog("After Reload: " + currentShotsLeftInMag + "/" + ShotsPerMag);
    }

    ///<summary>
    ///Give AI access to CheckShoot
    ///</summary>
    public void AICheckShootWrapper(bool L2 = false, bool R2 = false)
    {   

        if(R2){
            CheckShoot(false, true);
        }
        if(L2){
            CheckShoot(true, false);
        }
    }

    ///<summary>
    ///Starts the cooldown by setting temp cooldown to Cocktime and sets the cooldown bool to true
    ///</summary>
    protected void StartCooldown()
    {
        SetTempCooldown(Cocktime);
        SetCooldown(true);
    }

    ///<summary>
    ///Counts upwards to cooldown limit and sets if cooldown is active until limit met
    ///</summary>
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

    ///<summary>
    ///Activates the recoil
    ///<summary>
    protected void ActivateRecoil(bool isForward)
    {
        activeRecoil = true;
        if(recoilType < 3)
            tempRecoil = MaxRecoilPower * 10f * (isForward ? -1 : 1);
        // DLog("TempRecoil: " + tempRecoil);
    }

    ///<summary>
    ///Deactivates recoil by resetting all recoil and charge values
    ///</summary>
    public void DeactivateRecoil()
    {
        activeRecoil = false;
        tempRecoil = 0f;
        PCtrl.ConstVelocity = Vector3.zero;
        CurrentCharge = 0;
        isCharging = 0;
        DLog("TempRecoil: " + tempRecoil);
        DLog("ConstVelocity: " + PCtrl.ConstVelocity);
        DLog("ActiveRecoil: " + activeRecoil + " tempRecoil: " + tempRecoil);
    }

    //Aims weapon where the reticle is pointing at
    protected void AimWeapon()
    {
        Transform retTransform = cam.transform;
        RaycastHit hit;
        Ray ray = new Ray(retTransform.position, retTransform.forward);
        if(Physics.Raycast(ray, out hit, 1000, aimLayers))
        {
            Vector3 newPlayerRotLoc = hit.point;
            
            Debug.DrawLine(transform.position, newPlayerRotLoc, Color.white, 10);
            
            if(PCtrl.devTool_FreeCamera)
            {
                Player.transform.rotation  = Quaternion.Euler(cam.CamVertAngle,cam.CamHorAngle -180,0);
            }
            else
               Player.transform.LookAt(newPlayerRotLoc);

           ForwardBarrel.transform.LookAt(newPlayerRotLoc);
            //ParentWeapon.transform.rotation = Quaternion.Euler(newPlayerRotLoc)*Player.transform.rotation;
            DLog("Aiming at: " + hit.transform.gameObject.name);
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

public void tempshoot(bool direction){
    Shoot(direction);
  }
   

    public void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}
