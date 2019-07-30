using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PumpShotgun_Net : Weapon_Net 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "PumpShotgun_Net";

//---------------------------------------------------------------------------FIELDS:
//---------------------------------------------------------------------MONO METHODS:

  new void Start() 
  {
    base.Start();	
    PelletsPerMag = 24;
  }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
  protected override void Shoot(bool isForward)
  {
    DLog("Shoot!");
    AimWeapon();
	int i = 0;
    

    foreach(Quaternion quat in PelletList)
    {
      if(CurrentTotalPellets > 0 && currentShotsLeftInMag > 0){
			CmdSpawnPellets(isForward, i);
      }
    }

    audioSource.clip = ShotSound;
    audioSource.Play();
    StartCooldown();
    CurrentShotsLeftInMag--;
    recoilType = isForward ? 1 : 2;
    ActivateRecoil(isForward);
  }

  //checks if the player shoots forward or backward
  //calls Shoot() and recoils depending on shot direction
  protected override void CheckShoot()
  {
    if(currentShotsLeftInMag > 0 && CurrentTotalPellets > 0){
      if (Input.GetAxis(Axes.toStr[ShootForwardButton]) != 0 && !activeCooldown)
      {
        recoilType = 1;
        isCharging = 1;
        currentCharge += Time.deltaTime;
        if(currentCharge >= MaxChargeTime)
        {
          Shoot(true);
          currentCharge = 0;
          isCharging = 0;
        }
      }
      else if(Input.GetAxis(Axes.toStr[ShootBackwardButton]) != 0  && !activeCooldown)
      {
        recoilType = 2;
        isCharging = 2;
        currentCharge += Time.deltaTime;
        if(currentCharge >= MaxChargeTime)
        {
          Shoot(false);
          currentCharge = 0;
          isCharging = 0;
        }
      }
      else if(isCharging > 0 && !activeCooldown)
      {
        if(isCharging == 1)
          Shoot(true);
        if(isCharging == 2)
          Shoot(false);
        currentCharge = 0;
        isCharging = 0;
      }
    }
    else
    {
      DisableShoot();
    }
  }

  //checks if recoil is active every frame, performs recoil
  protected override void CheckRecoil()
  {
    float chargeMultiplier = currentCharge / MaxChargeTime;
    Vector3 recoilForce;
    switch(recoilType){
        case 1:
            if(activeRecoil && tempRecoil != 0 && !PCtrl.IsGrounded())
            {
              recoilForce = transform.up * tempRecoil * (ChargeRecoil ? chargeMultiplier : 1);
              PCtrl.ConstVelocity = recoilForce;
              PlayerRb.velocity = recoilForce;
              tempRecoil += 10;
              DLog("TempRecoil: " + tempRecoil);
              DLog("ConstVelocity: " + PCtrl.ConstVelocity);
            }
            if(tempRecoil >= 0.5f){
              PCtrl.ConstVelocity = Vector3.zero;
              tempRecoil = 0f;
              activeRecoil = false;
            }
            if(PCtrl.IsGrounded() && activeRecoil){
              activeRecoil = false;
              tempRecoil = 0f;
              PCtrl.ConstVelocity = Vector3.zero;
            }
            break;
        case 2:
            if(activeRecoil && tempRecoil != 0 && !PCtrl.IsGrounded())
            {
              recoilForce = ParentWeapon.forward * tempRecoil * (ChargeRecoil ? chargeMultiplier : 1);
              PCtrl.ConstVelocity = recoilForce;
              PlayerRb.velocity = recoilForce;
              tempRecoil -= 10;
              DLog("TempRecoil: " + tempRecoil);
              DLog("ConstVelocity: " + PCtrl.ConstVelocity);
            }
            if(tempRecoil <= -0.5f){
              PCtrl.ConstVelocity = Vector3.zero;
              tempRecoil = 0f;
              activeRecoil = false;
            }
            if(PCtrl.IsGrounded() && activeRecoil){
              activeRecoil = false;
              tempRecoil = 0f;
              PCtrl.ConstVelocity = Vector3.zero;
            }
            break;
        default:
            break;
    }
  }

  //--------------------------------------------------------------------------HELPERS:

	[Command]
	void CmdSpawnPellets(bool isForward, int i)
	{
		var barrel = isForward ? ForwardBarrel : BackwardBarrel;
    	var barrelRot = barrel.transform.rotation;
    	var barrelPos = barrel.transform.position;

		DLog("Command shoot");
		PelletList[i] = Random.rotation;
		var bullet1 = (GameObject)Instantiate(Projectile, barrelPos, barrelRot);
		bullet1.GetComponent<Projectile_Net>().SetTeam(PInfo.TeamNumber);
		var newPelletDamage = ChargeDamage ? (PelletDamage * (currentCharge / MaxChargeTime)) : PelletDamage;
		bullet1.GetComponent<Projectile_Net>().SetDamage(newPelletDamage); //if charge damage, multiply by current charge
		bullet1.GetComponent<Projectile_Net>().SetRange(Range);
		bullet1.transform.localScale = new Vector3(PelletSize, PelletSize, PelletSize);

		bullet1.transform.rotation = Quaternion.RotateTowards(bullet1.transform.rotation, PelletList[i], Spread);
		bullet1.GetComponent<Projectile_Net>().SetDelayedRange(DelayRange);
		bullet1.GetComponent<Projectile_Net>().SetDelayedVel(bullet1.transform.forward*PelletSpeed);
		
		bullet1.GetComponent<Rigidbody>().velocity = (barrel.transform.forward * PelletSpeed);//(bullet1.transform.forward*PelletSpeed);

		i++;
		//Destroy(bullet1, PelletDrop);
		CurrentTotalPellets--;

		NetworkServer.Spawn(bullet1);
	}
}