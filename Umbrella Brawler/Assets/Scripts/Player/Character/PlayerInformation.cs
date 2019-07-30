using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS

public class PlayerInformation : MonoBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public PlayerController PCtrl;

	[Tooltip("Maximum health - global vars(ex. 100) + value(ex. 0) = max health(ex. 100)")]
	public float PlayerMaxHealth = 0;
	public AudioClip PlayerGettingHit;
	public bool IsDead;
	public Enums.Team TeamNumber;
	public Enums.Player PlayerNumber;
	public delegate void HPChanged();
	public event HPChanged OnHPChanged;
	public float PlayerHealth
    {
        get
        {
            return playerHealth;
        }

        set
        {
            playerHealth = value;
            if (OnHPChanged != null)
            {
                OnHPChanged();
            }
        }
    }

	[SerializeField]
    private float playerHealth;
	private bool damageBoostActive;
	private float damageBoostDuration;
	[System.NonSerialized]
	public float TempMaxHP;
	[System.NonSerialized]
	public Enums.Team KilledByTeam;
	private AudioSource audioSource;
	public HitMarker Reticle;

	public enum PlayerStatus{
		NORMAL,
		FIRE,
		ELECTRIC
	}

	public PlayerStatus currentStatus;

	private float
		statusDamage,
		statusCooldown,
		statTempCooldown,
		velSlowdown;

//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		PlayerMaxHealth			+= GlobalVars.Instance.PlayerMaxHealth;

		TempMaxHP = PlayerMaxHealth;
		PlayerHealth = PlayerMaxHealth;
		damageBoostActive = false;
		damageBoostDuration = 0;
		currentStatus = PlayerStatus.NORMAL;
		statTempCooldown = 0;

		audioSource = GetComponent<AudioSource>();

		PCtrl = GetComponent<PlayerController>();
	}
		
	void Update()
    {
		damageBoostDurationTimer();
		checkStatus();
    }

//--------------------------------------------------------------------------METHODS:
	public float GetHealth()
	{
		return PlayerHealth;
	}

	//alters health with input value
	//checks for death
	public void ChangeHealth(float healthChange)
	{
		float newHealth = PlayerHealth + healthChange;
		if(newHealth > 0 && newHealth <= PlayerMaxHealth)
		{
            if(newHealth < PlayerHealth)
            {
                audioSource.PlayOneShot(PlayerGettingHit);
                PlayerHealth = newHealth;
            }
            else
            {
                PlayerHealth = newHealth;
            }
			
		}
		else if(newHealth <= 0){
			PlayerHealth = 0;
			//Death();
			IsDead = true;
		}
		else if(newHealth > PlayerMaxHealth)
		{
			PlayerHealth = PlayerMaxHealth;
		}

        if(OnHPChanged != null)
        {
            OnHPChanged();
        }

	}

	/// <Summary>
	/// Check for player Y speed. If above a threshold, kill/dmg player
	/// </Summary>

	public void FallDamage()
	{
		DLog("Faster than threshold");
		ChangeHealth(-PlayerHealth);
	}

	public void setDamageBoostActive(float boostDuration)
	{
		damageBoostActive = true;
		damageBoostDuration = boostDuration;
	}

	public bool isDamageBoostActive()
	{
		return damageBoostActive;
	}


//-------------------------------------------------------------------------Status Functions
	public void setFireStatus(float damage, float cooldown)
	{
		currentStatus = PlayerStatus.FIRE;
		statusDamage = damage;
		statusCooldown = cooldown;
		statTempCooldown = 1f;
	}

	public void setTeslaStatus(float damage, float cooldown, float slowdown)
	{
		currentStatus = PlayerStatus.ELECTRIC;
		statusDamage = damage;
		statusCooldown = cooldown;
		statTempCooldown = 1f;
		velSlowdown = slowdown;
	}

	public PlayerStatus getCurrentStatus()
	{
		return currentStatus;
	}

	public PlayerStatus fireStatus()
	{
		return PlayerStatus.FIRE;
	}
	public PlayerStatus electricStatus()
	{
		return PlayerStatus.ELECTRIC;
	}
	public PlayerStatus normalStatus()
	{
		return PlayerStatus.NORMAL;
	}

//--------------------------------------------------------------------------HELPERS:
	private void damageBoostDurationTimer()
	{
		if(damageBoostDuration > 0f)
			damageBoostDuration -= Time.deltaTime;
		if(damageBoostDuration <= 0f)
		{
			damageBoostActive = false;
			damageBoostDuration = 0f;
		}
	}

	private void checkStatus()
	{
		if(statusCooldown > 0 && currentStatus != PlayerStatus.NORMAL)
		{
			switch(currentStatus)
			{
				case PlayerStatus.FIRE:
					statTempCooldown -= Time.deltaTime;
					if(statTempCooldown <= 0)
					{
						ChangeHealth(-statusDamage);
						statTempCooldown = 1f;
						statusCooldown--;
						if(GetHealth() <= 0){
							DLog("Dead");
							GameManager.Instance.gameModeTracker.AddScore();
							currentStatus = PlayerStatus.NORMAL;
						}
					}
					break;
				case PlayerStatus.ELECTRIC:
					statTempCooldown -= Time.deltaTime;
					PCtrl.ConstVelocity *= velSlowdown;
					if(statTempCooldown <= 0)
					{
						ChangeHealth(-statusDamage);
						statTempCooldown = 1f;
						statusCooldown--;
						if(GetHealth() <= 0){
							DLog("Dead");
							GameManager.Instance.gameModeTracker.AddScore();
							currentStatus = PlayerStatus.NORMAL;
						}
					}
					break;
				default: break;
			}
			if(statusCooldown <= 0){
				currentStatus = PlayerStatus.NORMAL;
			}
		}
	}

    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}