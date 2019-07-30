using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
public class AirCurrent : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public float Strength, TopPos, BotPos, UmbrellaOpenStrength;
	public bool DevTool;
	private GameObject[] players;
	private bool[] top;
	private float tempGravity;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		if(DevTool)
		{
			players = new GameObject[2];
			top = new bool[2];
		}
		else
		{
			StartCoroutine(GetPlayerCounter());
		}
		tempGravity = GlobalVars.Instance.Gravity;

		TopPos = transform.position.y + TopPos;
		BotPos = transform.position.y + BotPos;
	}
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			//Find which player enter the fan
			int playerNumber = GetPlayerNumber(col);
			DLog("Player Number: " +playerNumber);
			//Assign that player to the array if not already in there
			if(players[playerNumber] == null)
			{
				DLog("Player GameObject: " +col.gameObject);
				players[playerNumber] = col.gameObject;
			}

			//Disable gravity/netforce/constant velocity
			players[playerNumber].GetComponent<PlayerController>().Gravity = 0;
			players[playerNumber].GetComponent<PlayerController>().NetForce = Vector3.zero;
			players[playerNumber].GetComponent<PlayerController>().ConstVelocity = Vector3.zero;
			top[playerNumber] = false;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.tag == "Player")
		{
			//Find which player exit the fan
			int playerNumber = GetPlayerNumber(col);

			//Re-enable gravity on player
			players[playerNumber].GetComponent<PlayerController>().Gravity = tempGravity;
			top[playerNumber] = true;
		}
	}

	void OnTriggerStay(Collider col)
	{
		//Find which players in the fan
		DLog("Staying");
		if(col.tag == "Player")
		{
			int playerNumber = GetPlayerNumber(col);

			Vector3 dir = transform.up;
			Vector3 vel = Vector3.zero;
			float d = Vector3.Project(col.transform.position - transform.position, dir).magnitude;
			vel = Strength * dir;
			if(players[playerNumber].GetComponent<PlayerController>().Umbrella.IsUmbrellaOpen)
			{
				vel = UmbrellaOpenStrength * dir;
			}

			Vector3 a = vel/Time.deltaTime;
			players[playerNumber].GetComponent<Rigidbody>().AddForce(a);

			Vector3 newPosition;
			if (top[playerNumber])
			{
				newPosition = new Vector3(players[playerNumber].transform.position.x, TopPos, players[playerNumber].transform.position.z);
			}
			else
			{
				newPosition = new Vector3(players[playerNumber].transform.position.x, BotPos, players[playerNumber].transform.position.z);
			}
			players[playerNumber].transform.position = Vector3.MoveTowards(players[playerNumber].transform.position, newPosition, Strength * Time.deltaTime);
		}
	}
//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
	private int GetPlayerNumber(Collider col)
	{
		Enums.Player playerEnum = col.GetComponent<PlayerInformation>().PlayerNumber;
		string playerEnumLastVal = playerEnum.ToString().Substring(playerEnum.ToString().Length-1, 1);
		int playerNumber = Utility.IntParseFast(playerEnumLastVal)-1;
		return playerNumber;
	}

	private IEnumerator GetPlayerCounter(){
        
        yield return new WaitForSeconds(1); 
		players = new GameObject[GameManager.Instance.PlayerCounter];
		DLog("Players count: " + players.Length);
		top = new bool[GameManager.Instance.PlayerCounter];
    }
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}