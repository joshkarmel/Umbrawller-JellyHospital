using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enums;
public class Options : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = true;

//---------------------------------------------------------------------------FIELDS:
	[System.NonSerialized]
	public PlayerController PC;
	[System.NonSerialized]
	public PlayerInformation PI;
	public GameObject OptionsButton, OptionList, OptionsBackButton;
	private GameObject optionMenu, returnToGame, returnToMainMenu, restartLevel;
	private StandaloneInputModule inputModule;
	private string optionName;
	private Weapon playerWeapon;
	private MapObjectSetup mapObjectSetup = new MapObjectSetup();
	private Button returnToGameButton;
	private Enums.Player playerNum;
	private bool inOptionMenu, inPause;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		inputModule = GameObject.Find("EventSystem").gameObject.GetComponent<StandaloneInputModule>();
		optionMenu = transform.Find("Option Menu").gameObject;
		returnToGame = optionMenu.transform.Find("Return to game").gameObject;
		returnToGameButton = returnToGame.GetComponent<Button>();
		returnToMainMenu = optionMenu.transform.Find("Main Menu").gameObject;
		restartLevel = optionMenu.transform.Find("Restart").gameObject;
		inOptionMenu = false;

		PC = transform.root.gameObject.GetComponent<PlayerController>();
		PI = transform.root.gameObject.GetComponent<PlayerInformation>();
	}
		
	void Update()
    {
		if(inPause)
		{
			if(inOptionMenu)
			{
				//Inneficient code to force selected object to be back button in this option location
				EventSystem.current.GetComponent<EventSystem>()
                               .firstSelectedGameObject = OptionsBackButton.gameObject;
				EventSystem.current.GetComponent<EventSystem>()
                               .SetSelectedGameObject(OptionsBackButton.gameObject);
				//This section is needed until we finalize the game option canvas to actually look decent
				//Otherwise, nothing
			}
			if(Input.GetButtonUp(inputModule.cancelButton.ToString()))
			{
				if(inOptionMenu)
				{
					Return();
				}
				else
				{
					ReturnToGame();
				}
			}
		}
    }

//--------------------------------------------------------------------------METHODS:
	//Until the UI if fully completed, this is what we have

	public void GetPlayerNumber(Enums.Player Player)
	{
		playerNum = Player;
	}
	/// <Summary>
	/// Open the list of options
	/// </Summary>
	public void OpenOptions()
	{
		OptionsButton.SetActive(false);
		OptionList.SetActive(true);
		returnToGame.SetActive(false);
		returnToMainMenu.SetActive(false);
		restartLevel.SetActive(false);
		playerWeapon = PC.PlayerWeapon;
		inOptionMenu = true;
		if(OptionsBackButton != null)
		{
			EventSystem.current.GetComponent<EventSystem>()
                               .firstSelectedGameObject = OptionsBackButton.gameObject;
			EventSystem.current.GetComponent<EventSystem>()
                               .SetSelectedGameObject(OptionsBackButton.gameObject);
		}
	}

	/// <Summary>
	/// Return from option list
	/// </Summary>
	public void Return()
	{
		OptionsButton.SetActive(true);
		returnToGame.SetActive(true);
		returnToMainMenu.SetActive(true);
		restartLevel.SetActive(true);

		if(returnToGameButton != null)
		{
			EventSystem.current.GetComponent<EventSystem>()
                               .firstSelectedGameObject = returnToGameButton.gameObject;
			EventSystem.current.GetComponent<EventSystem>()
                               .SetSelectedGameObject(returnToGameButton.gameObject);
		}
		inOptionMenu = false;
		OptionList.SetActive(false);
		
	}

	/// <Summary>
	/// Return to play the game
	/// </Summary>
	public void ReturnToGame()
	{
		inPause = false;
		inOptionMenu = false;
		OptionList.SetActive(false);
		optionMenu.SetActive(false);
		Time.timeScale = 1;
		Return();
		GameManager.Instance.PauseAllPlayers(false);
	}

	/// <Summary>
	/// Initiliaze every visible option when player press pause
	/// </Summary>
	public void PauseMenu()
	{
		optionMenu.SetActive(true);
		inPause = true;
		//TODO set the eventsystem horizontal/vertical/etc to the player who did it
		if(returnToGameButton != null)
		{
			EventSystem.current.GetComponent<EventSystem>()
                               .firstSelectedGameObject = returnToGameButton.gameObject;
			EventSystem.current.GetComponent<EventSystem>()
                               .SetSelectedGameObject(returnToGameButton.gameObject);
		}
	}

	/// <Summary>
	/// Allow player to go back to main menu.
	/// </Summary>
	public void ReturnToMainMenu()
	{
		GameManager.Instance.FreezeAllPlayers();
		SceneManager.LoadScene("MainMenu");
		GameManager.Instance.StopGameLoop();
		GameManager.Instance.DeactivateSelf();
		ReturnToGame();
	}

	/// <Summary>
	/// Allow player restart the game
	/// </Summary>
	public void Reset()
	{
		//Set time back to scale to normal just in case.
		Time.timeScale = 1;
		GameManager.Instance.FreezeAllPlayers();
		GameManager.Instance.StopGameLoop();
		GameManager.Instance.DeactivateSelf();
		ReturnToGame();

		// Temp. Replace with something that knows how many players in scene
		Debug.Log(this.gameObject.scene.name);
 		mapObjectSetup.GetObjectsReadyForSceneRestart(GameManager.Instance.Players.Count, this.gameObject.scene.name, this.gameObject.scene.name, true, "Player Prefab", GameManager.Instance.Players);
		GameManager.Instance.ScreenDividerCheck();
        // GameManager.Instance.Players[0].Team = Team.Team1;
        // GameManager.Instance.Players[1].Team = Team.Team2;
	}

	/// <Summary>
	/// Changes the player who paused variables.
	/// </Summary>
	public void SetData(string input)
	{
		DLog("input: "+input + " - optionName - " + optionName);
		
		switch(optionName)
		{
			case "Umbrella Gravity":
				PC.UmbrellaGravity = float.Parse(input);
				break;
			case "Recoil Power":
				playerWeapon.MaxRecoilPower = float.Parse(input);
				break;
			case "Movement Speed":
				PC.GroundSpeed = float.Parse(input);
				break;
			case "Air Speed":
				PC.AirMovementSpeed = float.Parse(input);
				break;
			case "Max Falling Speed":
				PC.MaxFallSpeed = float.Parse(input);
				break;
			case "Max Umbrella Falling Speed":
				PC.UmbrellaFallingSpeed = float.Parse(input);
				break;
			case "Max Diving Speed":
				PC.MaxDiveSpeed = float.Parse(input);
				break;
			case "Player Health":
				PI.PlayerHealth = float.Parse(input);
				break;
			case "Fall Multiplier":
				PC.FallMultiplier = float.Parse(input);
				break;
		}
	}

	/// <Summary>
	/// Get the options name
	/// There might be a better way to do this, but I haven't found it yet
	/// </Summary>
	public void GetData(string input)
	{
		optionName=input;	
	}
//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}