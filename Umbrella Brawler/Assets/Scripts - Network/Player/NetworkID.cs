using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; 
using UnityEngine.Networking;

public class NetworkID : NetworkBehaviour 
{

//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public int NetworkPlayerId = 0;

//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		CmdGetID();
	}
		
	void Update()
    {

    }

//--------------------------------------------------------------------------METHODS:
	[Command]
	public void CmdGetID()
	{
		NetworkPlayerId = GameObject.FindObjectsOfType<NetworkID>().Length;

		// Below is the "What if multiple people join the server at the same time"
		//
		
		// int additive = 0;
		// //After object is instantiated, check how many NetworkID exist.
		// NetworkID[] playerID = new NetworkID[GameObject.FindObjectsOfType<NetworkID>().Length];
		// Debug.Log("Length: " + GameObject.FindObjectsOfType<NetworkID>().Length);
		// foreach(NetworkID ID in playerID)
		// {
		// 	//If my ID is not unique
		// 	if(ID.NetworkPlayerId == NetworkPlayerId)
		// 	{
		// 		additive++;
		// 	}
		// 	else
		// 	{
		// 		//Set my ID and break
		// 		NetworkPlayerId = GameObject.FindObjectsOfType<NetworkID>().Length + additive;
		// 		break;
		// 	}
		// }	
	}
//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
