using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS
using UnityEngine.Networking;

public class PlayerInformation_Net : NetworkBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = true;

//---------------------------------------------------------------------------FIELDS:
	[Tooltip("Maximum health - global vars(ex. 100) + value(ex. 0) = max health(ex. 100)")]
	public float PlayerMaxHealth;
	public AudioClip PlayerGettingHit;
	public bool IsDead;
	public Enums.Team TeamNumber;
	public Enums.Player PlayerNumber;
	public int NetworkPlayerNumber;
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
	[System.NonSerialized]
	public float TempMaxHP;
	[System.NonSerialized]
	public Enums.Team KilledByTeam;
	private AudioSource audioSource;
	[SyncVar (hook = "ChangeHealth")] float newHealth;

//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		PlayerMaxHealth			+= GlobalVars.Instance.PlayerMaxHealth;
		
		TempMaxHP = PlayerMaxHealth;
		PlayerHealth = PlayerMaxHealth;

		audioSource = GetComponent<AudioSource>();

		if(!isLocalPlayer)
		{
			Debug.Log("Client");
			TeamNumber = Enums.Team.Team2;
		}
		else
		{
			Debug.Log("server");
			TeamNumber = Enums.Team.Team1;
		}
		
		//Set player ID
		StartCoroutine(SetPlayerIdentity());
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
		//Run damage calculation in the server and not local.
		//If ran local, then there would be n health values where n is the amount of players
		if(!isServer || PlayerHealth <= 0)
			return;

		newHealth = PlayerHealth + healthChange;
		
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
			RpcDied();
			
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
//--------------------------------------------------------------------------HELPERS:
	
	//Allow client to invoke info from server and ran on client
	[ClientRpc]
	private void RpcDied()
	{
		PlayerHealth = 0;
		IsDead = true;
		
	}
	private IEnumerator SetPlayerIdentity(){
        
        yield return new WaitForEndOfFrame(); 
		NetworkIdentity networkIdentity = gameObject.GetComponent<NetworkIdentity>();
		// Player number - total number of players
		// network identity starts populating before players themself, so we can end with up to 32 identities
		// but we only care about the last 16
		Debug.Log("Identity: " + MyUtility.Utility.IntParseFast(networkIdentity.netId.ToString()) + " - Observers: "+ networkIdentity.observers.Count);
		NetworkPlayerNumber = MyUtility.Utility.IntParseFast(networkIdentity.netId.ToString()) - networkIdentity.observers.Count;
    }
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}