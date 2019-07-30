using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(AudioSource))]
public class PlayerController_Net : NetworkBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "PlayerController_Net";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public Camera Cam;
	public Umbrella_Net UmbrellaNet;
    public Ultimate_Net Ultimate_Net;
	public CameraController CamCon;

	public Weapon_Net PlayerWeaponNet;
	public Axes.Action Horizontal, Vertical, JumpButton, UmbrellaButton, ShieldButton, StartButton;
    public float GroundSpeed; 
	public bool CanMove, Diving, IsDead, IsSpawning, IsFrozen, IsUltActive, canUseUlt;
	public bool CanUseUmbrella = true, CanJump = true;
	

	public float
		GroundedCharacterSpeed	= 0,
		AimingSpeed				= 0,
		JumpSpeed				= 0,
		FallMultiplier			= 0,
		MaxFallSpeed			= 0,
		MaxDiveSpeed			= 0,
		AirMovementSpeed		= 0,
		UmbrellaGravity		 	= 0,
		UmbrellaFallingSpeed	= 0,
		SpeedDecay				= 0,
		ExtraDivingSpeed		= 0,
		SpeedDecayTimer			= 0,
		Gravity					= 0,
		UmbrellaMaxHealth		= 0,
		FallDamageSpeedThreshold=0,
		RespawnTime;

	[Space(10)]
	public float MaxCameraDistance;
	
	public Vector3
		PlayerMove		=	Vector3.zero,
		NetForce		=	Vector3.zero,//velocity added for one frame only
		ConstVelocity	=	Vector3.zero,
		RigidBodyValue  =	Vector3.zero;

    // GameManager.Instance.Players[(int)PlayerNumber] gets you access 
    // to all of this player's selections from the Main Menu, e.g. Team

	public Animator Animator;

    public Vector3 velocity
	{
		get
		{
			return rb.velocity;
		}
	}

    [Space(10)]
    public AudioSource AudioSource;
    public AudioClip DiveClip;
    //public AudioSource diveSource;
    public AudioClip PlayerGettingHit;
	public AudioClip[] RunningSound;

	[Space(10)] [Range(0, 1)]
	public float SmoothCameraTransition;
	
	//Particle Section
	[Space(10)]
	public GameObject DivingParticles;

	//Particle Section End
	
	public bool CanUseWeapon {get; set;}


    //-------------------------------------------------------------------PRIVATE FIELDS:
    
	private Rigidbody rb;
	private Collider col;
	private bool canDive, callOnceDive = false, callOnceOutDive = true;
	private float 
		resetAirSpeed,
		umbrellaSpeedDecayTimer, 
		initialCamDis,
		tempRespawn,
		tempMaxUmbrellaHP;

	private GameObject model;
	private StandaloneInputModule inputModule;
	private PlayerManager playerManager;
	private PlayerInformation_Net playerInformationNet;

	[SerializeField]
	private bool devTool_AllowCameraAdjustment;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
        //If this is not the player, destroy it
		if(!isLocalPlayer)
		{
			//Then remove script
			Destroy(this);
			return;
		}

		playerInformationNet = GetComponent<PlayerInformation_Net>();
		col = gameObject.GetComponent<CapsuleCollider>();
		rb  = gameObject.GetComponent<Rigidbody>();
		// playerManager = gameObject.GetComponent<PlayerManager>();
		
		inputModule = GameObject.Find("EventSystem").gameObject.GetComponent<StandaloneInputModule>();
		model = transform.Find("Model").transform.Find("default").transform.Find("Ace_Mesh").gameObject; // Change this later
		GroundedCharacterSpeed		+= GlobalVars.Instance.GroundedCharacterSpeed;
		AimingSpeed					+= GlobalVars.Instance.AimingSpeed;
		JumpSpeed					+= GlobalVars.Instance.JumpSpeed;
		FallMultiplier				+= GlobalVars.Instance.FallMultiplier;
		MaxFallSpeed				+= GlobalVars.Instance.MaxFallSpeed;
		MaxDiveSpeed				+= GlobalVars.Instance.MaxDiveSpeed;
		AirMovementSpeed			+= GlobalVars.Instance.AirMovementSpeed;
		UmbrellaGravity				+= GlobalVars.Instance.UmbrellaGravity;
		UmbrellaFallingSpeed		+= GlobalVars.Instance.UmbrellaFallingSpeed;
		Gravity						+= GlobalVars.Instance.Gravity;
		ExtraDivingSpeed			+= GlobalVars.Instance.ExtraDivingSpeed;
		SpeedDecay					+= GlobalVars.Instance.SpeedDecay;
		SpeedDecayTimer				+= GlobalVars.Instance.SpeedDecayTimer;
		UmbrellaMaxHealth			+= GlobalVars.Instance.UmbrellaMaxHealth;
		RespawnTime					+= GlobalVars.Instance.RespawnTime;
		FallDamageSpeedThreshold	+= GlobalVars.Instance.FallDamageSpeedThreshold;
		CanUseWeapon = true;
		canUseUlt = true;

		tempMaxUmbrellaHP = UmbrellaMaxHealth;
		tempRespawn = RespawnTime;
		
		UmbrellaNet.MaxHitPoints = UmbrellaMaxHealth;

		initialCamDis	  = CamCon.CamDistance;
		resetAirSpeed = AirMovementSpeed;
	}
		
	void Update()
    {
		
		if(Input.GetButtonDown(Axes.toStr[StartButton]))
			PauseMenu();
		if(CanMove)
			MovePlayer(-Axes.GetAxis(Vertical), Axes.GetAxis(Horizontal));		
		if(CanJump)
			Jump(Input.GetButtonDown(Axes.toStr[JumpButton]));
		CheckRotation();
		if(CanUseUmbrella)
		{
			UmbrellaState(Input.GetButtonDown(Axes.toStr[UmbrellaButton]));
			if(UmbrellaNet.CanActivateShield)
				ShieldState(Input.GetButtonDown(Axes.toStr[ShieldButton]));
		}
		//CheckDead();
    }

