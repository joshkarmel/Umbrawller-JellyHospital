using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public class UltimateBase : Weapon {
//------------------------------------------------------------------------CONSTANTS:
	private const string LOG_TAG = "Ultimate";
//---------------------------------------------------------------------------FIELDS:
	public float MaxAmmo,
				AmmoPerMagazine,
				UltActivationCooldown;
	public bool HeldDownButtonActivation;
//-------------------------------------------------------------------PRIVATE FIELDS:
	protected float CurrentAmmo,
				  UltActivationTimer;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		SetTempCooldown(0f);
        SetTempRecoil(0f);
        SetCooldown(false);
        SetActiveRecoil(false);

		PCtrl           = Player.GetComponent<PlayerController>();
        PlayerRb        = Player.GetComponent<Rigidbody>();
        recoilType      = 0;
        ParentWeapon    = this.transform.parent;
		PelletList		= new List<Quaternion>(PelletsPerShot);

		UltActivationTimer = 0f;
		CurrentAmmo = AmmoPerMagazine;
	}
		
	void Update()
    {
		UltCooldownState();
		CheckShoot();
        checkCock();
		CheckRecoil();
		if(GetCooldown()){
            PerformCooldown();
        }
    }

//-----------------------------------------------------------------ABSTRACT METHODS:
	protected override void Shoot(bool isForward)
	{
		return;
	}

	protected override void CheckShoot(bool AIL2 = false, bool AIR2 = false)
	{
		if(currentShotsLeftInMag > 0 && CurrentTotalPellets > 0){
			if (Input.GetAxis(Axes.toStr[ShootForwardButton]) != 0 && !activeCooldown)
			{
				recoilType = 1;
				Shoot(true);
			}
			if(Input.GetAxis(Axes.toStr[ShootBackwardButton]) != 0  && !activeCooldown)
			{
				recoilType = 2;
				Shoot(false);
			}
		}
		else
		{
			DisableShoot();
		}
	}

	protected override void CheckRecoil()
	{
		
	}
//--------------------------------------------------------------------------METHODS:

	protected void UltCooldownState()
	{
		if (UltActivationTimer <= 0)
		{
			PCtrl.canUseUlt = true;
			UltActivationTimer = 0;
		}
		else
		{
			UltActivationTimer -= Time.deltaTime;
		}
	}

	

//--------------------------------------------------------------------------HELPERS:

}