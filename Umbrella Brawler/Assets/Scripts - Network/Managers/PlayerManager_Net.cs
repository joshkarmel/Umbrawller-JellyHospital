using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using MyUtility;
using Enums;
using UnityEngine.Networking;


[Serializable]
public class PlayerManager_Net : NetworkBehaviour 
{

//------------------------------------------------------------------------CONSTANTS:
    public bool VERBOSE = false;
//---------------------------------------------------------------------------FIELDS:

    public Transform m_SpawnPoint; // where an instance of a player spawns
    [HideInInspector] public int m_PlayerNumber; // Keeps track of players in scene
    [HideInInspector] public GameObject m_PlayerInstance;
    //[HideInInspector] public GameObject m_StateControllerInstance;
    [HideInInspector] List<Transform> m_WaypointList;
    // TODO: Finalize fields
    public string Name { get; set; }
    public Team Team { get; set; }
    public Player PlayerNumber {get; set;}

    //-------------------------------------------------------------------PRIVATE FIELDS:

    private PlayerController_Net playerController_Net;
    private PlayerInformation_Net playerInformation_Net;
    private CameraController cameraController;
    private Camera camera;
    private Weapon_Net weapon_Net;
    private PlayerCanvas_Net playerCanvas_Net;
    private StateController stateController; // State machine
    private GameObject nameTag; // or other canvas objects attached to the player prefab
    private Renderer renderer;
    private Ultimate_Net ultimate_Net;
//--------------------------------------------------------------------------METHODS:

    // Shared setup between human player and AI player
    // later, we take out camera and canvas stuff, and just place uniquely in PlayerSetup()
    private void CommonSetup(){
        playerController_Net = m_PlayerInstance.GetComponent<PlayerController_Net>();
        playerInformation_Net = m_PlayerInstance.GetComponent<PlayerInformation_Net>();
        cameraController = playerController_Net.CamCon;
        weapon_Net = playerController_Net.PlayerWeaponNet;
        PlayerNumber = (Player)m_PlayerNumber;
        camera = playerController_Net.Cam;
        ultimate_Net = playerController_Net.Ultimate_Net;
        playerCanvas_Net = m_PlayerInstance.transform.Find("PlayerCanvas").GetComponent<PlayerCanvas_Net>();
        renderer = m_PlayerInstance.transform.Find("Model").transform.Find("default").transform.Find("Ace_Mesh").GetComponent<SkinnedMeshRenderer>();

        switch(Team)
        {
            case Team.Team1:
                renderer.material.color = Color.red;
                break;
            case Team.Team2:
                renderer.material.color = Color.cyan;
                break;
            case Team.Team3:
                renderer.material.color = Color.green;
                break;
            case Team.Team4:
                renderer.material.color = Color.yellow;
                break;
        }
        
		// Change player setup to network only version
        // IndividualPlayerSetUp.SetUpPlayer(playerController_Net,
        //                                 playerInformation_Net,
        //                                 cameraController,
        //                                 weapon_Net,
        //                                 ultimate_Net,
        //                                 PlayerNumber);
        //IndividualPlayerSetUp.SetUpCameraPosition(camera, PlayerNumber, GameManager.Instance.Players.Count);
    }
    ///<Summary>
    /// Create a human controlled instance of a player
    ///</Summary>
    public void PlayerSetup(){
        CommonSetup();
        // playerController = m_PlayerInstance.GetComponent<PlayerController>();
        // cameraController = playerController.CamCon;
        // weapon = playerController.PlayerWeapon;
        // PlayerNumber = (Player)m_PlayerNumber;
        // camera = playerController.Cam;
        // ultimate = playerController.Ultimate;
        // playerCanvas = m_PlayerInstance.transform.Find("PlayerCanvas").GetComponent<PlayerCanvas>();
        // renderer = m_PlayerInstance.transform.Find("Model").transform.Find("default").transform.Find("Ace_Mesh").GetComponent<SkinnedMeshRenderer>();

        // switch(Team)
        // {
        //     case Team.Team1:
        //         renderer.material.color = Color.red;
        //         break;
        //     case Team.Team2:
        //         renderer.material.color = Color.cyan;
        //         break;
        //     case Team.Team3:
        //         renderer.material.color = Color.green;
        //         break;
        //     case Team.Team4:
        //         renderer.material.color = Color.yellow;
        //         break;
        // }
        
        // IndividualPlayerSetUp.SetUpPlayer(playerController,
        //                                 cameraController,
        //                                 weapon,
        //                                 ultimate,
        //                                 PlayerNumber);
        // IndividualPlayerSetUp.SetUpCameraPosition(camera, PlayerNumber, GameManager.Instance.Players.Count);
    }

