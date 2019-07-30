using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyUtility;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Enums;

public class MainMenu : MonoBehaviour
{

    //------------------------------------------------------------------------CONSTANTS:

    public bool VERBOSE = false;
    public bool MuteBGM;
    private const string BACK_BUTTON = "OpenUmbrellaPlayerOne";
    public AudioClip BGM;
    //---------------------------------------------------------------------------FIELDS:


    public Image img;
    public GameObject WelcomeScreen;
    public GameObject StartMenu;
    public GameObject GameModeScreen;
    public GameObject OptionsMenu;
    public GameObject PlayersMenu;
    public GameObject PlayersTrainingMenu;
    public GameObject LevelSelectMenu;
    public GameObject MatchSettingsPanel;
    public Button StartMenuFirstSelected;
    public Button OptionsMenuFirstSelected;
    public Button PlayersMenuFirstSelected;
    public Button PlayersTrainingMenuFirstSelected;
    public Button LevelSelectMenuFirstSelected;
    public Button GameModeScreenFirstSelected;
    private MapObjectSetup mapObjectSetup = new MapObjectSetup();
    private Dictionary<GameObject, Button> firstSelectedButton;
    private bool gameModeInput;
    private float joystickDelay;

    // To keep track of the button that enabled Match Settings
    private GameObject sender;

    //---------------------------------------------------------------------MONO METHODS:

    void Start()
    {
        firstSelectedButton = new Dictionary<GameObject, Button>()
        {
            {WelcomeScreen, null },
            {StartMenu, StartMenuFirstSelected },
            {OptionsMenu, OptionsMenuFirstSelected },
            {PlayersTrainingMenu, PlayersTrainingMenuFirstSelected },
            {PlayersMenu, PlayersMenuFirstSelected },
            {LevelSelectMenu, LevelSelectMenuFirstSelected },
            {GameModeScreen, GameModeScreenFirstSelected}
        };
        SoundManager.PlayBGM(BGM);
        MuteBGM = false;
        joystickDelay = 0.2f;
    }

    void Update()
    {
        // Submit takes us off the welcome screen and onto the start menu
        if(WelcomeScreen.activeSelf && Input.GetButtonUp("Submit")) // A or Enter on keyboard
        {
            WelcomeScreen.SetActive(false);
            ScreenSetActive(StartMenu);
        }
        // Cancel sends us back to the welcome screen from the start menu
        else if(StartMenu.activeSelf && Input.GetButtonUp(BACK_BUTTON)) // B
        {
            StartMenu.SetActive(false);
            ScreenSetActive(WelcomeScreen);
        }
        else if (StartMenu.activeSelf && Input.GetButtonUp("ReloadPlayerOne")) // X
        {
            QuitButton();
        }
        else if(LevelSelectMenu.activeSelf && Input.GetButtonUp(BACK_BUTTON)) // B
        {
            LevelSelectMenu.SetActive(false);
            ScreenSetActive(GameModeScreen);
        }
        else if(GameModeScreen.activeSelf && !gameModeInput && Input.GetButtonUp(BACK_BUTTON))
        {
            GameModeScreen.SetActive(false);
            ScreenSetActive(StartMenu);
        }
        else if(PlayersTrainingMenu.activeSelf && Input.GetButtonUp(BACK_BUTTON))
        {
            PlayersTrainingMenu.SetActive(false);
            ScreenSetActive(StartMenu);
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            if(MuteBGM == false)
            {
                SoundManager.StopBGM();
                MuteBGM = true;
            }
            else if(MuteBGM == true)
            {
                SoundManager.PlayBGM(BGM);
                MuteBGM = false;
            }
        }

        // Set the focus back onto the EventSystem's firstSelectedGameObject,
        // which is set appropriately when navigating through different screens
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(
                EventSystem.current.firstSelectedGameObject);
        }

