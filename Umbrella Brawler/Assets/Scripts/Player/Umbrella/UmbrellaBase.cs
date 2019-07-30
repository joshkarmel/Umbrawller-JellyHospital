using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
public class UmbrellaBase : MonoBehaviour {

//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "UmbrellaBaseClass";
	public bool VERBOSE = false;

    //---------------------------------------------------------------------------FIELDS:


	public float MaxHitPoints;
	public float ShieldCooldown = 3f;
	public float ShieldActiveTime = 1f;
	public float Weight;
	public bool IsUmbrellaOpen;
	public bool CanActivateShield;
	public bool IsShielding;
	public float HitPoints
    {
        get
        {
            return hitPoints;
        }
        set
        {
            hitPoints = value;
            if (OnHPChanged != null)
            {
                OnHPChanged();
            }
        }
    }
	public float ShieldCDTimer;
	public MeshRenderer UmbrellaOpen;
	public MeshRenderer UmbrellaClosed;

	public Collider UmbrellaCollider;
	private float ShieldActiveTimer;
    public float hitPoints;

	public PlayerController playerController;
	public GameObject UmbrellaBindLoc;
	public GameObject ForceFieldEffect;
	public float ffTimeLength;
	private float ffTimer;

    public delegate void HPChanged();
    public event HPChanged OnHPChanged;
    //---------------------------------------------------------------------MONO METHODS:

    void Start () 
	{
		HitPoints = MaxHitPoints;
		ShieldCooldown = 3f;
		ShieldActiveTime = 1f;
		ShieldCDTimer = 0f;
		ShieldActiveTimer = 0f;
		CanActivateShield = true;
		IsShielding = false;

		transform.SetParent(UmbrellaBindLoc.transform, false);
	}
		
	void Update () 
	{
		//Refer to the 'Helpers' section below for these methods
		ShieldCooldownState();
		ShieldActivationState();

		if (IsShielding)
		{
			ffTimer -= Time.deltaTime;
			if (ffTimer <= 0)
			{
				ForceFieldEffect.SetActive(false);
			}
		}
		else
		{
			ForceFieldEffect.SetActive(false);
			ffTimer = 0;
		}
	}
//--------------------------------------------------------------------------METHODS:

	public void OpenUmbrella()
	{
		IsUmbrellaOpen = true;
		UmbrellaOpen.enabled = true;
		UmbrellaClosed.enabled = false;
		UmbrellaCollider.enabled = true;
	}

	public void CloseUmbrella()
	{
		IsUmbrellaOpen = false;
		UmbrellaOpen.enabled = false;
		UmbrellaClosed.enabled = true;
		UmbrellaCollider.enabled = false;
	}

	public float getCurrentHP()
	{
		return HitPoints;
	}

	// Call this method with a positive or negative float as a parameter
	// to change the Umbrella's current HP. If it goes below or above the
	// set thresholds (0 and Max HP), it will be set to those threshold
	// values instead.
	public void ModifyUmbrellaHP(float HpChange)
	{
		float newHP = HitPoints + HpChange;
		if (newHP < 0)
		{
			HitPoints = 0;
			CloseUmbrella();
			playerController.CanUseUmbrella = false;
		}
		else if (newHP > MaxHitPoints)
			HitPoints = MaxHitPoints;
		else
			HitPoints = newHP;
	}

	// Activates the umbrella's shield state and starts the active timer
	// depending on the umbrella's ShieldActiveTime property.
	public void ActivateShield()
	{
		IsShielding = true;
		CanActivateShield = false;
		ShieldActiveTimer = ShieldActiveTime;
		transform.Rotate(0f, 0f, 90f);
	}

	private void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.tag == "projectile" && IsShielding == true)
		{
			ForceFieldEffect.SetActive(true);
			ffTimer = ffTimeLength;
		}
	}

//--------------------------------------------------------------------------HELPERS:
	// Controls the timer that enables you to activate the shield.
	// Once the timer reaches 0, the umbrella shield can be activated again.
	private void ShieldCooldownState()
	{
		if (ShieldCDTimer <= 0)
		{
			if(!CanActivateShield && !IsShielding)
			{
				CanActivateShield = true;
			}
			ShieldCDTimer = 0;
		}
		else
			ShieldCDTimer -= Time.deltaTime;
	}

	// Controls the timer that controls how long the shield is active for.
	// Once the timer reaches 0, the shield is deactivated and activation
	// cooldown timer is started.
	private void ShieldActivationState()
	{
		if (ShieldActiveTimer <= 0)
		{
			if (IsShielding)
			{
				IsShielding = false;
				transform.Rotate(0f, 0f, -90f);
				ShieldCDTimer = ShieldCooldown;
			}
			ShieldActiveTimer = 0;
		}
		else
			ShieldActiveTimer -= Time.deltaTime;
	}

}
