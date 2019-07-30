using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;
public class Boundaries : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "Boundaries";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	[Tooltip("Allow different interaction if it's the ground boundary")]
	public bool Ground;
	[Tooltip("Allow different interaction if it's the Lava boundary")]
	public bool Lava;
	[Tooltip("Deal X amount of damage to the player on hitting the lava. DAMAGE HAS TO BE POSITIVE")]
	public float LavaDamage;
	public bool Top;
	public float Timer, ThrowStrength;
    public float GForce = 1f;


    private bool canOpenUmbrella = false;
	private PlayerController pc;
	private PlayerInformation pi;
	private Rigidbody rb;
	private PlayerCanvas pCanvas;

//---------------------------------------------------------------------MONO METHODS:

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			DLog("Player has entered");
			try
			{
				pc = col.GetComponent<PlayerController>();
				pi = col.GetComponent<PlayerInformation>();
				rb = col.GetComponent<Rigidbody>();
				pCanvas = col.transform.Find("PlayerCanvas").GetComponent<PlayerCanvas>();
				if(Ground)
				{
					GroundedBoundary();
				}
				else if(Lava)
				{
					LavaBoundary();
				}
				else
				{
					DLog("Side Boundary");
					pCanvas.Timer = Timer;
					pCanvas.InitializeTimer("Return to the field in: ");

                }
			}
			catch(System.NullReferenceException e)
			{
				DLog(e.Message);
			}
		}
	}

	void OnTriggerStay(Collider col)
	{
		if(canOpenUmbrella)
		{
			if(pc.Umbrella.IsUmbrellaOpen)
			{
				DLog("Bouncing from OnTriggerStay");
				rb.velocity = new Vector3(rb.velocity.x,
										ThrowStrength,
										rb.velocity.z);	
				pCanvas.ResetVals();
			}
		}
        if (Top)
        {
            rb.AddForce(0, GForce * -9.81f, 0);
        }
	}
	/// <Summary>
	/// Calls reset when the player leaves
	/// </Summary>
	void OnTriggerExit(Collider col)
	{

		if(col.tag == "FallingObject")
		{	
			if(!Top){
				Destroy(col.gameObject);
				DLog("Falling object destroyed");
			}
		}
		
		if(col.tag == "Player")
		{	
			DLog("Exiting");
			pCanvas.ResetVals();
		}
	}
//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
	private void GroundedBoundary()
	{
		DLog("Ground collider");
		if(!pc.CanUseUmbrella)
		{
			//Reduce players health to 0 instead of killing it.
			//Killing it can create a bug where timer never disappears.
			pi.ChangeHealth(pi.PlayerMaxHealth);
		}
		else
		{
			if(!pc.Umbrella.IsUmbrellaOpen)
			{
				canOpenUmbrella = true;
				pCanvas.Timer = Timer;
				pCanvas.InitializeTimer("Open Umbrella in: ");
			}
			else
			{
				DLog("Bouncing");
				rb.velocity = new Vector3(rb.velocity.x,
										ThrowStrength,
										rb.velocity.z);					
			}
		}
	}

	///<Summary>
	/// Deal damage to player and then throws the up
	///</Summary>
	private void LavaBoundary()
	{
		Debug.Log("Damaged");
		pi.ChangeHealth(-LavaDamage);
		pc.LavaParticles.SetActive(true);
		pc.lpTimer = 2;
		rb.velocity = new Vector3(rb.velocity.x,
								ThrowStrength,
								rb.velocity.z);
	}
    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}