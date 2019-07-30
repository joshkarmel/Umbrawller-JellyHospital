using UnityEngine;
using System.Collections;
using MyUtility; //DON'T FORGET THIS
using UnityEngine.Networking;

public class Ultimate_Net : NetworkBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:

    public float UltCharge
    {
        get
        {
            return ultCharge;
        }
        set
        {
            ultCharge = value;
            if(OnChargeChanged != null)
            {
                OnChargeChanged(value);
            }
        }
    }

    public float
        ChargeSpeed,
        MaxActiveTime,
        MaxCharge;

    public delegate void ChargeChanged(float value);
    public event ChargeChanged OnChargeChanged;

    public GameObject
        UltWeapon,
        GunWeapon,
        PCtrl;
    public Axes.Action UltButton;

    //---------------------------------------------------------------------------PRIVATE FIELDS:
    private float ultCharge;
    private float chargeTimer;
    private float activeTimer;

    [SerializeField]
    private bool isCharged;

    //---------------------------------------------------------------------MONO METHODS:

    void Start() 
	{
		UltCharge = 0;
        // MaxCharge = 100;
        chargeTimer = 0;
        // ChargeSpeed = 120;
        isCharged = false;
        activeTimer = 0;
	}
		
	void Update()
    {
        // if(!isCharged)
        if(!isCharged)
		    IncrementCharge();
        CheckCharge();

        if(activeTimer > 0 && isCharged)
        {
            activeTimer -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            if(UltCharge < MaxCharge)
                UltCharge = MaxCharge;
            if(UltWeapon.activeSelf && activeTimer > 0)
                activeTimer = 0;
        }
    }

//--------------------------------------------------------------------------METHODS:

    public void CheckCharge()
    {
        if(UltCharge >= MaxCharge && !isCharged)
        // if(UltCharge > 3 && !isCharged) //for debugging
        {
            if(Input.GetButton(Axes.toStr[UltButton]))
                ActivateUlt();
        }
        if(UltWeapon.activeSelf && activeTimer <= 0)
        {
            DeactivateUlt();
        }
    }

    public void ActivateUlt()
    {
        GunWeapon.SetActive(false);
        UltWeapon.SetActive(true);
        PCtrl.GetComponent<PlayerController_Net>().PlayerWeaponNet = UltWeapon.GetComponent<Weapon_Net>();
        DLog("Ult Activated");
        isCharged = true;
        activeTimer = MaxActiveTime;
    }

    public void DeactivateUlt()
    {
        GunWeapon.SetActive(true);
        UltWeapon.GetComponent<GasGun>().DestroyGas();
        UltWeapon.SetActive(false);
        PCtrl.GetComponent<PlayerController_Net>().PlayerWeaponNet = GunWeapon.GetComponent<Weapon_Net>();
        UltCharge = 0;
        chargeTimer = 0;
        isCharged = false;
        DLog("Ult Deactivated " + UltWeapon.GetComponent<Weapon_Net>().GetCurrentShotsLeftInMag());
    }

    public void IncrementCharge()
    {
        if(chargeTimer >= ChargeSpeed && UltCharge < MaxCharge)
        {
            UltCharge++;
            chargeTimer = 0;
        }
        else if(chargeTimer < ChargeSpeed)
        {
            chargeTimer += Time.deltaTime;
        }
    }

    public float getUltCharge()
    {
        return UltCharge;
    }

    public float getMaxCharge()
    {
        return MaxCharge;
    }

    public void setUltCharge(float x)
    {
        UltCharge = x;
    }

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}