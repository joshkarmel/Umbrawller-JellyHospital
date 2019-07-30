using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PumpShotgun : Weapon 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "PumpShotgun";

//---------------------------------------------------------------------------FIELDS:

  [Header("Special Values")]
  [Tooltip("Damage per Second for Status Effect")]
  public float statusDamage;
  [Tooltip("Cooldown for Status Effect")]
  public float statusCooldown;

  [Tooltip("How much the player slows when hit with Tesla effect (multiplier)")]
  public float teslaSlowdown;

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
    var barrel = isForward ? ForwardBarrel : BackwardBarrel;
    var barrelRot = barrel.transform.rotation;
    var barrelPos = barrel.transform.position;

    foreach(Quaternion quat in PelletList.ToArray())
    {
      if(CurrentTotalPellets > 0 && currentShotsLeftInMag > 0){
        
        PelletList[i] = Random.rotation;
        var bullet1 = (GameObject)Instantiate(Projectile, barrelPos, barrelRot);
        bullet1.GetComponent<Projectile>().SetTeam(PInfo.TeamNumber);
        bullet1.GetComponent<Projectile>().SetBulletPlayerID(PInfo.PlayerNumber);
        bullet1.GetComponent<Projectile>().SetHitMarker(PInfo.Reticle);
        var newPelletDamage = ChargeDamage ? (MaxPelletDamage * CurrentCharge + MinPelletDamage) : MaxPelletDamage;
        bullet1.GetComponent<Projectile>().SetDamage(newPelletDamage); //if charge damage, multiply by current charge
        bullet1.GetComponent<Projectile>().SetRange(Range);
        if(AmmoType != StatusType.NORMAL)
        {
          bullet1.GetComponent<FirePellet>().statCooldown = statusCooldown;
          bullet1.GetComponent<FirePellet>().DPS = statusDamage;
          if(AmmoType == StatusType.ELECTRIC && bullet1.GetComponent<TeslaPellet>())
            bullet1.GetComponent<TeslaPellet>().slowdown = teslaSlowdown;
        }
        bullet1.transform.localScale = new Vector3(PelletSize, PelletSize, PelletSize);

        bullet1.transform.rotation = Quaternion.RotateTowards(bullet1.transform.rotation, PelletList[i], Spread);
        bullet1.GetComponent<Projectile>().SetDelayedRange(DelaySpreadRange);
        bullet1.GetComponent<Projectile>().SetDelayedVel(bullet1.transform.forward*PelletSpeed);

        bullet1.GetComponent<Rigidbody>().velocity = (barrel.transform.forward * PelletSpeed);//(bullet1.transform.forward*PelletSpeed);
        
        i++;
        Destroy(bullet1, PelletDrop);
        CurrentTotalPellets--;
      }
    }
    muzzleFlash.SetActive(true);
    PCtrl.Animator.SetBool("ShootBack", false);
    audioSource.clip = ShotSound;
    recoilCharge = CurrentCharge;
    CurrentCharge = 0;
    audioSource.Play();
    StartCooldown();
    CurrentShotsLeftInMag--;
    recoilType = isForward ? 1 : 2;
    ActivateRecoil(isForward);
    //AimWeapon();
  }

  //checks if the player shoots forward or backward
  //calls Shoot() and recoils depending on shot direction
  protected override void CheckShoot(bool AIL2 = false, bool AIR2 = false)
  {
    if(currentShotsLeftInMag > 0 && CurrentTotalPellets > 0){
      if ((Input.GetAxis(Axes.toStr[ShootForwardButton])!= 0 || AIR2) && !activeCooldown)
      {
        recoilType = 1;
        isCharging = 1;
        CurrentCharge += (Time.deltaTime / MaxChargeTime);
        if(CurrentCharge >= 1)
        {
          Shoot(true);
        }
      }
      else if((Input.GetAxis(Axes.toStr[ShootBackwardButton]) != 0 || AIL2)  && !activeCooldown)
      {
        PCtrl.Animator.SetBool("ShootBack", true);
        recoilType = 2;
        isCharging = 2;
        CurrentCharge += (Time.deltaTime / MaxChargeTime);
        if(CurrentCharge >= 1)
        {
          Shoot(false);
        }
      }
      else if(isCharging > 0 && !activeCooldown)
      {
        if(isCharging == 1)
          Shoot(true);
        if(isCharging == 2)
          Shoot(false);
        CurrentCharge = 0;
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
    float chargeMultiplier = recoilCharge / MaxChargeTime + MinRecoilPower/10;
    if(chargeMultiplier > MaxRecoilPower/10){
      chargeMultiplier = MaxRecoilPower/10;
    }
    Vector3 recoilForce;


    if(activeRecoil && tempRecoil != 0 && !PCtrl.IsGrounded())
    {
      recoilForce = ForwardBarrel.transform.forward * tempRecoil * (ChargeRecoil ? chargeMultiplier: recoilCharge);
      PCtrl.ConstVelocity = recoilForce;
      PlayerRb.velocity = recoilForce;

      if(recoilType == 1)
        tempRecoil += 10;
      if(recoilType == 2)
        tempRecoil -= 10;
      
      // DLog("TempRecoil: " + tempRecoil);
      // DLog("ConstVelocity: " + PCtrl.ConstVelocity);
      // DLog("ActiveRecoil: " + activeRecoil + " recoilForce " + recoilForce);
      // DLog("recoilCharge: " + recoilCharge + " MaxChargeTime: " + MaxChargeTime);
      DLog("Charge Multiplier: " + chargeMultiplier);
      DLog("Charge Mult w/o min: " + (chargeMultiplier - MinRecoilPower/10));
    }
    if((tempRecoil >= 0.5f && recoilType == 1) || 
        (tempRecoil <= -0.5f && recoilType == 2) ||
        (PCtrl.IsGrounded() && activeRecoil)) 
      DeactivateRecoil();
  }
  

}