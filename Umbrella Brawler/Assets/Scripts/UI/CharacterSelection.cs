using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Enums;
using UnityEngine.EventSystems;
using System;

public class CharacterSelection : MonoBehaviour
{
    //------------------------------------------------------------------------CONSTANTS:

    protected const string LOG_TAG = "TypeSelection";
    public bool VERBOSE = false;
    public bool KEYBOARD_INPUT = false;
    protected const int CONTROLLER_1 = (int)Controller.Controller1;
    protected const int CONTROLLER_2 = (int)Controller.Controller2;
    protected const int CONTROLLER_3 = (int)Controller.Controller3;
    protected const int CONTROLLER_4 = (int)Controller.Controller4;

    protected readonly string[] CATEGORIES = { "Character", "Team", "Gun", "Umbrella", "Ultimate" };
    protected readonly string[] ANIMATION_TRIGGERS_DOWN = { "CharacterToTeam", "TeamToGun", "GunToUmbrella", "UmbrellaToUlt" };
    protected readonly string[] ANIMATION_TRIGGERS_UP = { "TeamToCharacter", "GunToTeam", "UmbrellaToGun", "UltToUmbrella" };

    protected readonly List<IList> CHOICES = new List<IList>()
                                            {
                                                Enum.GetValues(typeof(Character)),//new List<string>() {"Characters"},
                                                Enum.GetValues(typeof(Team)),
                                                Enum.GetValues(typeof(Weapons)),
                                                Enum.GetValues(typeof(Umbrellas)),
                                                Enum.GetValues(typeof(Ultimates))
                                            };


    protected Dictionary<int, IList> EricIsAlsoGenius;
    
    [Header("Inputs for Players 1-4")]
    public Axes.Action[] PlayerHorizontal, PlayerVertical, PlayerSubmit, PlayerReturn, PlayerStart;
    public GameObject[] InitialJoinTexts, JoinedPanels;
    public GameObject StartButton;

    [Tooltip("Screen that displays when you try to exit back to the main menu")]
    public GameObject ReturnConfirmation;

    [Tooltip("Mapping to Player 1 X button to exit back to the Main Menu")]
    public Axes.Action ExitButton;

    [Header("UI Audio Clips")]
    public AudioClip bgm;
    public AudioClip navFx;
    public AudioClip selectFx;
    public AudioClip cancelFx;


    protected int MinPlayers = 2;
    protected Dictionary<Controller, PlayerManager> players = new Dictionary<Controller, PlayerManager>();
    protected MapObjectSetup mapObjectSetup = new MapObjectSetup();

    protected int teamVal;
    protected Canvas canvas;
    protected float[] joystickDelay, backDelay, selectDelay, deselectDelay;
    protected float tempDelay, holdDownButtonDelay;
    protected bool[] teamSelected, activePlayers, loadOutSelected;
    protected bool readyToStart = false;

    protected int[] verticalPosition = { 0, 0, 0, 0 };
    // Player Number x Vertical Category
    protected int[,] horizontalPositionGrid = {{ 0, 0, 0, 0, 0},
                                             { 0, 0, 0, 0, 0},
                                             { 0, 0, 0, 0, 0},
                                             { 0, 0, 0, 0, 0}};

    protected List<List<bool>> lockedIn = new List<List<bool>>();
    protected Dictionary<Team, int> playersInTeam = new Dictionary<Team, int>();
    protected Dictionary<Controller, int> controllerToPanelIndex = new Dictionary<Controller, int>();
    protected int playerCount = 0;
    
    //---------------------------------------------------------------------MONO METHODS:

    public void Start()
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
        SoundManager.PlayBGM(bgm);

