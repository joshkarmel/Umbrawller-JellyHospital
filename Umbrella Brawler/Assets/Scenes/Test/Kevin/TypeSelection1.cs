using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Enums;
using UnityEngine.EventSystems;

public class TypeSelection1 : MonoBehaviour
{
    //------------------------------------------------------------------------CONSTANTS:

    private const string LOG_TAG = "TypeSelection";
    public bool VERBOSE = false;
    private const int PLAYER_1 = (int)Player.Player1;
    private const int PLAYER_2 = (int)Player.Player2;
    private const int PLAYER_3 = (int)Player.Player3;
    private const int PLAYER_4 = (int)Player.Player4;

    private readonly string[] CATEGORIES = { "Character", "Team", "Gun", "Umbrella", "Ultimate" };
    private readonly string[] ANIMATION_TRIGGERS_DOWN = { "CharacterToTeam", "TeamToGun", "GunToUmbrella", "UmbrellaToUlt" };
    private readonly string[] ANIMATION_TRIGGERS_UP = { "TeamToCharacter", "GunToTeam", "UmbrellaToGun", "UltToUmbrella" };

    // TEMPORARY UNTIL I FIGURE THINGS OUT
    private readonly List<List<string>> CHOICES = new List<List<string>>(){  new List<string>() {"Characters"},
                                            new List<string>(){"Choose your team", "Team 1", "Team 2", "Team 3", "Team 4"},
                                            new List<string>(){ "Choose your gun", "Shotgun", "Dragon Breath"},
                                            new List<string>(){ "Choose your umbrella", "Umbrella 1"},
                                            new List<string>(){ "Choose your ultimate", "Broken Engine"}};


    //---------------------------------------------------------------------------FIELDS:
    //public Transform[] CharacterLocation, UmbrellaLocation, WeaponLocation, UltimateLocation;
	public Axes.Action[] PlayerHorizontal, PlayerVertical, PlayerSubmit, PlayerReturn, PlayerStart;
    public GameObject[] InitialJoinTexts, JoinedPanels;
    public GameObject StartButton;
    public Text[] Player1Text, Player2Text, Player3Text, Player4Text;

    private int teamVal;
	private Canvas canvas;
    //private int[] currentPos, playersInTeam, currentCategory, teamPos;
    private float[] joystickDelay, backDelay, selectDelay, deselectDelay;
	private float tempDelay, holdDownButtonDelay;
	private bool[] teamSelected, activePlayers, loadOutSelected;
    private bool readyToStart = false;

    // mine below

    private int[] verticalPosition = { 0, 0, 0, 0 };
    private List<Text[]> playerGrids = new List<Text[]>();


    // Player Number x Vertical Category
    private int[,] horizontalPositionGrid = {{ 0, 0, 0, 0, 0},
                                             { 0, 0, 0, 0, 0},
                                             { 0, 0, 0, 0, 0},
                                             { 0, 0, 0, 0, 0}};

    private Dictionary<Player, List<bool>> lockedIn = new Dictionary<Player, List<bool>>();
    private Dictionary<Player, PlayerManager> players = new Dictionary<Player, PlayerManager>();
    private Dictionary<Team, int> playersInTeam = new Dictionary<Team, int>();
    private MapObjectSetup mapObjectSetup = new MapObjectSetup();
    //---------------------------------------------------------------------MONO METHODS:

    void Start() 
	{
		tempDelay = 0.2f;
		holdDownButtonDelay = 2;
		canvas = FindObjectOfType<Canvas>();
		backDelay = new float[4];
        deselectDelay = new float[4];
        selectDelay = new float[4];
		activePlayers = new bool[4];
		teamSelected = new bool[4];
		joystickDelay = new float[4];
		loadOutSelected = new bool[4];
        playerGrids.Add(Player1Text);
        playerGrids.Add(Player2Text);

		for(int i = 0; i< 4; i++)
		{
			teamSelected[i] = false;
			joystickDelay[i] = tempDelay;
            selectDelay[i] = tempDelay;
            deselectDelay[i] = tempDelay;
            backDelay[i] = holdDownButtonDelay;
			loadOutSelected[i] = false;
            playersInTeam[(Team)i] = 0;
		}
	}
	