    ///<Summary>
	/// Create an AI instance of a player
	///</Summary>
    public void AIPlayerSetup(List<Transform> waypoints){
        CommonSetup();

        /////////////////////////////////////////// VITAL ///////////////////////////////////
        // put on the player preafb
        stateController = m_PlayerInstance.GetComponent<StateController>();
        stateController.enabled = true;
        stateController.SetupAI(true, waypoints);
    }

    public PlayerController_Net GetPlayerController(){

       
        PlayerController_Net playerController = this.playerController_Net;
        return playerController;

    }

    public PlayerInformation_Net GetPlayerInformation()
    {
        PlayerInformation_Net playerInformation = this.playerInformation_Net;
        return playerInformation;

    }
    
    public PlayerCanvas_Net GetPlayerCanvas(){

        PlayerCanvas_Net playerCanvas = this.playerCanvas_Net;
        return playerCanvas;
        
    }    


    ///<Summary>
    /// Disable player movement and other actions
	/// Commonly used during start of a round, when players spawn before game starts
	///</Summary>
    public void DisableInstance()
        {
			if(playerController_Net != null){
                playerController_Net.DisablePlayer();
            }

			if(stateController != null){
				stateController.enabled = false;
            }
        }

    ///<Summary>
    /// Enable player movement and other actions
	/// Commonly used during the start of a round 
	///</Summary>
    public void EnableInstance(){
        if(playerController_Net!= null){
            playerController_Net.EnablePlayer();
        }
        if(stateController != null){
            stateController.enabled = true;
        }
    }

    public void EnableCanMove(){}

    public bool IsInstanceDead(){
        if(playerInformation_Net.IsDead == true){
            return true;
        }else{
            return false;
        }
    }

    public bool IsInstanceSpawning(){
        if(playerController_Net.IsSpawning == true){
            return true;
        }else{
            return false;
        }
    }

    public void SetInstanceIsSpawning(bool isSpawning){
        playerController_Net.IsSpawning = isSpawning;
    }

    public void SetInstanceIsDead(bool isDead){
        playerInformation_Net.IsDead = isDead;
    }

    public void SetInstanceIsFrozen(bool isFrozen){
        playerController_Net.KinematicEnabled(isFrozen);
    }

    public void SetModelIsEnabled(bool enabled){
        playerController_Net.ModelEnabled(enabled);
    }

    public void ResetStats(){
        playerController_Net.ResetStatsAfterDeath();
    }

    public bool isAI(){
        if(m_PlayerInstance.GetComponent<StateController>().enabled){
            return true;
        }
        else{
            return false;
        }
    }

    ///probably temp
    public void SetAIInstanceCanMove(bool canMove){
        if(this.isAI()){
            playerController_Net.CanMove = canMove;
        }
    }

    public void SetAIInstanceGravity(bool gravityEnabled){
        if(this.isAI()){
            m_PlayerInstance.GetComponent<Rigidbody>().useGravity = gravityEnabled;
        }
    }



    ///<Summary>
    /// Used to respawn players at their designated spawn location
	/// Commonly used during the start of a round, or after death
	///</Summary>
    public void Respawn ()
        {
            m_PlayerInstance.transform.position = m_SpawnPoint.position;
            m_PlayerInstance.transform.rotation = m_SpawnPoint.rotation;
            m_PlayerInstance.SetActive (false);
            m_PlayerInstance.SetActive (true);
        }

    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );  
    }

   
}