        for (int i = 0; i < 4; i++)
        {
            teamSelected[i] = false;
            joystickDelay[i] = tempDelay;
            selectDelay[i] = tempDelay;
            deselectDelay[i] = tempDelay;
            backDelay[i] = holdDownButtonDelay;
            loadOutSelected[i] = false;
            playersInTeam[(Team)i + 1] = 0;
        }
#if UNITY_EDITOR
        if (KEYBOARD_INPUT)
        {
            PlayerHorizontal[0] = Axes.Action.Horizontal;
            PlayerVertical[0] = Axes.Action.Vertical;
            PlayerReturn[0] = Axes.Action.Escape;
            PlayerSubmit[0] = Axes.Action.Jump;
            PlayerStart[0] = Axes.Action.Shield;
        }
#endif
    }

    public void Update()
    {
        if (Input.GetButtonDown(Axes.toStr[ExitButton]))
        {
            ReturnConfirmation.SetActive(true);
        }

        // Normal Inputs
        if (!ReturnConfirmation.activeSelf)
        {
            if (!activePlayers[CONTROLLER_1] && Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_1]]))
            {
                HandlePlayerJoined(Controller.Controller1);
            }
            else if (!activePlayers[CONTROLLER_2] && Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_2]]))
            {
                HandlePlayerJoined(Controller.Controller2);
            }
            else if (!activePlayers[CONTROLLER_3] && Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_3]]))
            {
                HandlePlayerJoined(Controller.Controller3);
            }
            else if (!activePlayers[CONTROLLER_4] && Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_4]]))
            {
                HandlePlayerJoined(Controller.Controller4);
            }


            if (activePlayers[CONTROLLER_1]) StartCoroutine(HandlePlayerInputs(Controller.Controller1));

            if (activePlayers[CONTROLLER_2]) StartCoroutine(HandlePlayerInputs(Controller.Controller2));

            if (activePlayers[CONTROLLER_3]) StartCoroutine(HandlePlayerInputs(Controller.Controller3));

            if (activePlayers[CONTROLLER_4]) StartCoroutine(HandlePlayerInputs(Controller.Controller4));

            readyToStart = AreAllPlayersReady();
            StartButton.SetActive(readyToStart);

            if (readyToStart)
            {
                if (Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_1]]) ||
                    Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_2]]) ||
                    Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_3]]) ||
                    Input.GetButtonDown(Axes.toStr[PlayerStart[CONTROLLER_4]]))
                {
                    StartGame();
                }
            }
        }
        // Yes/No to return to Main Menu
        else
        {
            if (Input.GetButtonUp(Axes.toStr[PlayerSubmit[CONTROLLER_1]])) // A
            {
                SceneManager.LoadScene("MainMenu");
                SoundManager.StopBGM();
            }
            else if (Input.GetButtonUp(Axes.toStr[PlayerReturn[CONTROLLER_1]])) // B
            {
                ReturnConfirmation.SetActive(false);
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.P) && players.Keys.Count < MinPlayers)
        {
            if (!lockedIn[0].Exists(x => x == false))
            {
                HandlePlayerJoined(Controller.Controller2);
                players[Controller.Controller2].Team = Team.Team2;
                players[Controller.Controller2].Weapon = Weapons.Shotgun;
                players[Controller.Controller2].Umbrella = Umbrellas.Umbrella;
                players[Controller.Controller2].Ultimate = Ultimates.BrokenEngine;
                StartGame();
            }
        }

         if (Input.GetKeyUp(KeyCode.A) && players.Keys.Count < MinPlayers)
        {
            if (!lockedIn[0].Exists(x => x == false))
            {
                HandlePlayerJoined(Controller.Controller2);
                players[Controller.Controller2].Team = Team.Team2;
                players[Controller.Controller2].Weapon = Weapons.Shotgun;
                players[Controller.Controller2].Umbrella = Umbrellas.Umbrella;
                players[Controller.Controller2].Ultimate = Ultimates.BrokenEngine;
                players[Controller.Controller2].IsAIPlayer = true;
                GameManager.Instance.m_AIOn = true;
                
                StartGame();
            }
        }
