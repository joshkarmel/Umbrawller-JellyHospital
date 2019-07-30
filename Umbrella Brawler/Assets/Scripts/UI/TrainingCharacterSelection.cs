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

public class TrainingCharacterSelection : CharacterSelection
{
    //------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

    //---------------------------------------------------------------------------FIELDS:

    public Axes.Action[] PlayerRB, PlayerLB;
    protected int totalPlayers;
    protected float bumperDelay = 0.2f;
	
    //---------------------------------------------------------------------MONO METHODS:

	new void Start() 
	{
        base.Start();
        MinPlayers = 1;
        SetupPlayers();
	}

    new void Update()
    {
        base.Update();
    }

    //--------------------------------------------------------------------------METHODS:

    public override void StartGame()
    {
        List<PlayerManager> tempplayers = players.Values.ToList();
        DLog("Starting Game");
        mapObjectSetup.GetObjectsReadyForSceneChange(players.Count, this.gameObject.scene.name, true, "Player Prefab", tempplayers);
        SoundManager.StopBGM();
    }

    /// <summary>
    /// Sets up player panels 
    /// </summary>
    private void SetupPlayers()
    {
        // Assume that we have the correct number of players in GameManager from MainMenu
        for(int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            if (i == 0) // Force first panel to be a controller player
                HandlePlayerJoined(Controller.Controller1);
            else // Rest will be AI 
                HandleAIJoined();
        }
    }

    /// <Summary>
    /// Sets up variables and UI elements when a player joins
    /// </Summary>
    protected void HandleAIJoined()
    {
        players[(Controller)playerCount] = new PlayerManager();
        players[(Controller)playerCount].PlayerNumber = (Player)playerCount;
        players[(Controller)playerCount].IsAIPlayer = true;

        InitialJoinTexts[playerCount].SetActive(false);
        JoinedPanels[playerCount].SetActive(true);

        lockedIn.Add(new List<bool> { false, false, false, false, false });

        playerCount++;
    }

    /// <Summary>
    /// Get all the inputs from all playing controllers at the same time.
    /// Apply the input to their respective location
    /// For now, only allow left/right and a and b(hold) to go back
    /// Update deals with player 1.
    /// in other words, imagine this as 3 Updates functions
    /// </Summary>
    protected override IEnumerator HandlePlayerInputs(Controller controllerNum)
    {
        if (!readyToStart)
            // Movement
            GetJoystickInput(controllerNum);

        // Selection
        GetSelectInput(controllerNum);

        // Deselection
        GetReturnInput(controllerNum);

        // Switch panels
        GetBumperInput(controllerNum);

        yield return null;
    }

    private void GetBumperInput(Controller controllerNum)
    {
        bool RBPressed = Input.GetButtonDown(Axes.toStr[PlayerRB[(int)controllerNum]]);
        bool LBPressed = Input.GetButtonDown(Axes.toStr[PlayerLB[(int)controllerNum]]);

        int panelNumber = GetPanelIndex(controllerNum);

        if(RBPressed && Time.time > bumperDelay)
        {
            if (panelNumber < players.Count - 1)
            {
                //int nextControllablePanel;
                //for(nextControllablePanel = panelNumber+1; nextControllablePanel < players.Count; nextControllablePanel++)
                //{
                //    if(players[(Controller)nextControllablePanel].IsAIPlayer || BackAtOriginalPanel)
                //    {
                //        // Then we can control the panel at index = nextControllablePanel
                //    }
                //}
                controllerToPanelIndex[controllerNum]++;
                ResetBumperDelay(controllerNum);
            }
        }
        else if(LBPressed && Time.time > bumperDelay)
        {
            if (panelNumber > 0)
            {
                // Now... we need to get the next panel index
                //int nextControllablePanel;
                //for (nextControllablePanel = panelNumber - 1; nextControllablePanel >= 0; nextControllablePanel--)
                //{
                //    if (players[(Controller)nextControllablePanel].IsAIPlayer || BackAtOriginalPanel)
                //    {
                //        // Then we can control the panel at index = nextControllablePanel
                //    }
                //}
                controllerToPanelIndex[controllerNum]--;
                ResetBumperDelay(controllerNum);
            }
        }
    }

    /// <Summary>
    /// Locks in the player's choices for each category
    /// </Summary>
    protected override void GetSelectInput(Controller controllerNum)
    {
        bool selectPressed = Input.GetButtonDown(Axes.toStr[PlayerSubmit[(int)controllerNum]]);
        int panelNumber = GetPanelIndex(controllerNum);

        bool readyToLockIn = true;

        if (selectPressed && Time.time >= selectDelay[panelNumber])
        {
            // Validations first
            for (int categoryIndex = 0; categoryIndex < CATEGORIES.Length; categoryIndex++)
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
            if (readyToLockIn)
            {
                for (int categoryIndex = 0; categoryIndex < CATEGORIES.Length; categoryIndex++)
                {
                    System.Enum selected = GetChoice(categoryIndex, GetHorizontalPosition(panelNumber, categoryIndex));
                    switch (CATEGORIES[categoryIndex])
                    {
                        case "Team":
                            playersInTeam[(Team)selected]++;
                            players[(Controller)panelNumber].Team = (Team)selected;
                            break;
                        case "Gun":
                            players[(Controller)panelNumber].Weapon = (Weapons)selected;
                            break;
                        case "Umbrella":
                            players[(Controller)panelNumber].Umbrella = (Umbrellas)selected;
                            break;
                        case "Ultimate":
                            players[(Controller)panelNumber].Ultimate = (Ultimates)selected;
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

    /// <summary>
    /// Resets delay for RB/LB presses. This one takes controllerNum as an
    /// argument because bumpers switch panel numbers.
    /// </summary>
    /// <param name="controllerNum"></param>
    private void ResetBumperDelay(Controller controllerNum)
    {
        bumperDelay = Time.time + tempDelay;
    }





  

    //--------------------------------------------------------------------------HELPERS:    
}