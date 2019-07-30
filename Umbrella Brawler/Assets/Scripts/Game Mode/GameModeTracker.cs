using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Enums;
using System.Linq;

public class GameModeTracker : MonoBehaviour 
{
    private class PlayerGameModeData
    {
        public Team team;
        public PlayerCanvas playerCanvas;
        public PlayerController playerController;
        public PlayerInformation playerInformation;
        public PlayerManager playerManager;

        public PlayerGameModeData(PlayerManager a_playerManager)
        {
            playerCanvas = a_playerManager.GetPlayerCanvas();
            playerController = a_playerManager.GetPlayerController();
            playerInformation = a_playerManager.GetPlayerInformation();
            team = a_playerManager.Team;
            playerManager = a_playerManager;
        }
    }

    //------------------------------------------------------------------------CONSTANTS:

    public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public float MatchTime;
	public int MaxScore;
	public int[] TeamScore;
	public Canvas MainCanvas;

	private PlayerManager playerManager;
	private List<PlayerManager> player; 
	private PlayerController[] playerControllers;
	private PlayerInformation[] playerInformations;
	private PlayerCanvas[] playerCanvas;
	private Enums.Team[] playerTeam;
	private bool runOnce, timeRunOut, stopTimer;
	private int deadPlayer;
	private float tempRespawnTimer;
    private float minutes, seconds;
	private Text gameTimer;


    // -------------------------------------------------
    private List<PlayerGameModeData> players;
    private Dictionary<Team, int> team_score;


//---------------------------------------------------------------------MONO METHODS:


	void Awake(){
        this.enabled = false;
		Debug.Log("gamemodetracker was disabled.");
	}
	
	void Start() 
	{
        //MatchTime *= 60;
        MatchTime = GameManager.Instance.MatchTime;
        MaxScore = GameManager.Instance.MaxScore;
		deadPlayer = 500; //Start as an impossible player
		runOnce = false;
		stopTimer = false;
		player = GameManager.Instance.Players;
		playerTeam = new Enums.Team[player.Count];
		playerControllers = new PlayerController[player.Count];
		playerInformations = new PlayerInformation[player.Count];
        playerCanvas = new PlayerCanvas[player.Count];
		TeamScore = new int [player.Count]; //Get up to player teams
		gameTimer = MainCanvas.transform.Find("Game Timer").gameObject.GetComponent<Text>();
        team_score = new Dictionary<Team, int>();
        players = new List<PlayerGameModeData>();

        // //Need a "GetPlayerController" and "GetPlayerCanvas" in playermanager
        // playerControllers = FindObjectsOfType<PlayerController>();
        // playerCanvas = FindObjectsOfType<PlayerCanvas>();

        // System.Array.Reverse(playerControllers);
        // System.Array.Reverse(playerCanvas);


        player = GameManager.Instance.Players;


        // Set all team scores based on DISTINCT teams that exist in this match
        foreach(Team team in (player.Select(p => p.Team).Distinct()))
        {
            team_score[team] = 0;
        }

        // Set the players 
        for (int i = 0; i < player.Count; i++)
        {
            var iPlayer = player.Where(x => x.PlayerNumber == (Player)i).First();

            players.Add(new PlayerGameModeData(iPlayer));




            if (iPlayer != null)
            {
                playerTeam[i] = iPlayer.Team;
                playerControllers[i] = iPlayer.GetPlayerController();
                playerInformations[i] = iPlayer.GetPlayerInformation();
                playerCanvas[i] = iPlayer.GetPlayerCanvas();
                Debug.Log("found controller: " + playerControllers[i].name);
                Debug.Log("found canvas: " + playerCanvas[i].name);
            }
        }




    }

    void Update()
    {
        GameCountdown();
    }

//--------------------------------------------------------------------------METHODS:
	
//--------------------------------------------------------------------------HELPERS:
	/// <Summary>
	/// If a player dies, add +1 to the team that killed it
	/// After adding the value, reset checkfordeath
	/// </Summary>
	public void AddScore()
	{
		if(CheckForDeath())
		{
            // Find the team that killed an opponent
            Team killingTeam = playerInformations[deadPlayer].KilledByTeam;

            // Update that team's score
            team_score[killingTeam]++;

            // Update the canvases for each player belonging to the team
            foreach (PlayerGameModeData pgmd in players.Where(p => p.team == killingTeam))
            {
                pgmd.playerCanvas.ScoreText.text = team_score[killingTeam] + " kill" + ((team_score[killingTeam] > 1) ? "s" : "");
            }

            CheckForWinner(killingTeam);
            runOnce = true;
        }
	}

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
    /// </Summary>(team_score[recentKillTeam] >= MaxScore) 
    private void CheckForWinner(Team lastKillTeam)
    {
        if (timeRunOut || team_score[lastKillTeam] >= MaxScore)
        {
            int highestScore = team_score.Values.Max();
            Team winningTeam = (timeRunOut) ? team_score.FirstOrDefault(team => team.Value == highestScore).Key : lastKillTeam;

            foreach (PlayerGameModeData pgmd in players)
            {
                if (pgmd.team == winningTeam)
                    pgmd.playerCanvas.EndState.sprite = pgmd.playerCanvas.winMessage;
                else
                    pgmd.playerCanvas.EndState.sprite = pgmd.playerCanvas.loseMessage;

                pgmd.playerCanvas.EndGameFade();

                pgmd.playerController.enabled = false;
            }

            //Activate return to main menu button.
            //Allow player 1 to select to main menu
            stopTimer = true;
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
    }
   

	/// <Summary>
	/// Start game countdown
	/// </Summary>
	private void GameCountdown()
	{	
		
		//Start timer of Timer -- until 0. x.xx 
		if(!stopTimer)
			MatchTime -= Time.deltaTime;

		if(MatchTime >= 0)
        {
            TimeChanger();
        }
		else
		{
			gameTimer.text = "0";
			timeRunOut = true;
			CheckForWinner(0);
			DLog("Time Ran Out");
		}	
	}

    ///<Summary>
    /// Convert seconds to minute and seconds
    ///</Summary>
    private void TimeChanger()
    {
        TimeSpan ts = TimeSpan.FromSeconds(MatchTime);

        minutes = ts.Minutes;
        seconds = ts.Seconds;

        //Check for timer to become x:yy, 0:yy, x:0y, yy, y
        // where x = minutes, y = seconds
        if (minutes == 0) // print yy and y
        {
            //Only print seconds remaining
            gameTimer.text = "  " + seconds.ToString("F0");
        }
        else if (seconds <= 9) //Print x:0y
        {
            gameTimer.text = minutes.ToString("F0") + ":0" + seconds.ToString("F0");
        }
        else // print x:yy
        {
            gameTimer.text = minutes.ToString("F0") + ":" + seconds.ToString("F0");
        }
    }

    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }

}