#endif
    }


    //--------------------------------------------------------------------------METHODS:


    public virtual void StartGame()
    {
        List<PlayerManager> tempplayers = players.Values.ToList();
        DLog("Starting Game");
        mapObjectSetup.GetObjectsReadyForSceneChange(players.Count, this.gameObject.scene.name, true, "Player Prefab", tempplayers);
        SoundManager.StopBGM();
    }

    /// <Summary>
    /// Sets up variables and UI elements when a player joins
    /// </Summary>
    protected void HandlePlayerJoined(Controller controller)
    {
        players[controller] = new PlayerManager();
        players[controller].PlayerNumber = (Player)playerCount;
        players[controller].ControllerNumber = controller;

        activePlayers[(int)controller] = true;
        InitialJoinTexts[playerCount].SetActive(false);
        JoinedPanels[playerCount].SetActive(true);

        lockedIn.Add(new List<bool> { false, false, false, false, false });
        controllerToPanelIndex[controller] = playerCount;

        playerCount++;
    }

    /// <Summary>
    /// Get all the inputs from all playing controllers at the same time.
    /// Apply the input to their respective location
    /// For now, only allow left/right and a and b(hold) to go back
    /// Update deals with player 1.
    /// in other words, imagine this as 3 Updates functions
    /// </Summary>
    protected virtual IEnumerator HandlePlayerInputs(Controller controllerNum)
    {
        if (!readyToStart)
            // Movement
            GetJoystickInput(controllerNum);

        // Selection
        GetSelectInput(controllerNum);
        
        // Deselection
        GetReturnInput(controllerNum);

        yield return null;
    }


    /// <Summary>
    /// Gets player joystick movement, i.e. navigate through the categories
    /// </Summary>
    protected void GetJoystickInput(Controller controller)
    {
        // Get the correct controller's input based on the player
        float vertical = Axes.GetAxis(PlayerVertical[(int)controller]);
        float horizontal = Axes.GetAxis(PlayerHorizontal[(int)controller]);

        // Get the panel index that this player is mapped to
        int panelNumber = GetPanelIndex(controller);

        // Up
        if (vertical > 0.5 && Time.time >= joystickDelay[panelNumber])
        {
            if (GetVerticalPosition(panelNumber) > 0)
            {
                verticalPosition[panelNumber]--;
                JoinedPanels[panelNumber].GetComponent<Animator>().SetTrigger(ANIMATION_TRIGGERS_UP[GetVerticalPosition(panelNumber)]);
                SoundManager.PlaySFX(navFx, false, 1f);
                ResetJoystickDelay(panelNumber);
            }
        }

        // Down
        else if (vertical < -0.5 && Time.time >= joystickDelay[panelNumber])
        {
            if (GetVerticalPosition(panelNumber) < ANIMATION_TRIGGERS_DOWN.Length)
            {
                JoinedPanels[panelNumber].GetComponent<Animator>().SetTrigger(ANIMATION_TRIGGERS_DOWN[GetVerticalPosition(panelNumber)]);
                verticalPosition[panelNumber]++;
                SoundManager.PlaySFX(navFx, false, 1f);
                ResetJoystickDelay(panelNumber);
            }
        }

        // Right
        else if (horizontal > 0.5 && !IsCurrentCategoryLockedIn(panelNumber) && Time.time >= joystickDelay[panelNumber])
        {
            if (GetHorizontalPosition(panelNumber) < CHOICES[GetVerticalPosition(panelNumber)].Count - 1)
            {
                if (GetVerticalPosition(panelNumber) > 0)
                {
                    horizontalPositionGrid[panelNumber, verticalPosition[panelNumber]]++;
                    SoundManager.PlaySFX(navFx, false, 1f);

                    SetLeftArrowVisible(panelNumber, GetVerticalPosition(panelNumber), GetHorizontalPosition(panelNumber) != 0);
                    SetRightArrowVisible(panelNumber, GetVerticalPosition(panelNumber), GetHorizontalPosition(panelNumber) != CHOICES[GetVerticalPosition(panelNumber)].Count - 1);

                    JoinedPanels[panelNumber].transform
                        .Find(CATEGORIES[GetVerticalPosition(panelNumber)])
                        .Find("Text")
                        .GetComponent<Text>()
                        .text = Utility.GetEnumDescription((System.Enum)CHOICES[GetVerticalPosition(panelNumber)][GetHorizontalPosition(panelNumber)]);

                    ResetJoystickDelay(panelNumber);
                }
            }
        }

        // Left
        else if (horizontal < -0.5 && !IsCurrentCategoryLockedIn(panelNumber) && Time.time >= joystickDelay[panelNumber])
        {
            if (GetHorizontalPosition(panelNumber) > 0)
            {
                if (GetVerticalPosition(panelNumber) > 0)
                {
                    horizontalPositionGrid[panelNumber, verticalPosition[panelNumber]]--;
                    SoundManager.PlaySFX(navFx, false, 1f);

                    SetLeftArrowVisible(panelNumber, GetVerticalPosition(panelNumber), GetHorizontalPosition(panelNumber) != 0);
                    SetRightArrowVisible(panelNumber, GetVerticalPosition(panelNumber), GetHorizontalPosition(panelNumber) != CHOICES[GetVerticalPosition(panelNumber)].Count - 1);

                    JoinedPanels[panelNumber].transform
                        .Find(CATEGORIES[GetVerticalPosition(panelNumber)])
                        .Find("Text")
                        .GetComponent<Text>()
                        .text = Utility.GetEnumDescription((System.Enum)CHOICES[GetVerticalPosition(panelNumber)][GetHorizontalPosition(panelNumber)]);
                    ResetJoystickDelay(panelNumber);
                }
            }
        }
    }


    /// <Summary>
    /// Locks in the player's choices for each category
    /// </Summary>
    protected virtual void GetSelectInput(Controller controllerNum)
    {
        bool selectPressed = Input.GetButtonDown(Axes.toStr[PlayerSubmit[(int)controllerNum]]);
        int panelNumber = GetPanelIndex(controllerNum);

        bool readyToLockIn = true;

        if(selectPressed && Time.time >= selectDelay[panelNumber])
        {
            // Validations first
            for(int categoryIndex = 0; categoryIndex < CATEGORIES.Length; categoryIndex++)
            {
                System.Enum selected = GetChoice(categoryIndex, GetHorizontalPosition(panelNumber, categoryIndex));
                switch (CATEGORIES[categoryIndex])
                {
                    case "Team":
                        // Check if there's room on the team
                        if (GetHorizontalPosition(panelNumber, categoryIndex) == 0 
                            || (players.Count != 1 && playersInTeam[(Team)selected] == players.Count - 1))
                        {
                            readyToLockIn = false;
                            StartCoroutine(ErrorFlash(JoinedPanels[panelNumber].transform.Find(CATEGORIES[categoryIndex]).gameObject));
                        }
                        break;
                    case "Gun":
                    case "Umbrella":
                    case "Ultimate":
                        if (GetHorizontalPosition(panelNumber, categoryIndex) == 0)
                        {
                            readyToLockIn = false;
                            StartCoroutine(ErrorFlash(JoinedPanels[panelNumber].transform.Find(CATEGORIES[categoryIndex]).gameObject));
                        }
                        break;
                }
            }

            // Lock in all categories if we should
            if(readyToLockIn)
            {
                for (int categoryIndex = 0; categoryIndex < CATEGORIES.Length; categoryIndex++)
                {
                    System.Enum selected = GetChoice(categoryIndex, GetHorizontalPosition(panelNumber, categoryIndex));
                    switch (CATEGORIES[categoryIndex])
                    {
                        case "Team":
                            playersInTeam[(Team)selected]++;
                            players[controllerNum].Team = (Team)selected;
                            break;
                        case "Gun":
                            players[controllerNum].Weapon = (Weapons)selected;
                            break;
                        case "Umbrella":
                            players[controllerNum].Umbrella = (Umbrellas)selected;
                            break;
                        case "Ultimate":
                            players[controllerNum].Ultimate = (Ultimates)selected;
                            break;
                    }

                    lockedIn[panelNumber][categoryIndex] = true;
                    JoinedPanels[panelNumber].transform.Find(CATEGORIES[categoryIndex]).Find("Selected").gameObject.SetActive(true);
                    SetLeftArrowVisible(panelNumber, categoryIndex, false);
                    SetRightArrowVisible(panelNumber, categoryIndex, false);
                }
            }
            ResetSelectDelay(panelNumber);
        }

    }


    /// <Summary>
    /// Deselects the player's choice for the selected category
    /// </Summary>
    protected void GetReturnInput(Controller controllerNum)
    {
        bool returnPressed = Input.GetButtonDown(Axes.toStr[PlayerReturn[(int)controllerNum]]);
        int panelNumber = GetPanelIndex(controllerNum);

        if (returnPressed && Time.time >= deselectDelay[panelNumber])
        {
            //if (GetVerticalPosition(panelNumber) == 0 || (GetVerticalPosition(panelNumber) != 0 && GetHorizontalPosition(panelNumber) != 0))
            //{
            for (int categoryIndex = 0; categoryIndex < CATEGORIES.Length; categoryIndex++)
            {
                // Don't do anything if any categories are on the default "Choose your _____" option
                if (CATEGORIES[categoryIndex] == "Character" || (categoryIndex != 0 && GetHorizontalPosition(panelNumber, categoryIndex) != 0))
                {
                    if (CATEGORIES[categoryIndex] == "Team")
                    {
                        Team selectedTeam = (Team)GetChoice(categoryIndex, GetHorizontalPosition(panelNumber, categoryIndex));
                        playersInTeam[selectedTeam]--;
                    }

                    SetLeftArrowVisible(panelNumber, categoryIndex, GetHorizontalPosition(panelNumber, categoryIndex) != 0);
                    SetRightArrowVisible(panelNumber, categoryIndex, GetHorizontalPosition(panelNumber, categoryIndex) != CHOICES[GetVerticalPosition(panelNumber)].Count - 1);
                    lockedIn[panelNumber][categoryIndex] = false;
                    JoinedPanels[panelNumber].transform.Find(CATEGORIES[categoryIndex]).Find("Selected").gameObject.SetActive(false);

                    ResetDeselectDelay(panelNumber);
                }
            }
            ResetDeselectDelay(panelNumber);
        }
    }


    //--------------------------------------------------------------------------HELPERS:

    /// <Summary>
    /// Returns the vertical position (category) for the player on a given panel
    /// </Summary>
    protected int GetVerticalPosition(int panelIndex)
    {
        return verticalPosition[panelIndex];
    }

    /// <Summary>
    /// Returns the horizontal position (option for a given category) for the player on a given panel
    /// </Summary>
    protected int GetHorizontalPosition(int panelIndex)
    {
        return horizontalPositionGrid[panelIndex, verticalPosition[panelIndex]];
    }

    /// <Summary>
    /// Returns the horizontal position (option for a given category) for the player on a given panel
    /// </Summary>
    protected int GetHorizontalPosition(int panelIndex, int categoryIndex)
    {
        return horizontalPositionGrid[panelIndex, categoryIndex];
    }

    /// <Summary>
    /// Returns true if the category that the player for the given panel is currently on is locked in
    /// </Summary>
    protected bool IsCurrentCategoryLockedIn(int panelIndex)
    {
        return lockedIn[panelIndex][GetVerticalPosition(panelIndex)];
    }

    /// <Summary>
    /// Resets joystick delay
    /// </Summary>
    protected void ResetJoystickDelay(int panelIndex)
    {
        joystickDelay[panelIndex] = Time.time + tempDelay;
    }

    /// <Summary>
    /// Resets select delay
    /// </Summary>
    protected void ResetSelectDelay(int panelIndex)
    {
        selectDelay[panelIndex] = Time.time + tempDelay;
    }

    /// <Summary>
    /// Resets deselect delay
    /// </Summary>
    protected void ResetDeselectDelay(int panelIndex)
    {
        deselectDelay[panelIndex] = Time.time + tempDelay;
    }

    /// <Summary>
    /// Returns true if all players are fully locked in
    /// </Summary>
    protected bool AreAllPlayersReady()
    {
        if (players.Count < MinPlayers) return false;

        if (lockedIn
            .Any(selectionSet => selectionSet
            .Exists(selection => selection == false))) return false;

        return true;
    }

    /// <summary>
    /// Enables or Disables the Left indicator for the current category of player panelIndex
    /// </summary>
    /// <param name="panelIndex"></param>
    /// <param name="value"></param>
    protected void SetLeftArrowVisible(int panelIndex, int categoryIndex, bool value)
    {
        try
        {
            JoinedPanels[panelIndex].transform
                .Find(CATEGORIES[categoryIndex])
                .Find("Left Arrow")
                .gameObject
                .SetActive(value);
        }
        catch(NullReferenceException ex)
        {
            // Ignore exception because it probably comes from the character
            // category not having any arrows
        }
    }

    /// <summary>
    /// Enables or Disables the Right indicator for the current category of player panelIndex
    /// </summary>
    /// <param name="panelIndex"></param>
    /// <param name="value"></param>
    protected void SetRightArrowVisible(int panelIndex, int categoryIndex, bool value)
    {
        try
        {
            JoinedPanels[panelIndex].transform
                .Find(CATEGORIES[categoryIndex])
                .Find("Right Arrow")
                .gameObject
                .SetActive(value);
        }
        catch(NullReferenceException ex)
        {
            // Ignore exception because it probably comes from the character
            // category not having any arrows
        }
    }

    /// <summary>
    /// Returns the selected Enum in the grid of possible choices
    /// </summary>
    /// <param name="vertical">Player's vertical position in the list</param>
    /// <param name="horizontal">Player's horizontal position in the list</param>
    /// <returns></returns>
    protected System.Enum GetChoice(int vertical, int horizontal)
    {
        return CHOICES[vertical][horizontal] as System.Enum;
    }

    /// <summary>
    /// Returns the index of Panels in the UI for a given player
    /// </summary>
    /// <param name="message"></param>
    protected int GetPanelIndex(Controller controller)
    {
        return controllerToPanelIndex[controller];
    }

    /// <summary>
    /// Makes a GUI element flash red to indicate an error has occurred
    /// </summary>
    /// <param name="message"></param>
    protected IEnumerator ErrorFlash(GameObject slot)
    {
        if (slot.GetComponent<Image>() != null)
        {
            Color originalColor = slot.GetComponent<Image>().color;
            Color errorColor = new Color(0.9f, 0, 0, 0.7f);

            for (var n = 0; n < 3; n++)
            {
                slot.GetComponent<Image>().color = originalColor;
                yield return new WaitForSeconds(0.1f);
                slot.GetComponent<Image>().color = errorColor;
                yield return new WaitForSeconds(0.1f);
            }

            slot.GetComponent<Image>().color = originalColor;
            yield return null;
        }
    }

    protected void DLog(string message)
    {
        if (VERBOSE) this.GetType().Name.TPrint(message);
    }
}