        if(gameModeInput)
        {
            StartCoroutine(GameModeInputHandler(MatchSettingsPanel));
        }
    }


    //--------------------------------------------------------------------------METHODS:

    public void QuitButton()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }


    // TODO: Do something actually useful with this
    public void Slider(float value)
    {
        var tempColor = img.color;
        tempColor.a = value;
        img.color = tempColor;
    }


    /// <summary>
    /// Creates Players, assigns teams, and goes to the appropriate scene
    /// Called when a number of players button is selected
    /// </summary>
    /// <param name="numPlayers"></param>
    public void PlayersStart(int numPlayers)
    {

        // If 2, set their teams and go directly to gameplay (Turns into FFA/1v1)
        // if (numPlayers == 2)
        // {

                mapObjectSetup.GetObjectsReadyForSceneChange(numPlayers, "TwoPlayers",this.gameObject.scene.name, true, "Player Prefab");

                // GameManager.Instance.Players[0].Team = Team.Team1;
                // GameManager.Instance.Players[1].Team = Team.Team2;

        //}
        // Anything else goes to Team selection
        // else
        // {
        //     mapObjectSetup.GetObjectsReadyForSceneChange(numPlayers, "TeamSelection",this.gameObject.scene.name, false, "");
        // }
    }


    /// <summary>
    /// THIS FUNCTION IS TEMPORARY
    /// Goes directly to a static or dynamic level with two players
    /// In the future, MainMenu will go to TeamSelection instead of directly to the game
    /// Just store the level choice in GameManager and go to TeamSelection when we get to that point
    /// </summary>
    /// <param name="level"></param>
    public void TEMP_GameStart(int level)
    {
        GameManager.Instance.Level = (Level) level;
        SceneManager.LoadScene("CharacterSelection");
    }

    /// <summary>
    /// Loads the desired scene.
    /// Level is always 0
    /// </summary>
    /// <param name="level"></param>
    public void LoadDesiredLevel(string scene)
    {
        //GameManager.Instance.Level = (Level) 0;
        SceneManager.LoadScene(scene);
    }
    /// <summary>
    /// Creates one player the rest being AI, assigns teams, and goes to the appropriate scene
    /// Called when a number of players (training) button is selected
    /// </summary>
    /// <param name="numPlayers"></param>
    public void TrainingStart(int numPlayers)
    {

        // If 2, set their teams and go directly to gameplay (Turns into FFA/1v1)
        // if (numPlayers == 2)
        // {
        GameManager.Instance.Level = Level.TrainingStaticLevel;
        GameManager.Instance.Players = new List<PlayerManager>();   
        GameManager.Instance.m_AIOn = true;
        for (int i = 0; i < numPlayers; i++)
        {
            GameManager.Instance.Players.Add(new PlayerManager());
        }

        LoadDesiredLevel("TrainingCharacterSelection");
            //mapObjectSetup.GetObjectsReadyForSceneChange(2, "TrainingStaticLevel",this.gameObject.scene.name, true, "Player Prefab");

            //GameManager.Instance.Players[0].Team = Team.Team1;
            //GameManager.Instance.Players[1].Team = Team.Team2;
            //GameManager.Instance.Players[1].IsAIPlayer = true;
        
        
        // }
    
        // else
        // {
        //     // GetObjectsReadyForSceneChange(numPlayers, "TeamSelection", false);
        // }
    }

    /// <summary>
    /// Used for switching between the parent GameObjects holding the 
    /// Main Menu screen elements
    /// Called from inspector from different screens in the main menu
    /// </summary>
    /// <param name="screen"></param>
    public void ScreenSetActive(GameObject screen)
    {
        // Set the thing to active
        screen.SetActive(true);

        if (firstSelectedButton[screen] != null)
        {
            // Set what the first selected button should be for the  
            // Update loop if focus is lost
            EventSystem.current.GetComponent<EventSystem>()
                               .firstSelectedGameObject = firstSelectedButton[screen].gameObject;

            // Actually set the selection
            EventSystem.current.GetComponent<EventSystem>()
                               .SetSelectedGameObject(firstSelectedButton[screen].gameObject);


            if (firstSelectedButton[screen].animator != null)
            {
                firstSelectedButton[screen].animator.SetTrigger("Highlighted");
            }            
        }

    }

    public void MatchSettings(int mode)
    {
        GameManager.Instance.GameMode = (GameMode)mode;
        MatchSettingsPanel.SetActive(true);
        this.sender = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(GameObject.Find("MatchTimeContainer"));
        gameModeInput = true;
    }

    private IEnumerator GameModeInputHandler(GameObject settingsMenu)
    {
        if (Input.GetButtonUp(Axes.toStr[Axes.Action.OpenUmbrellaPlayerOne]))
        {
            gameModeInput = false;
            settingsMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(sender);
        }
        else
        {
            if (EventSystem.current.currentSelectedGameObject.name == "MatchTimeContainer")
            {
                // Adjust match time
                Text time = settingsMenu.transform.Find("MatchTimeContainer/Time").gameObject.GetComponent<Text>();
                int minutes = int.Parse(time.text.Substring(0, time.text.IndexOf(':')));

                // Check for increase/decrease
                if(minutes < 15 && Input.GetAxis(Axes.toStr[Axes.Action.MoveXPlayerOne]) > 0.5 && Time.time > joystickDelay)
                {
                    time.text = string.Format("{0}:00", minutes + 1);
                    ResetJoystickDelay();
                }

                else if(minutes > 1 && Input.GetAxis(Axes.toStr[Axes.Action.MoveXPlayerOne]) < -0.5 && Time.time > joystickDelay)
                {
                    time.text = string.Format("{0}:00", minutes - 1);
                    ResetJoystickDelay();
                }
                GameManager.Instance.MatchTime = minutes * 60;
                
            }
            else if (EventSystem.current.currentSelectedGameObject.name == "MaxScoreContainer")
            {
                // Adjust max score
                Text score = settingsMenu.transform.Find("MaxScoreContainer/Score").gameObject.GetComponent<Text>();
                int maxScore = int.Parse(score.text);

                // Check for increase/decrease
                if (Input.GetAxis(Axes.toStr[Axes.Action.MoveXPlayerOne]) > 0.5 && Time.time > joystickDelay)
                {
                    score.text = (maxScore + 1).ToString();
                    ResetJoystickDelay();
                }

                else if (maxScore > 1 && Input.GetAxis(Axes.toStr[Axes.Action.MoveXPlayerOne]) < -0.5 && Time.time > joystickDelay)
                {
                    score.text = (maxScore - 1).ToString();
                    ResetJoystickDelay();
                }

                GameManager.Instance.MaxScore = maxScore;
            }

            // TODO: For next term, add variations to this game mode
            //else if (EventSystem.current.currentSelectedGameObject.name == "ModeContainer")
            //{
            //    // Adjust mode
            //    Text mode = EventSystem.current.currentSelectedGameObject.transform.Find("ModeContainer/Mode").gameObject.GetComponent<Text>();

            //    switch(mode.text)
            //    {
            //        case "Kills":
            //            // Check for increase/decrease
            //            if (Input.GetAxis(Axes.toStr[Axes.Action.MoveXPlayerOne]) > 0.5)
            //            {
            //                mode.text = "Lives";
            //            }
            //            break;
            //        case "Lives":

            //            break;
            //        case "Time":
            //            break;
            //    }
            //}

            else if (EventSystem.current.currentSelectedGameObject.name == "OK")
            {

                // If selected
                if (Input.GetButtonUp(Axes.toStr[Axes.Action.JumpPlayerOne]))
                {
                    GameModeScreen.SetActive(false);
                    ScreenSetActive(transform.Find("LevelSelectMenu").gameObject);
                }
            }
        }

        yield return null;
    }


    //--------------------------------------------------------------------------HELPERS:

    /// <Summary>
    /// Resets joystick delay
    /// </Summary>
    private void ResetJoystickDelay()
    {
        joystickDelay = Time.time + 0.2f;
    }


    private void DLog(string message)
    {
        if (VERBOSE) GetType().Name.TPrint(message);
    }

}
