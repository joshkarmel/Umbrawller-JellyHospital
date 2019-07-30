using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGun : Weapon 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "Rail Gun";

//---------------------------------------------------------------------------FIELDS:

  [Header("Rail Gun Values")]
  public int hitTimes;

//---------------------------------------------------------------------MONO METHODS:

  new void Start() 
  {
    base.Start();
    PelletsPerMag = 24;
  }

//--------------------------------------------------------------------------METHODS:
  protected override void Shoot(bool isForward)
  {
    DLog("Shoot!");
    AimWeapon();
    var barrel = isForward ? ForwardBarrel : BackwardBarrel;
    var barTrans = barrel.transform;
    var barrelRot = barrel.transform.rotation;
    var barrelPos = barrel.transform.position;

    hitTimes = Mathf.RoundToInt(MaxPelletDamage / DamageDrop);
    initiateRaycast(transform, hitTimes, Mathf.RoundToInt(MaxPelletDamage));

    audioSource.clip = ShotSound;
    recoilCharge = CurrentCharge;
    CurrentCharge = 0;
    audioSource.Play();
    StartCooldown();
    CurrentShotsLeftInMag--;
    recoilType = isForward ? 1 : 2;
    ActivateRecoil(isForward);
    AimWeapon();
  }

  //checks if recoil is active every frame, performs recoil
  protected override void CheckRecoil()
  {
    float chargeMultiplier = recoilCharge / MaxChargeTime;
    Vector3 recoilForce;

    if(activeRecoil && tempRecoil != 0 && !PCtrl.IsGrounded())
    {
      recoilForce = ForwardBarrel.transform.forward * tempRecoil * (ChargeRecoil ? recoilCharge: 1);
      PCtrl.ConstVelocity = recoilForce;
      PlayerRb.velocity = recoilForce;

      if(recoilType == 1)
        tempRecoil += 10;
      if(recoilType == 2)
        tempRecoil -= 10;
      
      DLog("TempRecoil: " + tempRecoil);
      DLog("ConstVelocity: " + PCtrl.ConstVelocity);
      DLog("ActiveRecoil: " + activeRecoil + " recoilForce " + recoilForce);
      DLog("recoilCharge: " + recoilCharge + " MaxChargeTime: " + MaxChargeTime);
    }
    if((tempRecoil >= 0.5f && recoilType == 1) || 
        (tempRecoil <= -0.5f && recoilType == 2) ||
        (PCtrl.IsGrounded() && activeRecoil)) 
      DeactivateRecoil();
  }
  
  //checks if the player shoots forward or backward
  //calls Shoot() and recoils depending on shot direction
  protected override void CheckShoot(bool AIL2 = false, bool AIR2 = false)
  {
    if(currentShotsLeftInMag > 0 && CurrentTotalPellets > 0){
      if (Input.GetAxis(Axes.toStr[ShootForwardButton]) != 0 && !activeCooldown)
      {
        recoilType = 1;
        isCharging = 1;
        CurrentCharge += (Time.deltaTime / MaxChargeTime);
        if(CurrentCharge >= MaxChargeTime)
        {
          Shoot(true);
        }
      }
      else if(Input.GetAxis(Axes.toStr[ShootBackwardButton]) != 0  && !activeCooldown)
      {
        recoilType = 2;
        isCharging = 2;
        CurrentCharge += (Time.deltaTime / MaxChargeTime);
        if(CurrentCharge >= MaxChargeTime)
        {
          Shoot(false);
        }
      }
      // else if(isCharging > 0 && !activeCooldown)
      // {
      //   if(isCharging == 1)
      //     Shoot(true);
      //   if(isCharging == 2)
      //     Shoot(false);
      //   CurrentCharge = 0;
      //   isCharging = 0;
      // }
    }
    else
    {
      DisableShoot();
    }
  }
//--------------------------------------------------------------------------HELPERS:

  void initiateRaycast(Transform trans, int times, int damage)
  {
    GameObject go = new GameObject();
    Transform t = go.transform;
    t.rotation = trans.rotation;
    

    foreach(Quaternion quat in PelletList)
    {
      if(CurrentTotalPellets > 0 && currentShotsLeftInMag > 0)
      {
        RaycastHit hit;
        Ray ray = new Ray(trans.position, trans.forward);
        if(Physics.SphereCast(ray, 0.1f, out hit, Range, aimLayers))
        {
          Vector3 hitLoc = hit.point;
          t.position = hit.point;

          Color[] hitColors = {Color.red, Color.blue, Color.green, Color.magenta, Color.cyan, Color.yellow, Color.white, Color.red, Color.blue, Color.green, Color.magenta, Color.cyan};
          
          DLog("hit times: " + times);
          Debug.DrawLine(trans.position, hitLoc, hitColors[times], 10);

          if(hit.transform.gameObject.tag == "Player")
          {
              PlayerInformation HitPInfo = hit.transform.gameObject.GetComponent<PlayerInformation>();
              DLog("Player Hit Team: " + HitPInfo.TeamNumber);
              if(HitPInfo.TeamNumber != PInfo.TeamNumber)
              {
                  HitPInfo.ChangeHealth(-damage);
                  DLog("Player Hit Health: " + HitPInfo.GetHealth());
                  if(HitPInfo.GetHealth() <= 0 )
                  {
                    HitPInfo.KilledByTeam = PInfo.TeamNumber;
                    DLog(PInfo.TeamNumber + " killed " + HitPInfo.TeamNumber);
                    if(GameManager.Instance.gameModeTracker != null)
                      GameManager.Instance.gameModeTracker.AddScore();
                  }
              }
          }
          else if(hit.transform.gameObject.tag == "umbrella")
          {
              UmbrellaBase umbrella = hit.transform.gameObject.GetComponentInParent<UmbrellaBase>();
              PlayerInformation HitPInfo = hit.transform.gameObject.GetComponent<PlayerInformation>();
              if(HitPInfo.TeamNumber != PInfo.TeamNumber)
              {
                  umbrella.ModifyUmbrellaHP(-damage);
              }
          }

          if(VERBOSE)
          {
            GameObject bullet1;
            bullet1 = (GameObject)Instantiate(Projectile, t.position, t.rotation);
            Renderer rend = bullet1.GetComponent<Renderer>();
            rend.material.color = hitColors[times];
            DLog("hit times:" + times);
            Destroy(bullet1, 10);
          }

          if(damage > 0)
          {
            initiateRaycast(t, times-1, damage - Mathf.RoundToInt(DamageDrop));
          }
        }
      }
    }
  }


  // public float Damage(PlayerInformation pInfo)
  // {
  //     if (pInfo.isDamageBoostActive())
  //         return PelletDamage * damageBoostMultiplier;
  //     else
  //         return PelletDamage;
  // }
  
  

}