	void Update()
    {
		if (!activePlayers[PLAYER_1] && Input.GetButtonDown(Axes.toStr[PlayerStart[PLAYER_1]]))
        {
            HandlePlayerJoined(Player.Player1);
        }
        else if (!activePlayers[PLAYER_2] && Input.GetButtonDown(Axes.toStr[PlayerStart[PLAYER_2]]))
		{
            HandlePlayerJoined(Player.Player2);
        }
        else if (!activePlayers[PLAYER_3] && Input.GetButtonDown(Axes.toStr[PlayerStart[PLAYER_3]]))
        {
            HandlePlayerJoined(Player.Player3);
        }
        else if (!activePlayers[PLAYER_4] && Input.GetButtonDown(Axes.toStr[PlayerStart[PLAYER_4]]))
        {
            HandlePlayerJoined(Player.Player4);
        }


        if (activePlayers[PLAYER_1]) StartCoroutine(CheckPlayerInputs(PLAYER_1));
        
        if (activePlayers[PLAYER_2]) StartCoroutine(CheckPlayerInputs(PLAYER_2));
        
        if (activePlayers[PLAYER_3]) StartCoroutine(CheckPlayerInputs(PLAYER_3));
        
        if (activePlayers[PLAYER_4]) StartCoroutine(CheckPlayerInputs(PLAYER_4));


        readyToStart = AllPlayersReady();

        if (readyToStart)
        {
            StartButton.SetActive(true);
            //EventSystem.current.SetSelectedGameObject(StartButton);
        }
        else
        {
            StartButton.SetActive(false);
        }

        //-----------------------
  //      if (startGame)
		//{
		//	if(Input.GetButtonDown(Axes.toStr[PlayerStart[0]]))
		//	{
		//		DLog("START GAME");
		//		//Go to the next scene according to the amount of players
		//		switch(ActivePlayersCheck())
		//		{
		//			case 1: DLog("There's only 1 player"); break;
		//			case 2: SceneManager.LoadScene("TwoPlayers"); break;
		//			case 3: SceneManager.LoadScene("ThreePlayers"); break;
		//			case 4: SceneManager.LoadScene("FourPlayers"); break;
		//		}
		//	}
		//}
		
    }

//--------------------------------------------------------------------------METHODS:


    public void StartGame()
    {

        GameManager.Instance.Level = Level.Static;
        List<PlayerManager> tempplayers = players.Values.ToList();
        DLog("Starting Game");
        mapObjectSetup.GetObjectsReadyForSceneChange(players.Count, this.gameObject.scene.name, true, "Player Prefab", tempplayers);
    }

//--------------------------------------------------------------------------HELPERS:

    /// <Summary>
    /// Get all the inputs from all playing controllers at the same time.
    /// Apply the input to their respective location
    /// For now, only allow left/right and a and b(hold) to go back
    /// Update deals with player 1.
    /// in other words, imagine this as 3 Updates functions
    /// </Summary>
    private IEnumerator CheckPlayerInputs(int playerNum)
    {
        if (!readyToStart)
            // Movement
            GetJoystickInput(playerNum);

        // Selection
        GetSelectInput(playerNum);
        

        // Deselection
        GetReturnInput(playerNum);

        yield return null;
    }


