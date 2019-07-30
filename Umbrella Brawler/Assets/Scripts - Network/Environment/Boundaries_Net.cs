using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Boundaries_Net : NetworkBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "Boundaries_Net";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	[Tooltip("Allow different interaction if it's the ground boundary")]
	public bool Ground;
	public bool Top;
	public float Timer, ThrowStrength;
	private bool canOpenUmbrella = false;
	private PlayerController_Net pc;
	private PlayerInformation_Net pi;
	private Rigidbody rb;
	private PlayerCanvas_Net pCanvas;

//---------------------------------------------------------------------MONO METHODS:

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			DLog("Player has entered");
			try
			{
				pc = col.GetComponent<PlayerController_Net>();
				pi = col.GetComponent<PlayerInformation_Net>();
				rb = col.GetComponent<Rigidbody>();
				pCanvas = col.transform.Find("PlayerCanvas").GetComponent<PlayerCanvas_Net>();
				if(Ground)
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
						if(!pc.UmbrellaNet.IsUmbrellaOpen)
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

	/// <Summary>
	/// Only used for bouncing back up
	/// </Summary>
	void OnTriggerStay(Collider col)
	{
		if(canOpenUmbrella)
		{
			if(pc.UmbrellaNet.IsUmbrellaOpen)
			{
				DLog("Bouncing from OnTriggerStay");
				rb.velocity = new Vector3(rb.velocity.x,
										ThrowStrength,
										rb.velocity.z);	
				pCanvas.ResetVals();
			}
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

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}