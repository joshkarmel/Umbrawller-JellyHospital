using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS
using Enums;
public class CharacterSetUp : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public PlayerController[] playerControllers;
	public CameraController[] cameraController;
	public Weapon[] weapon;

	private List<PlayerManager> player; 
	private PlayerManager playerManager;
    private GameObject canvas;
    private GameObject verticalLine;
    private GameObject horizontalLine;
	private int playerToChangeTo = 0;
//---------------------------------------------------------------------MONO METHODS:

	void Awake()
	{
        this.enabled = false;
    }

	void Start() 
	{
		player = GameManager.Instance.Players;
		//playerController = playerManager.GetPlayerController();
	}
		
	void Update()
    {

    }

//--------------------------------------------------------------------------METHODS:
	public void ChangePlayers()
	{
        playerToChangeTo++;
        if (playerToChangeTo < player.Count)
        { 
            PlayersSettings(playerToChangeTo);
		}
		else
		{
			playerToChangeTo = 0;
			PlayersSettings(playerToChangeTo);
		}
	}

    public void PlayersSettings(int playerNum)
    {
        // Change the next player to player one inputs
        playerControllers[playerNum].Horizontal = Axes.Action.MoveXPlayerOne;
        playerControllers[playerNum].Vertical = Axes.Action.MoveYPlayerOne;
        cameraController[playerNum].XAxis = Axes.Action.CamXPlayerOne;
        cameraController[playerNum].YAxis = Axes.Action.CamYPlayerOne;
        weapon[playerNum].ShootForwardButton = Axes.Action.ShootForwardPlayerOne;
        weapon[playerNum].ShootBackwardButton = Axes.Action.ShootBackwardPlayerOne;
        weapon[playerNum].ReloadButton = Axes.Action.ReloadPlayerOne;
        playerControllers[playerNum].JumpButton = Axes.Action.JumpPlayerOne;
        playerControllers[playerNum].ShieldButton = Axes.Action.ShieldPlayerOne;
        //playerController Aim button for future
        playerControllers[playerNum].UmbrellaButton = Axes.Action.OpenUmbrellaPlayerOne;
        playerControllers[playerNum].StartButton = Axes.Action.StartPlayerOne;


        for (int i = 0; i < player.Count; i++)
        {
            if(i != playerNum)
            {
                // Keyboard escape key; the other players are useless
                playerControllers[i].Horizontal = Axes.Action.Escape;
                playerControllers[i].Vertical = Axes.Action.Escape;
                cameraController[i].XAxis = Axes.Action.Escape;
                cameraController[i].YAxis = Axes.Action.Escape;
                weapon[i].ShootForwardButton = Axes.Action.Escape;
                weapon[i].ShootBackwardButton = Axes.Action.Escape;
                weapon[i].ReloadButton = Axes.Action.Escape;
                playerControllers[i].JumpButton = Axes.Action.Escape;
                playerControllers[i].ShieldButton = Axes.Action.Escape;
                //playerController Aim button for future
                playerControllers[i].UmbrellaButton = Axes.Action.Escape;
                playerControllers[i].StartButton = Axes.Action.Escape;
            }
        }
    }

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}