    /// <Summary>
    /// Gets player joystick movement, i.e. navigate through the categories
    /// </Summary>
    private void GetJoystickInput(int playerNum)
	{
        float vertical = Axes.GetAxis(PlayerVertical[playerNum]);
        float horizontal = Axes.GetAxis(PlayerHorizontal[playerNum]);

        // Up
        if (vertical > 0.5  && Time.time >= joystickDelay[playerNum])
        {
            if (GetVerticalPosition(playerNum) > 0)
            {
                verticalPosition[playerNum]--;
                JoinedPanels[playerNum].GetComponent<Animator>().SetTrigger(ANIMATION_TRIGGERS_UP[GetVerticalPosition(playerNum)]);
                ResetJoystickDelay(playerNum);
            }
        }

        // Down
        else if (vertical < -0.5 && Time.time >= joystickDelay[playerNum])
        {
            if (GetVerticalPosition(playerNum) < ANIMATION_TRIGGERS_DOWN.Length)
            {
                JoinedPanels[playerNum].GetComponent<Animator>().SetTrigger(ANIMATION_TRIGGERS_DOWN[GetVerticalPosition(playerNum)]);
                verticalPosition[playerNum]++;
                ResetJoystickDelay(playerNum);
            }
        }

        // Right
        else if (horizontal > 0.5 && !IsCurrentCategoryLockedIn(playerNum) && Time.time >= joystickDelay[playerNum])
        {
            if (GetHorizontalPosition(playerNum) < CHOICES[GetVerticalPosition(playerNum)].Count - 1)
            {
                if (GetVerticalPosition(playerNum) > 0)
                {
                    horizontalPositionGrid[playerNum, verticalPosition[playerNum]]++;
                    playerGrids.ElementAt(playerNum)[GetVerticalPosition(playerNum) - 1].text 
                        = CHOICES[GetVerticalPosition(playerNum)][GetHorizontalPosition(playerNum)];
                    ResetJoystickDelay(playerNum);
                }
            }
        }

        // Left
        else if (horizontal < -0.5 && !IsCurrentCategoryLockedIn(playerNum) && Time.time >= joystickDelay[playerNum]) 
        {
            if (GetHorizontalPosition(playerNum) > 0)
            {
                if (GetVerticalPosition(playerNum) > 0)
                {
                    horizontalPositionGrid[playerNum, verticalPosition[playerNum]]--;
                    playerGrids.ElementAt(playerNum)[GetVerticalPosition(playerNum) - 1].text 
                        = CHOICES[GetVerticalPosition(playerNum)][GetHorizontalPosition(playerNum)];
                    ResetJoystickDelay(playerNum);
                }
            }
        }
    }

