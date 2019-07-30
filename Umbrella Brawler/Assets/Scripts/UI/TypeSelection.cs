using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Enums;

public class TypeSelection : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "TypeSelection";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public Transform[] CharacterLocation, UmbrellaLocation, WeaponLocation, UltimateLocation;
	public Axes.Action[] PlayerHorizontal, PlayerVertical, PlayerSubmit, PlayerReturn, PlayerStart, PlayerChooseTeam;
	public GameObject[] PlayerSelector, PlayersParent, RemoveObjects;
	public GameObject StartButton, SelectTeamText;

	private int teamVal;
	private GameMode gameMode;
	private Canvas canvas;
	private int[] currentPos, playersInTeam, currentCategory, teamPos;
	private float[] delay, backDelay;
	private float tempDelay, holdDownButtonDelay;
	private bool[] teamSelected, activePlayers, loadOutSelected;
	private bool startGame = false;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		tempDelay = 0.2f;
		holdDownButtonDelay = 2;
		canvas = FindObjectOfType<Canvas>();

		gameMode = GameManager.Instance.GameMode;
		playersInTeam = new int[4];
		backDelay = new float[4];
		activePlayers = new bool[4];
		currentPos = new int[5]; //X axis
		currentCategory = new int[4]; //Y axis
		teamPos = new int[4];
		teamSelected = new bool[4];
		delay = new float[4];
		loadOutSelected = new bool[4];
	

		for(int i = 0; i< 4; i++)
		{
			currentPos[i] = i;
			teamPos[i] = i;
			teamSelected[i] = false;
			delay[i] =tempDelay;
			backDelay[i] = holdDownButtonDelay;
			playersInTeam[i] = 0;
			loadOutSelected[i] = false;
			//DLog("Player in team: " + player[i].Team);
		}
	}
	
	void Update()
    {
		DLog("Players: " + ActivePlayersCheck());
		GetPlayerInput(Axes.GetAxis(PlayerHorizontal[0]), 0, Axes.GetAxis(PlayerVertical[0]));
		BackButton(Input.GetButtonDown(Axes.toStr[PlayerReturn[0]]), 0, currentPos[0]);
		BackReturn(Input.GetButton(Axes.toStr[PlayerReturn[0]]), 0);


		if(Input.GetButtonDown(Axes.toStr[PlayerStart[1]]))
		{
			//Show object for 1 frame
			DLog("Controller 2 activated");
			RemoveObjects[0].SetActive(false);
			PlayersParent[0].SetActive(true);
			activePlayers[1] = true;
		}
		else if(Input.GetButtonDown(Axes.toStr[PlayerStart[2]]))
		{
			//Show object for 1 frame
			DLog("Controller 3 activated");
			RemoveObjects[1].SetActive(false);
			PlayersParent[1].SetActive(true);
			activePlayers[2] = true;
		}
		else if(Input.GetButtonDown(Axes.toStr[PlayerStart[3]]))
		{
			//Show object for 1 frame
			DLog("Controller 4 activated");
			RemoveObjects[2].SetActive(false);
			PlayersParent[2].SetActive(true);
			activePlayers[3] = true;
		}
		
		if(activePlayers[1])
		{
			StartCoroutine(Player2Inputs());
		}
		else if(activePlayers[2])
		{
			StartCoroutine(Player3Inputs());
		}
		else if(activePlayers[3])
		{
			StartCoroutine(Player4Inputs());
		}

		//-----------------------
		if(startGame)
		{
			if(Input.GetButtonDown(Axes.toStr[PlayerStart[0]]))
			{
				DLog("START GAME");
				//Go to the next scene according to the amount of players
				switch(ActivePlayersCheck())
				{
					case 1: DLog("There's only 1 player"); break;
					case 2: SceneManager.LoadScene("TwoPlayers"); break;
					case 3: SceneManager.LoadScene("ThreePlayers"); break;
					case 4: SceneManager.LoadScene("FourPlayers"); break;
				}
			}
		}
		
    }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:

	///<Summary>
	/// Check how many active players are in the scene
	///</Summary>
	private int ActivePlayersCheck()
	{
		int players = 1;
		foreach(bool joinedPlayers in activePlayers)
		{
			if(joinedPlayers)
				players++;
		}
		return players;
	}
	private void BackButton(bool back, int playerNum, int teamPos)
	{
		
		if(back)
		{
			// //Start hold timer
			backDelay[playerNum] = Time.time + holdDownButtonDelay;
			// //Check if player has a team selected
			// if(teamSelected[playerNum])
			// {
			// 	//Remove the start game button if it's active
			// 	if(StartButton.activeSelf)
			// 	{
			// 		SelectTeamText.SetActive(true);
			// 		StartButton.SetActive(false);
			// 	}
			// 	//Remove player from team
            //     playersInTeam[teamPos]--;
				
			// 	//take out player team boolean (so no start button)
			// 	teamSelected[playerNum] = false;
			// }
			if(currentCategory[playerNum] > 0)
				currentCategory[playerNum]--;
			else
				loadOutSelected[playerNum] = false; //Player retracted his loadout state
			switch(currentCategory[playerNum])
			{
				case 0: //Characters
					selector(CharacterLocation, playerNum, currentPos[playerNum]);
					break;
				case 1: //Umbrellas
					selector(UmbrellaLocation, playerNum, currentPos[playerNum]);
					break;
				case 2://Weapons
					selector(WeaponLocation, playerNum, currentPos[playerNum]);
					break;
			}
		}
		// Return to the previous scene
	}
	private void BackReturn(bool backReturn, int playerNum)
	{
		if(backReturn && Time.time >= backDelay[playerNum]) //holdown button instead of click
		{
			SceneManager.LoadScene("MainMenu");
		}
	}

	///<Summary>
	/// Get the corners of the desired object with colliders
	///</Summary>
	private Vector3[] GetCornerLocation(Transform[] location, int position)
	{
		BoxCollider2D col = location[position].GetComponent<BoxCollider2D>();
		Vector3 center = col.bounds.center;
        		
		Bounds bounds = new Bounds(center, col.bounds.size);
		Vector3[] corners = Utility.CornerPoints(bounds);
		//DLog("Corner 1: " + corners[0]); //Top right
		//DLog("Corner 2: " + corners[1]); //Bottom Left
		//DLog("Corner 3: " + corners[2]); //Top Left
		//DLog("Corner 4: " + corners[3]); //bottom right
		return corners;

	}
	/// <Summary>
	/// Gets player input and move the "white block"(player selected team)
	/// <param name="location"> Which category location is being used</param>
	/// <param name="playerNum"> Which player is applying input</param>
	/// </Summary>
	private void selector(Transform[] location, int playerNum, int position)
	{
		switch(playerNum)
		{
			case 0: //Player 1
				PlayerSelector[playerNum].transform.position = GetCornerLocation(location, position)[2]; //Top left
				break;
			case 1: //Player 2
				PlayerSelector[playerNum].transform.position = GetCornerLocation(location, position)[0]; //Top Right
				break;
			case 2: //Player 3
				PlayerSelector[playerNum].transform.position = GetCornerLocation(location, position)[1]; //Bottom Left
				break;
			case 3: //Player 4
				PlayerSelector[playerNum].transform.position = GetCornerLocation(location, position)[3]; //Bottom right
				break;
		}
		
	}
	/// <Summary>
	/// Gets player input and move the "white block"(player selected team)
	/// <param name="horizontal"> Horziontal input</param>
	/// <param name="playerNum"> Which player is applying input</param>
	/// </Summary>
	private void GetPlayerInput(float horizontal, int playerNum, float vertical)
	{
		//Get player's input and check if he/she hasn't selected a team
		if(horizontal > 0)
		{
			//Move the selector left or right depending on the position.
			if(currentPos[playerNum] >= 0 && currentPos[playerNum] < 5 && Time.time >= delay[playerNum] 
				&& currentCategory[playerNum] < 3)
			{
				//Increase position of the array. ->
				//Player 1 starts at 0, 2 at 1, and so on.
				currentPos[playerNum]++;

				//Check for the category that the player is in
				switch(currentCategory[playerNum])
				{
					case 0: //Characters
						selector(CharacterLocation, playerNum, currentPos[playerNum]);
						break;
					case 1: //Umbrellas
						selector(UmbrellaLocation, playerNum, currentPos[playerNum]);
						break;
					case 2://Weapons
						selector(WeaponLocation, playerNum, currentPos[playerNum]);
						break;
				}
															
				
				//Add a delay to movement so that it wouldn't be instant
				delay[playerNum] = Time.time + tempDelay;
			}
			else if(currentCategory[playerNum] == 3)//If player is in the ultimate category
			{
				DLog("Ultimate selection Forward");
				DLog("Forward: " + currentPos[playerNum]);
				if(currentPos[playerNum] >= 0 && currentPos[playerNum] < 2 && Time.time >= delay[playerNum])
				{
					currentPos[playerNum]++;
					selector(UltimateLocation, playerNum, currentPos[playerNum]);

					delay[playerNum] = Time.time + tempDelay;
				}
			}
		}
		else if(horizontal < 0) //Left side
		{
			if(currentPos[playerNum] >0 && currentPos[playerNum] <=5 && Time.time >= delay[playerNum]
				&& currentCategory[playerNum] < 3)
			{
				//Decrease position of the array. <-
				currentPos[playerNum]--;
				switch(currentCategory[playerNum])
				{
					case 0: //Characters
						selector(CharacterLocation, playerNum, currentPos[playerNum]);
						break;
					case 1: //Umbrellas
						selector(UmbrellaLocation, playerNum, currentPos[playerNum]);
						break;
					case 2://Weapons
						selector(WeaponLocation, playerNum, currentPos[playerNum]);
						break;
				}
				
				
				delay[playerNum] = Time.time + tempDelay;
			}
			else if(currentCategory[playerNum] == 3)
			{
				DLog("Ultimate selection Back");
				DLog("Backwards: " + currentPos[playerNum]);
				if(currentPos[playerNum] >0 && currentPos[playerNum] <=3 && Time.time >= delay[playerNum])
				{
					//Decrease position of the array. <-
					currentPos[playerNum]--;
					selector(UltimateLocation, playerNum, currentPos[playerNum]);
					
					delay[playerNum] = Time.time + tempDelay;
				}
			}
		}

		//Vertical usage
		// if(vertical > 0)
		// {
		// 	if(teamPos[playerNum] >= 0 && teamPos[playerNum] < 4 && Time.time >= delay[playerNum])
		// 	{
		// 		DLog("Teampos down");
		// 		teamPos[playerNum]++;

		// 		delay[playerNum] = Time.time + tempDelay;
		// 	}
		// }
		// else if(vertical < 0 )
		// {
		// 	if(teamPos[playerNum] > 0 && teamPos[playerNum] <= 4 && Time.time >= delay[playerNum])
		// 	{
		// 		DLog("Teampos up");
		// 		teamPos[playerNum]--;

		// 		delay[playerNum] = Time.time + tempDelay;
		// 	}
			
		// }

		
		//Change the color of the player selector and add it to the team on the Player.cs
		switch(teamPos[playerNum])
		{
			case 0: 
				PlayerSelector[playerNum].GetComponent<Image>().color = Color.red;
				break;
			case 1: 
				PlayerSelector[playerNum].GetComponent<Image>().color = Color.cyan;
				break;
			case 2: 
				PlayerSelector[playerNum].GetComponent<Image>().color = Color.green;
				break;
			case 3: 
				PlayerSelector[playerNum].GetComponent<Image>().color = Color.yellow;
				break;
		}
			
		
		SelectCategory(Input.GetButtonDown(Axes.toStr[PlayerSubmit[playerNum]]), playerNum);
		SelectTeam(Input.GetButtonDown(Axes.toStr[PlayerChooseTeam[playerNum]]), playerNum);
	}

	

	/// <Summary>
	/// Get all the inputs from all playing controllers at the same time.
	/// Apply the input to their respective location
	/// For now, only allow left/right and a and b(hold) to go back
	/// Update deals with player 1.
	/// in other words, imagine this as 3 Updates functions
	/// </Summary>
	private IEnumerator Player2Inputs()
	{
		GetPlayerInput(Axes.GetAxis(PlayerHorizontal[1]), 1, Axes.GetAxis(PlayerVertical[1]));
		BackButton(Input.GetButtonDown(Axes.toStr[PlayerReturn[1]]), 1, currentPos[1]);
		BackReturn(Input.GetButton(Axes.toStr[PlayerReturn[1]]), 1);
		yield return null;	
	}
	private IEnumerator Player3Inputs()
	{
		GetPlayerInput(Axes.GetAxis(PlayerHorizontal[2]), 2, Axes.GetAxis(PlayerVertical[2]));
		BackButton(Input.GetButtonDown(Axes.toStr[PlayerReturn[2]]), 2, currentPos[2]);
		BackReturn(Input.GetButton(Axes.toStr[PlayerReturn[2]]), 2);
		yield return null;	
	}
	private IEnumerator Player4Inputs()
	{
		GetPlayerInput(Axes.GetAxis(PlayerHorizontal[3]), 3, Axes.GetAxis(PlayerVertical[3]));
		BackButton(Input.GetButtonDown(Axes.toStr[PlayerReturn[3]]), 3, currentPos[3]);
		BackReturn(Input.GetButton(Axes.toStr[PlayerReturn[3]]), 3);
		yield return null;	
	}

	private void SelectCategory(bool select, int playerNum)
	{
		if(select)
		{
			//TODO
			//Option selected = load that option to the player controller or something like that

			//Go to the next category for the player
			if(currentCategory[playerNum] < 3)
			{
				currentCategory[playerNum]++;
				switch(currentCategory[playerNum])
				{
					case 0: //Characters
						selector(CharacterLocation, playerNum, 0);
						break;
					case 1: //Umbrellas
						selector(UmbrellaLocation, playerNum, 0);
						break;
					case 2://Weapons
						selector(WeaponLocation, playerNum, 0);
						break;
					case 3://Weapons
						selector(UltimateLocation, playerNum, 0);
						break;
				}
				currentPos[playerNum] = 0;
			}
			else //Ultimate was selected
			{
				loadOutSelected[playerNum] = true; //Player finished selecting
				//Create start game button
				StartGameAvailable(playerNum);
			}
		}
	}
	///<Summary>
	/// Allows players to select their team by the color
	/// If it is Team death match, then multiple of the same color can play
	/// 	Check for max players, all players can't be the same color
	/// If it is Free for all, the same color can't be chosen twice.
	///</Summary>
	private void SelectTeam(bool select, int playerNum)
	{
		if(select)
		{
			//After clicking X/Square, check all the teams that the players are in
			//Add the players into the team
			if(teamPos[playerNum] >= 0 && teamPos[playerNum] < 4 && Time.time >= delay[playerNum])
			{
				DLog("Teampos Up");
				DLog("Player: " + playerNum + " clicked: " + select);
				playersInTeam[teamPos[playerNum]]++;
				teamPos[playerNum]++;
				//player[playerNum].Team = (Team)teamPos;
				//PlayerSelector[playerNum].GetComponent<Image>().color = Color.white;
	
				delay[playerNum] = Time.time + tempDelay;
			}
			else if(teamPos[playerNum] >= 4 && Time.time >= delay[playerNum])
			{
				DLog("Teampos Reset");
				DLog("Reset = Player: " + playerNum + " clicked: " + select);
				teamPos[playerNum] = 0;
				playersInTeam[teamPos[playerNum]] = 0;
				
				
				delay[playerNum] = Time.time + tempDelay;
			}
        }
	}

	///<Summary>
	/// Spawn a Start button
	/// Allow any player to start the game. 
	/// If any player presses back, then make sure to make it disappear.
	///</Summary>
	private void StartGameAvailable(int playerNum)
	{
		//If all the players are in the same team
		if(teamPos[playerNum] > ActivePlayersCheck() - 1)
		{
			DLog("Full party");
		}
		else
		{
			for(int i = 0; i < ActivePlayersCheck(); i++)
			{
				if(loadOutSelected[i])
					startGame = true;
				else
				{
					startGame = false;
					break;
				}
			}

			if(startGame)
			{
				SelectTeamText.SetActive(false);
				StartButton.SetActive(true);
			}
		}
	}

    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );         
    }
}