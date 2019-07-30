using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Enums;
using System;

public class DominationMode : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	[Tooltip("How long the game will run for if score is not reached, in minutes")]
	public float MatchTime;
	[Tooltip("Max amount of points required to win")]
	public float MaxScore;
	[Tooltip("The Prefab named MainCanvas")]
	public Canvas MainCanvas;
	[Tooltip("All the triggers with capture point script")]
	public List<CapturePoint> CapturePoints;
	[Tooltip("Current Score for all teams")]
	public float[] IndividualTeamScore;
	
	public bool DevMode;
	
	private List<PlayerManager> player; 
	private PlayerController[] playerControllers;
	private PlayerInformation[] playerInformations;
	private PlayerCanvas[] playerCanvas;
	private bool stopTimer, timeRunOut;
	private float minutes, seconds;
	
	private int numberOfTeams;
	private Text gameTimer;
	private Text[] teamScoreText;
	private GameObject GameMode, killScore;

//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		GameMode = MainCanvas.transform.Find("Game Mode").gameObject;
        MatchTime = GameManager.Instance.MatchTime;
        MaxScore = GameManager.Instance.MaxScore;
        GameMode.SetActive(true);
		stopTimer = false;
		if(DevMode)
		{
			numberOfTeams = 2;
			teamScoreText = new Text[2];
		}
		else
		{
			playerControllers = new PlayerController[GameManager.Instance.Players.Count];
			numberOfTeams = GameManager.Instance.NumberOfTeams;
			playerInformations = new PlayerInformation[GameManager.Instance.Players.Count];
			playerCanvas = new PlayerCanvas[GameManager.Instance.Players.Count];
			teamScoreText = new Text[numberOfTeams];

			player = GameManager.Instance.Players;
			for (int i = 0; i < player.Count; i++)
			{
				var tempPlayer = player.Where(x => x.PlayerNumber == (Player)i).First();
				if (tempPlayer != null)
				{
					playerControllers[i] = tempPlayer.GetPlayerController();
					playerInformations[i] = tempPlayer.GetPlayerInformation();
					playerCanvas[i] = tempPlayer.GetPlayerCanvas();
				}
			}
		}

		//Deactivate Kill score since we don't use that for domination
		foreach(PlayerController x in playerControllers)
		{
			killScore = x.gameObject.transform.Find("PlayerCanvas").Find("Team Score").gameObject;
			killScore.SetActive(false);
		}
		//Deactivate Capture points if it's only 2 players
		if(GameManager.Instance.Players.Count == 2)
		{
			if(CapturePoints.Count > 1)
				CapturePoints.RemoveRange(1,CapturePoints.Count-1);
		}

		for(int i = 0; i < numberOfTeams; i++)
		{
			int x = i+1;
			teamScoreText[i] = GameMode.transform.Find("Team " + x + " Score").GetComponent<Text>();
			teamScoreText[i].gameObject.SetActive(true);
		}
		gameTimer = MainCanvas.transform.Find("Game Timer").gameObject.GetComponent<Text>();
		IndividualTeamScore = new float[numberOfTeams];
	}
		
	void Update()
    {
		GameCountdown();
		UpdateCanvas();
		//If game is still ongoing
		if(!timeRunOut)
			IncreaseScore();
    }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:

	/// <Summary>
	/// Check if a team reached max score
	/// </Summary>
	private void CheckForWinner()
	{
		for(int i = 0; i < IndividualTeamScore.Length; i++)
		{
			if(IndividualTeamScore[i] >= MaxScore || timeRunOut)
			{
				//Check which team had the highest score
				float max = IndividualTeamScore[0];
				for (int k = 1; k < IndividualTeamScore.Length; k++) 
				{
					if (IndividualTeamScore[k] > max) 
					{
						max = IndividualTeamScore[k];
					}
				}
				for(int k = 0; k < numberOfTeams; k++)
				{

                    playerCanvas[k].EndState.color = new Color(1,1,1,1);
					//If player team = team that won(which is i)
					if((int)playerInformations[k].TeamNumber == i)
					{
						playerCanvas[k].EndState.sprite = playerCanvas[k].winMessage;	
					}
					else
					{
						playerCanvas[k].EndState.sprite = playerCanvas[k].loseMessage;
					}	
					
					playerControllers[k].enabled = false;
				}
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

				timeRunOut = true;
			}
		}
	}

	/// <Summary>
	/// Check how many nodes have been captured
	/// Give +PointsPerSecond to corresponding PointCapturedBy team
	/// </Summary>
	private void IncreaseScore()
	{
		for(int i = 0; i < CapturePoints.Count; i++)
		{
			if(CapturePoints[i].Captured)
			{
				//Get the int value of the team number
				string playerEnumLastVal = CapturePoints[i].PointCapturedBy.ToString().Substring(CapturePoints[i].PointCapturedBy.ToString().Length-1, 1);
				int playerTeam = Utility.IntParseFast(playerEnumLastVal)-1;
				//Increase Individual Team Score x points per second to the team that has it captured
				IndividualTeamScore[playerTeam] += CapturePoints[i].PointsPerSecond * Time.deltaTime;
			}
		}
		CheckForWinner();
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
			CheckForWinner();
			DLog("Time Ran Out");
		}	
	}

	private void UpdateCanvas()
	{
		for(int i = 0; i < teamScoreText.Length; i++)
		{
			teamScoreText[i].text = "Team " + i + " Score: " + IndividualTeamScore[i].ToString("F0");
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
		else if(seconds <= 9) //Print x:0y
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