    /// <Summary>
    /// Locks in the player's choice for the selected category
    /// </Summary>
    private void GetSelectInput(int playerNum)
    {
        if (Input.GetButtonDown(Axes.toStr[PlayerSubmit[playerNum]]) && Time.time >= selectDelay[playerNum])
        {
            if (!readyToStart && (GetVerticalPosition(playerNum) == 0 || (GetVerticalPosition(playerNum) != 0 && GetHorizontalPosition(playerNum) != 0)))
            {
                switch(GetVerticalPosition(playerNum))
                {
                    case 0: // Character
                        lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)] = true;
                        JoinedPanels[playerNum].transform.Find(CATEGORIES[GetVerticalPosition(playerNum)]).Find("Selected").gameObject.SetActive(true);
                        break;
                    case 1: // Team
                        Team selectedTeam = (Team)GetHorizontalPosition(playerNum) - 1;

                        // Check if there's room on the team
                        if (playersInTeam[selectedTeam] != players.Count - 1 || players.Count == 1) 
                        {
                            playersInTeam[selectedTeam]++;
                            players[(Player)playerNum].Team = (Team)GetHorizontalPosition(playerNum) - 1;
                            lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)] = true;
                            JoinedPanels[playerNum].transform.Find(CATEGORIES[GetVerticalPosition(playerNum)]).Find("Selected").gameObject.SetActive(true);
                        }
                        break;
                    case 2: // Gun
                        lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)] = true;
                        JoinedPanels[playerNum].transform.Find(CATEGORIES[GetVerticalPosition(playerNum)]).Find("Selected").gameObject.SetActive(true);
                        break;
                    case 3: // Umbrella
                        lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)] = true;
                        JoinedPanels[playerNum].transform.Find(CATEGORIES[GetVerticalPosition(playerNum)]).Find("Selected").gameObject.SetActive(true);
                        break;
                    case 4: // Ultimate
                        lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)] = true;
                        JoinedPanels[playerNum].transform.Find(CATEGORIES[GetVerticalPosition(playerNum)]).Find("Selected").gameObject.SetActive(true);
                        break;
                }

                ResetSelectDelay(playerNum);
            }
            else if (readyToStart && playerNum == PLAYER_1)
            {
                StartGame();
            }
        }
    }

    /// <Summary>
    /// Deselects the player's choice for the selected category
    /// </Summary>
    private void GetReturnInput(int playerNum)
    {
        if (Input.GetButtonDown(Axes.toStr[PlayerReturn[playerNum]]) && Time.time >= deselectDelay[playerNum])
        {
            if (GetVerticalPosition(playerNum) == 0 || (GetVerticalPosition(playerNum) != 0 && GetHorizontalPosition(playerNum) != 0))
            {
                switch (GetVerticalPosition(playerNum))
                {
                    case 0: // Character
                        break;
                    case 1: // Team
                        if (IsCurrentCategoryLockedIn(playerNum))
                        {
                            Team selectedTeam = (Team)GetHorizontalPosition(playerNum) - 1;
                            playersInTeam[selectedTeam]--;
                        }
                        break;
                    case 2: // Gun
                        break;
                    case 3: // Umbrella
                        break;
                    case 4: // Ultimate
                        break;
                }

                lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)] = false;
                JoinedPanels[playerNum].transform.Find(CATEGORIES[GetVerticalPosition(playerNum)]).Find("Selected").gameObject.SetActive(false);
                ResetDeselectDelay(playerNum);
            }
        }
    }

    /// <Summary>
    /// Sets up variables and UI elements when a player joins
    /// </Summary>
    private void HandlePlayerJoined(Player player)
    {
        players[player] = new PlayerManager();
        players[player].PlayerNumber = player;

        activePlayers[(int)player] = true;
        InitialJoinTexts[(int)player].SetActive(false);
        JoinedPanels[(int)player].SetActive(true);

        lockedIn[player] = new List<bool> { false, false, false, false, false };   
    }

    /// <Summary>
    /// Returns the vertical position (category) of the respective player
    /// </Summary>
    private int GetVerticalPosition(int playerNum)
    {
        return verticalPosition[playerNum];
    }

    /// <Summary>
    /// Returns the horizontal position (option) of the respective player
    /// </Summary>
    private int GetHorizontalPosition(int playerNum)
    {
        return horizontalPositionGrid[playerNum, verticalPosition[playerNum]];
    }

    /// <Summary>
    /// Returns true if the category that the player is currently on is locked in
    /// </Summary>
    private bool IsCurrentCategoryLockedIn(int playerNum)
    {
        return lockedIn[(Player)playerNum][GetVerticalPosition(playerNum)];
    }

    /// <Summary>
    /// Resets joystick delay
    /// </Summary>
    private void ResetJoystickDelay(int playerNum)
    {
        joystickDelay[playerNum] = Time.time + tempDelay;
    }

    /// <Summary>
    /// Resets select delay
    /// </Summary>
    private void ResetSelectDelay(int playerNum)
    {
        selectDelay[playerNum] = Time.time + tempDelay;
    }

    /// <Summary>
    /// Resets deselect delay
    /// </Summary>
    private void ResetDeselectDelay(int playerNum)
    {
        deselectDelay[playerNum] = Time.time + tempDelay;
    }

    /// <Summary>
    /// Returns true if all players are fully locked in
    /// </Summary>
    private bool AllPlayersReady()
    {
        if (players.Count < 2) return false;

        foreach (Player key in lockedIn.Keys)
        {
            if(lockedIn[key].Exists(x => x == false))
            {
                return false;
            }
        }

        return true;
    }

    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );         
    }
}