//--------------------------------------------------------------------------METHODS:

	/*
	Example of adding force function
	public void AddForce(Vector3 f) {
		netForce+=f;
	}
	 */


	/// <Summary>
	/// Stops player from moving, shooting, or using the umbrella
	/// </Summary>
	public void DisablePlayer()
	{
		CanMove = false;
		canDive = false;
		DisableWeapon();
		CanJump = false;
		CanUseUmbrella = false;
	}
	/// <Summary>
	/// Allows player to move, shoot, and use the umbrella
	/// </Summary>
	public void EnablePlayer()
	{
		CanMove = true;
		canDive = true;
		EnableWeapon();
		CanJump = true;
		CanUseUmbrella = true;
	}


	public bool IsGrounded()
    {
        float groundPos = col.bounds.extents.y;

        float r = new Vector3(col.bounds.extents.x/2, 0, col.bounds.extents.z/2).magnitude;
		Collider[] allHits = Physics.OverlapSphere(transform.position + groundPos * Vector3.down,
													r);
		
		if(allHits.Length > 1)
        {
			//rb.velocity.y is negative while falling.
			//If rb.velocity.y is a lower number than the threshold go in
			if(rb.velocity.y < FallDamageSpeedThreshold)
			{
				playerInformationNet.FallDamage();
				return true;
			}
			//Set grounded
			Animator.SetBool("Free Falling", false);
			Animator.SetBool("Grounded", true);
            canDive = false;
			return true;
        }
		else
		{
			Animator.SetBool("Free Falling", true);
			Animator.SetBool("Grounded", false);
			return false;
		}
    }

	public bool CanShoot()
	{
		return CanUseWeapon;
	}

	public float GetRespawnCountdown()
	{
			return tempRespawn;
	}
	public void DisableWeapon()
	{
		CanUseWeapon = false;
	}
	public void EnableWeapon()
	{
		CanUseWeapon = true;
	}

	///<Summary>
	/// Increases player falling speed to the maximum diving speed
	/// Increases the player AirMovementSpeed if diving speed reaches thresholds
	/// Changes Camera position
	/// Activate the Diving particle effects (To be done)
	/// Change movement style 
	///</Summary>
	public void IsDiving(float forward)
	{
		if(canDive)
		{

            if (Mathf.Abs(forward) > 0)
			{
				DLog("Diving initialize");
                //Smoothly move the camera out
                callOnceDive = true;
				CamCon.CamDistance = Mathf.Lerp(CamCon.CamDistance, MaxCameraDistance, SmoothCameraTransition);
				Animator.SetBool("Free Falling", false);
				if(DivingParticles != null)
					DivingParticles.SetActive(true);
				Animator.SetBool("Diving", true);
                if (callOnceDive && callOnceOutDive)
                {
                    AudioSource.clip = DiveClip;
                    AudioSource.Play();
                    callOnceDive = false;
                    callOnceOutDive = false;
                }
                Diving =true;

				//Weapon can be used while diving
				//DisableWeapon();
            
                
			}
			else
			{
				CamCon.CamDistance = Mathf.Lerp(CamCon.CamDistance, initialCamDis, SmoothCameraTransition);
                AudioSource.Stop();
                callOnceOutDive = true;
                Diving = false;
				if(DivingParticles != null)
					DivingParticles.SetActive(false);
				Animator.SetBool("Diving", false);
				if(!IsDead)
					EnableWeapon();
                
			}
		}
        else
        {
            AudioSource.Stop();
			if(DivingParticles != null)
				DivingParticles.SetActive(false);
			Animator.SetBool("Diving", false);
            Diving = false;
        }
	}

	///<Summary>
	/// Check for camera vertical angle
	/// If the player is not on the ground and vert angle is < 0
	/// canDive = true;
	///</Summary>
	public void CheckRotation()
	{
		bool grounded = IsGrounded();

		if(!grounded && !UmbrellaNet.IsUmbrellaOpen)
		{
			if(CamCon.CamVertAngle >= 40 )
			{
				DLog("Can Dive");
				canDive = true;
			}
			else
			{
				canDive = false;
				if(DivingParticles != null)
					DivingParticles.SetActive(false);
				Animator.SetBool("Diving", false);
				Diving = false;
			}
			IsDiving(-Axes.GetAxis(Vertical));
		}
		else if(CamCon.CamDistance != initialCamDis && !devTool_AllowCameraAdjustment)
			CamCon.CamDistance = Mathf.Lerp(CamCon.CamDistance, initialCamDis, SmoothCameraTransition*2);
	}

	///<Summary>
	/// Add this function after shooting/diving/grounded
	/// Resets the air speed decay timer
	///</Summary>
	public void ResetAirSpeed()
	{
		umbrellaSpeedDecayTimer = Time.time + SpeedDecayTimer;
		AirMovementSpeed = resetAirSpeed;
	}
	///<Summary>
	///	Draws the sphere cast to see when the player touches the ground
	/// Uncomment to see it
	///</Summary>

	// void OnDrawGizmosSelected()
    // {
    //     	// Draw a yellow sphere at the transform's position
    //     	Gizmos.color = Color.red;
	// 		Gizmos.DrawSphere(transform.position + col.bounds.extents.y * Vector3.down, new Vector3(col.bounds.extents.x/2, 0, col.bounds.extents.z/2).magnitude);
    // }

	
	/// <Summary>
	/// Assign the player as dead
	/// </Summary>
	public void ModelEnabled(bool isEnabled)
	{
		model.GetComponent<SkinnedMeshRenderer>().enabled = isEnabled;
		DLog("Model changed!");
	}

	public void KinematicEnabled(bool isEnabled){
		GetComponent<Rigidbody>().isKinematic = isEnabled;
		IsFrozen = isEnabled;
	}

	/// <Summary>
	/// Move the player to a respawn location after x seconds
	/// Re-enable the player controllers
	/// </Summary>
	public void ResetStatsAfterDeath()
	{
		DLog("Reset Stats!");
		IsDead = false;
		playerInformationNet.PlayerMaxHealth = playerInformationNet.TempMaxHP;
        playerInformationNet.PlayerHealth = playerInformationNet.PlayerMaxHealth;
		UmbrellaMaxHealth = tempMaxUmbrellaHP;
		tempRespawn = RespawnTime;
	}
