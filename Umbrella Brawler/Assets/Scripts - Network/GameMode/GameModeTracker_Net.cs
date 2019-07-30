using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class GameModeTracker_Net : NetworkBehaviour 
{

//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
public float MatchTime;
	public int MaxScore;
	public int[] TeamScore;
	public Canvas MainCanvas;

	private List<PlayerManager_Net> player; 
	private PlayerController_Net[] playerControllers;
	private PlayerInformation_Net[] playerInformations;
	private PlayerCanvas_Net[] playerCanvas;
	private Enums.Team[] playerTeam;
	private bool runOnce, timeRunOut, stopTimer;
	private int deadPlayer;
	private float tempRespawnTimer;
	private Text gameTimer;
//---------------------------------------------------------------------MONO METHODS:


	void Awake(){
        this.enabled = false;
		Debug.Log("gamemodetracker was disabled.");
	}
	
	void Start() 
	{
		//If this is not the server, delete it. This is server only script
		if(!isServer)
		{
			Destroy(this);
			return;
		}

		MatchTime *= 60;
		deadPlayer = 500; //Start as an impossible player
		runOnce = false;
		stopTimer = false;
		player = GameManager_Net.Instance.Players_Net;
		playerTeam = new Enums.Team[player.Count];
		playerControllers = new PlayerController_Net[player.Count];
		playerInformations = new PlayerInformation_Net[player.Count];
        playerCanvas = new PlayerCanvas_Net[player.Count];
		TeamScore = new int [player.Count]; //Get up to player teams
		gameTimer = MainCanvas.transform.Find("Game Timer").gameObject.GetComponent<Text>();

        // //Need a "GetPlayerController" and "GetPlayerCanvas" in playermanager
        // playerControllers = FindObjectsOfType<PlayerController>();
        // playerCanvas = FindObjectsOfType<PlayerCanvas>();

        // System.Array.Reverse(playerControllers);
        // System.Array.Reverse(playerCanvas);



        player = GameManager_Net.Instance.Players_Net;
        for (int i = 0; i < player.Count; i++)
        {

            playerTeam[i] = player[i].Team;
            playerControllers[i] = player[i].GetPlayerController();
			playerInformations[i] = player[i].GetPlayerInformation();
            playerCanvas[i] = player[i].GetPlayerCanvas();
            Debug.Log("found controller: " + playerControllers[i].name);
            Debug.Log("found canvas: " + playerCanvas[i].name);
        }
        
		
	}

    void Update()
    {
        RpcGameCountdown();
    }

//--------------------------------------------------------------------------METHODS:
	
//--------------------------------------------------------------------------HELPERS:

	/// <Summary>
	/// Check if a player has died
	/// </Summary>
	private bool CheckForDeath()
	{
		DLog("Checking for death");
		for(int i = 0; i < player.Count; i++)
		{
			if(player[i].IsInstanceDead())
			{
				//DLog("Player " + playerControllers[i].name + " has died by " 
				//		+ playerControllers[i].KilledByTeam);
				//If it's the same player and it has not respawned
				if(deadPlayer == i && Time.time <= tempRespawnTimer)
				{
					return false;
				}
				
				deadPlayer = i;
				runOnce = false;
				tempRespawnTimer = Time.time+playerControllers[i].RespawnTime;
				return true;
			}	
		}
		return false;
	}

	/// <Summary>
	/// Check if a team reached max score
	/// </Summary>
	private void CheckForWinner(int recentKillTeam)
	{
		if(TeamScore[recentKillTeam] >= MaxScore || timeRunOut)
		{
			if(timeRunOut)
			{
				//Check which team had the highest score
				int max = TeamScore[0];
				for (int i = 1; i < TeamScore.Length; i++) 
				{
					if (TeamScore[i] > max) 
					{
						max = TeamScore[i];
					}
				}
				for(int i = 0; i < player.Count; i++)
				{
					if((int)playerInformations[i].TeamNumber == max)
						playerCanvas[(int)playerInformations[i].TeamNumber].EndState.text ="VICTORY!";
					else
						playerCanvas[(int)playerInformations[i].TeamNumber].EndState.text ="DEFEAT!";
					
					playerControllers[i].enabled = false;
				}
			}
			else
			{
				for(int i = 0; i < playerControllers.Length; i++)
				{
					if((int)playerInformations[i].TeamNumber == recentKillTeam)
					{
						playerCanvas[(int)playerInformations[i].TeamNumber].EndState.text ="VICTORY!";
					}
					else
					{
						playerCanvas[(int)playerInformations[i].TeamNumber].EndState.text ="DEFEAT!";
					}
					//For now, manually disable all playercontrollers
					//Later call the method in game manager
					playerControllers[i].enabled = false;
				}
			}
			//Activate return to main menu button.
			//Allow player 1 to select to main menu
			stopTimer = true;
			CmdOpenMenus();
		}
	}


	/// <Summary>
	/// Slows down time to all users
	///	Server sends information to everyone to open their menu after game is over
	/// </Summary>
	[Command]
	private void CmdOpenMenus()
	{
		Time.timeScale = 0.25f;
		MainCanvas.transform.Find("Option Menu").gameObject.SetActive(true);

		GameObject reset = MainCanvas.transform.Find("Option Menu").transform.Find("Restart").gameObject;
		MainCanvas.transform.Find("Option Menu").transform.Find("Return to game").gameObject.SetActive(false);
		MainCanvas.transform.Find("Option Menu").transform.Find("Options Button").gameObject.SetActive(false);

		EventSystem.current.GetComponent<EventSystem>()
							.firstSelectedGameObject = reset;
		EventSystem.current.GetComponent<EventSystem>()
							.SetSelectedGameObject(reset);
	}

	/// <Summary>
	/// Start game countdown
	/// Client calls the counter from the server
	/// </Summary>
	[ClientRpc]
	private void RpcGameCountdown()
	{	
		
		//Start timer of Timer -- until 0. x.xx 
		if(!stopTimer)
			MatchTime -= Time.deltaTime;
		if(MatchTime >= 0)
			gameTimer.text = MatchTime.ToString("F2");
		else
		{
			gameTimer.text = "0";
			timeRunOut = true;
			CheckForWinner(0);
			DLog("Time Ran Out");
		}	
	}
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}