using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS
using Enums;
using System;

public class CapturePoint : MonoBehaviour
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	[Tooltip("How many points required to capture point")]
	public float MaxCapture;
	[Tooltip("How fast the point gets captured at, 1 = 1 per second")]
	public float CaptureRate;
	[Tooltip("How fast the point gets decaptured at, 1 = 1 per second")]
	public float DecaptureRate;
	[Tooltip("Give x points to the team that has it captued")]
	public float PointsPerSecond;
	[Tooltip("Current Capture number, DON'T TOUCH")]
	public float CaptureValue;
	[Tooltip("If true, CaptureValue increases. Turns false if another team joins or no player is inside")]
	public bool Capturing;
	[Tooltip("If true, this capture point increase points in DominationMode.cs")]
	public bool Captured;
	[Tooltip("Shows which team has this point captured")]
	public Team PointCapturedBy;
	public bool DevMode;
	private GameObject[] players;
	private List<Team> teamsInside;
	private Team capturedByTeam;
	private bool oneTeamInside;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		PointCapturedBy = Team.None;
		if(DevMode)
		{
			teamsInside = new List<Team>(2);
			players = new GameObject[2];
		}
		else
		{
			players = new GameObject[GameManager.Instance.PlayerCounter];
			teamsInside = new List<Team>(GameManager.Instance.NumberOfTeams);
		}
	}
		
	void Update()
    {
		
    }

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			int playerNumber = GetPlayerNumber(col);
			int playerTeam = GetPlayerTeam(col);
			//Get team information
			if(players[playerNumber] == null)
			{
				DLog("Player GameObject: " +col.gameObject);
				players[playerNumber] = col.gameObject;
			}
			// Add team to the list that are INSIDE the object
			teamsInside.Add(col.GetComponent<PlayerInformation>().TeamNumber);
			CheckTeams();
		}
	}

	void OnTriggerStay(Collider col)
	{
		if(col.tag == "Player")
		{
			Conquering();
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.tag == "Player")
		{
			int playerTeam = GetPlayerTeam(col);
			// Remove team from the list when they leave the object
			teamsInside.Remove(col.GetComponent<PlayerInformation>().TeamNumber);
			CheckTeams();
		}
	}

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
	///<Summary>
	/// Capturing point and the aftermath
	///</Summary>
	private void Conquering()
	{
		if(!Captured)
		{
			if(Capturing && CaptureValue < MaxCapture)
			{
				//Increase capture value
				CaptureValue += Time.deltaTime * CaptureRate;
			}
			else if(CaptureValue > 0 && !Capturing)
			{
				CaptureValue -= Time.deltaTime * DecaptureRate;
			}
			if(CaptureValue >= MaxCapture)
			{
				PointCapturedBy = capturedByTeam;
				Captured = true;
			}
		}
		else //Can be decaptured
		{
			DeConquering();
		}
	}

	///<Summary>
	/// Decapturing point after a new team comes in
	///</Summary>
	private void DeConquering()
	{
		//Check that it's another team capping
		if(capturedByTeam != PointCapturedBy)
		{
			if(CaptureValue > 0 && Captured)
			{
				//Reset capture state to 0
				CaptureValue -= Time.deltaTime;
			}
			if(Captured && CaptureValue <= 0)
			{
				//Go back to Conquering
				Captured = false;
			}
		}
	}

	///<Summary>
	/// Check all players inside the trigger collider
	/// If a new entity enters and is not from the same team
	/// Change oneTeamInside
	/// If an entity leaves the trigger change that value
	///</Summary>
	private void CheckTeams()
	{
		Team previousTeam;
		DLog("Teams Inside Count: " + teamsInside.Count);
		for(int i = 0; i < teamsInside.Count; i++)
		{
			if(teamsInside.Count == 1) //Literally only 1 player inside
			{
				Capturing = true;
				capturedByTeam = teamsInside[0];
				//After first iteration it should break
			}
			else
			{	
				if(i - 1 > -1) // Ignore first run
				{
					previousTeam = teamsInside[i-1];
					//If current teamsInside does NOT contains the previous team
					if(!teamsInside.ToString().Contains(previousTeam.ToString()))
					{
						Capturing = false;
						//This means that a new team is inside the object.
					}
					else
					{
						Capturing = true;
						//Return first team. There should only be 1 team anyway
						capturedByTeam = teamsInside[0];
					}
				}
			}
		}
	}

	///<Summary>
	/// Get player number from the enum string
	///</Summary>
	private int GetPlayerNumber(Collider col)
	{
		Enums.Player playerEnum = col.GetComponent<PlayerInformation>().PlayerNumber;
		string playerEnumLastVal = playerEnum.ToString().Substring(playerEnum.ToString().Length-1, 1);
		int playerNumber = Utility.IntParseFast(playerEnumLastVal)-1;
		return playerNumber;
	}

	///<Summary>
	/// Get player team number from the enum string
	///</Summary>
	private int GetPlayerTeam(Collider col)
	{
		Enums.Team playerEnum = col.GetComponent<PlayerInformation>().TeamNumber;
		string playerEnumLastVal = playerEnum.ToString().Substring(playerEnum.ToString().Length-1, 1);
		int playerTeam = Utility.IntParseFast(playerEnumLastVal)-1;
		return playerTeam;
	}
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}