//--------------------------------------------------------------------------HELPERS:


	private void Jump(bool jButton)
	{
		bool grounded = IsGrounded();
		
		// DLog("Jump Button: " + Input.GetButtonDown(Axes.toStr[JumpButton]).ToString());

		if(jButton && grounded)
		{
			DLog("Jumping at the speed of " + JumpSpeed.ToString());
			rb.velocity = new Vector3(rb.velocity.x, JumpSpeed, rb.velocity.z) ;
			ResetAirSpeed();
		}
		else if(rb.velocity.y < 0)
		{
			//Falling velocity section
			if(UmbrellaNet.IsUmbrellaOpen && !Diving)
			{
				//DLog("y velocity: " +rb.velocity.y);
				//Increase falling speed of the player with umbrella if they haven't reached maximum velocity.
				if(Mathf.Abs(rb.velocity.y) <= UmbrellaFallingSpeed)
				{
					DLog("Umbrella Falling Accelerating");
					rb.AddForce(Vector3.up*UmbrellaGravity);
					ConstVelocity.y = Mathf.Lerp(ConstVelocity.y, 
										UmbrellaFallingSpeed, 
										(FallMultiplier-1)*Time.deltaTime);
				}
				else
				{
					DLog("Max falling speed with Umbrella open");
					rb.velocity = new Vector3(rb.velocity.x, 
											-UmbrellaFallingSpeed, 
											rb.velocity.z);
				}
			}
			else if (Diving)
			{
				if (Mathf.Abs(rb.velocity.y) <= MaxDiveSpeed)//maxdive * sin(angle) - angle starts after looking a little bit down 
				{
					rb.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier +1) * Time.deltaTime;	
					//ConstVelocity.y = Mathf.Lerp(ConstVelocity.y, -MaxDiveSpeed, FallMultiplier*Time.deltaTime);
					DLog("Increasing Diving speed");
				}
				else
				{
					DLog("Max Diving Speed");
					rb.velocity = new Vector3(rb.velocity.x, -MaxDiveSpeed, rb.velocity.z);
				}
			}
			// If shielding, player falls at normal falling speeds.
			else if(Mathf.Abs(rb.velocity.y)  <= MaxFallSpeed || (UmbrellaNet.IsUmbrellaOpen && UmbrellaNet.IsShielding))
			{
				//ConstVelocity.y = Mathf.Lerp(ConstVelocity.y, -MaxFallSpeed, FallMultiplier*Time.deltaTime);
				rb.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier -1) * Time.deltaTime;	
				DLog("Max falling speed");
			}	
			else
			{
				rb.velocity = new Vector3(rb.velocity.x, -MaxFallSpeed, rb.velocity.z);	
			}
		}
	}
	private void MovePlayer(float vertical, float horizontal)
	{
		bool grounded = IsGrounded();

		Vector3 CamForward = Vector3.Cross(Vector3.up,Cam.transform.right).normalized;

		//this is now the direction that the player's moving
		Vector3 InputVector = new Vector3(vertical,
								0,
								horizontal);
		Vector3 PlayerDirection = InputVector.x * CamForward + 
									InputVector.z * Cam.transform.right;
		PlayerDirection.y = 0f;

		if(CamCon.CanUseCamera)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation,
										Cam.transform.rotation, 
										10 * Time.deltaTime);
		}

		//DLog("Vector Input: " + InputVector.ToString());

		if (InputVector.magnitude > .1) 
		{
			float atan2 = Mathf.Atan2(PlayerDirection.x, PlayerDirection.z) * Mathf.Rad2Deg; //- 90;
			transform.rotation = Quaternion.Lerp(transform.rotation, 
									Quaternion.Euler(0, (atan2), 
									0), 
									10 * Time.deltaTime);
			Animator.SetBool("Running", true);						
		}
		else
			Animator.SetBool("Running", false);		
		
		if (grounded)
		{
			PlayerMove = GroundedCharacterSpeed * PlayerDirection;
			ResetAirSpeed();
		}
		else
			PlayerMove = AirMovementSpeed * PlayerDirection;

		//Hold player on the ground
		if(grounded && NetForce.y <= 0)			NetForce.y = 0;
		if(grounded && ConstVelocity.y <= 0)		ConstVelocity.y = -.1f;

		ConstVelocity += NetForce*Time.deltaTime;

		PlayerMove+=ConstVelocity;

		//While(because it's in update) not grounded and umbrella is open
		//and after SpeedDecayTimer, speed starts to decay.
		if(!grounded && Time.time >= umbrellaSpeedDecayTimer 
				&& UmbrellaNet.IsUmbrellaOpen && AirMovementSpeed >= 1)
		{
			AirMovementSpeed -= SpeedDecay*Time.deltaTime;
		}

		Vector3 v = PlayerMove;
		v.y=0;

		//Move the player
		rb.velocity=v+new Vector3(0,rb.velocity.y,0);
		RigidBodyValue = rb.velocity;
		NetForce=UmbrellaGravity*Vector3.down;
	}

	private void UmbrellaState(bool uButton)
	{
		if(uButton)
		{
			//Stop diving even if you are looking down
			if(Diving)
			{
				//If you actually dived and your speed is around 25
				if(Mathf.Abs(ConstVelocity.y)  >= (MaxFallSpeed+MaxFallSpeed)/2)
				{
					DLog("Diving after speed: " );
                    
					AirMovementSpeed = resetAirSpeed+ExtraDivingSpeed;
				}
				/// TODO : "Camera Jank"
				AudioSource.Stop();
				if(DivingParticles != null)
					DivingParticles.SetActive(false);
				Animator.SetBool("Diving", false);
				Diving = false;
			}				
			
			if(UmbrellaNet.IsUmbrellaOpen)
			{
				Animator.SetBool("Umbrella Falling", false);	
				Animator.SetBool("Free Falling", true);	

				if(!IsDead)
					EnableWeapon();
				UmbrellaNet.CloseUmbrella();
			}
			else
            {
				Animator.SetBool("Free Falling", false);
				Animator.SetBool("Umbrella Falling", true);		

				if(!IsDead)
					EnableWeapon();
                canDive = false;
				UmbrellaNet.OpenUmbrella();
            }
		}
	}

	private void ShieldState(bool uButton)
	{
		if (uButton)
		{
			// Shield can only be activated if the umbrella is open.
			if(UmbrellaNet.IsUmbrellaOpen && UmbrellaNet.CanActivateShield)
			{
				UmbrellaNet.ActivateShield();
			}
		}
	}

	private void PauseMenu()
	{
		DLog("Start pressed");
		GameObject optionButton = GameManager.Instance.MainCanvas.transform.Find("Option Menu").transform.Find("Options Button").gameObject;
		Options_Net options = GameManager.Instance.MainCanvas.GetComponent<Options_Net>();

		if(Time.timeScale == 1)
		{
			//Time.timeScale = 0; //Can't stop time during online
			options.GetPlayerNumber(playerInformationNet.PlayerNumber);
			options.PauseMenu();
			options.PCN = this;
			PauseInfo(optionButton);
			GameManager.Instance.PauseAllPlayers(true);
		}
		else
		{
			options.ReturnToGame();
		}
	}

	private void PauseInfo(GameObject optionButton)
	{
		inputModule.horizontalAxis = Axes.toStr[Horizontal];
		inputModule.verticalAxis = Axes.toStr[Vertical];
		inputModule.submitButton = Axes.toStr[JumpButton];
		inputModule.cancelButton = Axes.toStr[UmbrellaButton];

	}

	//You can only take damage from the server. This will send the information
	//to all players, that this playercontroller has died.
	//This is a Remote Procedure Call

	[ClientRpc]
	private void RpcDied()
	{
		//This sends the information to everyone that player died.
		IsDead = true;
	}

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
