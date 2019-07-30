using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility; //DON'T FORGET THIS

public class DragonBreathGun : GasGun
{
//------------------------------------------------------------------------CONSTANTS:

//---------------------------------------------------------------------------FIELDS:
	
//---------------------------------------------------------------------MONO METHODS:
  new void Start() 
  {
    base.Start();
    ShotsPerMag = 100;
    PelletsPerMag = 300;
    gasCones = new List<GameObject>(100);
  }

//--------------------------------------------------------------------------METHODS:
    protected override void Shoot(bool isForward)
    {
        AimWeapon();
        if(gasCone == null)
        {
            var barrel = isForward ? ForwardBarrel : BackwardBarrel;
            var barrelRot = barrel.transform.rotation;
            var barrelPos = barrel.transform.position;

            gasCone = (GameObject)Instantiate(Projectile, barrelPos, barrelRot, (isForward ? ForwardBarrel.transform : BackwardBarrel.transform));
            gasCone.transform.localScale = new Vector3(3, 3, 3);
            gasCone.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));

            Projectile gasChild = gasCone.transform.GetChild(0).GetComponent<Projectile>();
            gasChild.SetTeam(PInfo.TeamNumber);
            gasChild.GetComponent<Projectile>().SetDamage(MaxPelletDamage);
            gasChild.GetComponent<Projectile>().SetRange(Range);

            recoilType  = isForward ? 1 : 2;
            ActivateRecoil(isForward);
            gasCones.Add(gasCone);
        }
        
        //recoil
        if(!PCtrl.IsGrounded()){
            var recoilForce = ForwardBarrel.transform.forward * (isForward ? -10 : 10);
            PCtrl.ConstVelocity = recoilForce;
            PlayerRb.velocity = recoilForce;
        }
    }
    
    //checks if the player shoots forward or backward
    //calls Shoot() and recoils depending on shot direction
    protected override void CheckShoot(bool AIL2 = false, bool AIR2 = false)
    { 
        base.CheckShoot();
        
        // for flamethrower
        if(Input.GetAxis(Axes.toStr[ShootForwardButton]) == 0 && Input.GetAxis(Axes.toStr[ShootBackwardButton]) == 0)
        {
            if(gasCone != null)
            {
                Destroy(gasCone);
                DeactivateRecoil();
            }
        }

    }

    //checks if recoil is active every frame, performs recoil
    protected override void CheckRecoil()
    {
        float chargeMultiplier = recoilCharge / MaxChargeTime;
        Vector3 recoilForce;

        if(activeRecoil && tempRecoil != 0 && !PCtrl.IsGrounded())
        {
            recoilForce = ForwardBarrel.transform.forward;
            switch(recoilType){
                case 1: 
                    recoilForce *= -10;
                    break;
                case 2:
                    recoilForce *= 10;
                    break;
                default:
                    break;
            }
            PCtrl.ConstVelocity = recoilForce;
            PlayerRb.velocity = recoilForce;
            DLog(recoilForce + "");
        }
        if((PCtrl.IsGrounded() && activeRecoil)) 
            DeactivateRecoil();
    }

//--------------------------------------------------------------------------HELPERS:
}