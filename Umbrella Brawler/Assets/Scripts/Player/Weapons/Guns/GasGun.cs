using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyUtility; //DON'T FORGET THIS

public class GasGun : Weapon
{
//------------------------------------------------------------------------CONSTANTS:

//---------------------------------------------------------------------------FIELDS:

    [Header("Gas Gun Values")]
    [Tooltip("Constant recoil speed")]
    public float recoilSpeed;

    protected GameObject gasCone;
    protected List<GameObject> gasCones;
    public GameObject GasConeParent;
	
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
        // if(gasCone == null)
        // {
            var barrel = isForward ? ForwardBarrel : BackwardBarrel;
            var barrelRot = barrel.transform.rotation;
            var barrelPos = barrel.transform.position;

            gasCone = (GameObject)Instantiate(Projectile, barrelPos, barrelRot, GasConeParent.transform);//[[for flamethrower]], transform);
            // gasCone.transform.localScale = new Vector3(3, 3, 3);
            // gasCone.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            gasCone.transform.Rotate(new Vector3(0,180,0), Space.Self);

            Projectile gasChild = gasCone.transform.GetChild(0).GetComponent<Projectile>();
            gasChild.SetTeam(PInfo.TeamNumber);
            gasChild.GetComponent<Projectile>().SetDamage(MaxPelletDamage);
            gasChild.GetComponent<Projectile>().SetRange(Range);
            // gasCone.transform.rotation = transform.rotation;

            recoilType  = isForward ? 1 : 2;
            ActivateRecoil(isForward);
            gasCones.Add(gasCone);
        // }
        // CurrentShotsLeftInMag--;
    }
    
    //checks if the player shoots forward or backward
    //calls Shoot() and recoils depending on shot direction
    protected override void CheckShoot(bool AIL2 = false, bool AIR2 = false)
    { 
        // DLog(Input.GetAxis(Axes.toStr[ShootForwardButton])+"");
        if(Input.GetAxis(Axes.toStr[ShootForwardButton]) != 0)
        {
            Shoot(true);
        }
        else if (Input.GetAxis(Axes.toStr[ShootBackwardButton]) != 0)
        {
            Shoot(false);
        }
        else{
            DeactivateRecoil();
        }

    }

    //checks if recoil is active every frame, performs recoil
  protected override void CheckRecoil()
  {
    float chargeMultiplier = recoilCharge / MaxChargeTime;

    if(activeRecoil && tempRecoil != 0 && !PCtrl.IsGrounded())
    {
        var recoilForce = ForwardBarrel.transform.forward * (recoilType == 1 ? -10 : recoilType == 2 ? 10 : 0f);
        PCtrl.ConstVelocity = recoilForce;
        PlayerRb.velocity = recoilForce;
    }
    if((tempRecoil >= 0.5f && recoilType == 1) || 
        (tempRecoil <= -0.5f && recoilType == 2) ||
        (PCtrl.IsGrounded() && activeRecoil)) 
      DeactivateRecoil();
  }

    public void DestroyGas()
    {
        if(gasCones != null)
        {
            // Destroy(GasConeParent);
            DeactivateRecoil();
        }
    }

//--------------------------------------------------------------------------HELPERS:
}