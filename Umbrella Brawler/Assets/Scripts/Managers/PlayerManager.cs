using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using MyUtility;
using Enums;

[Serializable]
public class PlayerManager{

//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "PlayerManager";
    private const string WEAPONS_PATH = "Weapons/Guns/";
    private const string ULTS_PATH = "Weapons/Ultimates/";
    private const string UMBRELLAS_PATH = "Weapons/";
    public bool VERBOSE = true;
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

    // The controller that controls this player, defaults to Controllers.None if unassigned
    public Controller ControllerNumber;
    public bool IsAIPlayer;

    [Header("Selections from CharacterSelection stored in these variables")]
    public Ultimates Ultimate;
    public Umbrellas Umbrella;
    public Weapons Weapon;

    //-------------------------------------------------------------------PRIVATE FIELDS:

    private PlayerController playerController;
    private PlayerInformation playerInformation;
    private CameraController cameraController;
    private Camera camera;
    private Camera UICamera;
    private PlayerCanvas playerCanvas;
    private StateController stateController; // State machine
    private GameObject nameTag; // or other canvas objects attached to the player prefab
    private Renderer renderer;
    private SimulationShader simShader;
    
//--------------------------------------------------------------------------METHODS:

  
    private void CommonSetup(){ 
       // simShader = m_PlayerInstance.GetComponentInChildren<SimulationShader>();
        playerController = m_PlayerInstance.GetComponent<PlayerController>();
        playerInformation = m_PlayerInstance.GetComponent<PlayerInformation>();
        cameraController = playerController.CamCon;
        camera = playerController.Cam;
        UICamera = playerController.UICam;
        playerCanvas = m_PlayerInstance.transform.Find("PlayerCanvas").GetComponent<PlayerCanvas>();
        simShader = m_PlayerInstance.transform.Find("Model").transform.Find("default").transform.Find("Ace_Mesh").gameObject.GetComponent<SimulationShader>();
        renderer = m_PlayerInstance.transform.Find("Model").transform.Find("default").transform.Find("Ace_Mesh").GetComponent<SkinnedMeshRenderer>();

        // Here "Color_50BC8552" is "OutlineShader" because Unity is dumb
        switch(Team)
        {
            case Team.Team1:
                //renderer.material.color = Color.red;
                //renderer.material.SetColor("Color_50BC8552", Color.red);
                simShader.OutlineColor = Color.red;
                
                break;
            case Team.Team2:
                //renderer.material.color = Color.cyan;
                //renderer.material.SetColor("Color_50BC8552", Color.cyan);
                simShader.OutlineColor = Color.cyan;
                break;
            case Team.Team3:
                //renderer.material.color = Color.green;
                //renderer.material.SetColor("Color_50BC8552", Color.green);
                simShader.OutlineColor = Color.green;
                break;
            case Team.Team4:
                //renderer.material.color = Color.yellow;
                //renderer.material.SetColor("Color_50BC8552", Color.yellow);
                simShader.OutlineColor = Color.yellow;
                break;
        }
        
        //IndividualPlayerSetUp.SetUpPlayer(playerController,
        //                                playerInformation,
        //                                cameraController,
        //                                UICamera,
        //                                playerController.PlayerWeapon,
        //                                playerController.Ultimate,
        //                                PlayerNumber);
        IndividualPlayerSetUp.SetUpCameraPosition(camera, UICamera, PlayerNumber, GameManager.Instance.Players.Count, playerCanvas);
    }
    ///<Summary>
    /// Create a human controlled instance of a player
    ///</Summary>
    public void PlayerSetup(){
        CommonSetup();

        SetWeapon();
        SetUltimate();

        IndividualPlayerSetUp.SetUpPlayer(playerController,
                                        playerInformation,
                                        cameraController,
										UICamera,
                                        playerController.PlayerWeapon,
                                        playerController.Ultimate,
                                        PlayerNumber,
                                        ControllerNumber);
    }

    ///<Summary>
	/// Create an AI instance of a player
	///</Summary>
    public void AIPlayerSetup(List<Transform> waypoints){
        PlayerNumber = (Player)m_PlayerNumber;
        CommonSetup();
        SetWeapon();
        SetUltimate();
        IndividualPlayerSetUp.SetUpPlayer(playerController,
                                        playerInformation,
                                        cameraController,
										UICamera,
                                        playerController.PlayerWeapon,
                                        playerController.Ultimate,
                                        PlayerNumber,
                                        Controller.Controller2); // temporary fix

        stateController = m_PlayerInstance.GetComponent<StateController>();
        stateController.enabled = true;
        stateController.SetupAI(true, waypoints);
    }

    public PlayerController GetPlayerController(){

       
        PlayerController playerController = this.playerController;
        return playerController;

    }

    public PlayerInformation GetPlayerInformation()
    {
        PlayerInformation playerInformatoin = this.playerInformation;
        return playerInformation;

    }
    
    public PlayerCanvas GetPlayerCanvas(){

        PlayerCanvas playerCanvas = this.playerCanvas;
        return playerCanvas;
        
    }

    public StateController GetStateController(){
        return this.stateController;
    }    


    ///<Summary>
    /// Disable player movement and other actions
	/// Commonly used during start of a round, when players spawn before game starts
	///</Summary>
    public void DisableInstance()
        {
			if(playerController != null){
                playerController.DisablePlayer();
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
        if(playerController!= null){
            playerController.EnablePlayer();
        }
        if(stateController != null){
            stateController.enabled = true;
        }
    }

    public void EnableCanMove(){}

    public bool IsInstanceDead(){
        if(playerInformation.IsDead == true){
            return true;
        }else{
            return false;
        }
    }

    public bool IsInstanceSpawning(){
        if(playerController.IsSpawning == true){
            return true;
        }else{
            return false;
        }
    }

    public void SetInstanceIsSpawning(bool isSpawning){
        playerController.IsSpawning = isSpawning;
    }

    public void SetInstanceIsDead(bool isDead){
        playerInformation.IsDead = isDead;

    }

    public void SetInstanceDeathSimulation(bool isDisintegrating){
        simShader.enabled = isDisintegrating;
    }

    public void PlayDeathSim(){
        SetInstanceDeathSimulation(false);
        simShader.selectedBlendEffect = SimulationShader.SetBlendEffect.Decrease;
        simShader.selectedEmission = SimulationShader.SetEmission.Decrease;
        simShader.selectedBottomToTopProgression = SimulationShader.SetBottomToTopProgression.None;
        simShader.selectedOutlineThickness = SimulationShader.SetOutlineThickness.None;
        simShader.selectedOutlineIntensity = SimulationShader.SetOutlineIntensity.Decrease;
        SetInstanceDeathSimulation(true);
        // do the reverse stuff
    }

    public void RestartRespawnSim(){
        simShader.selectedBlendEffect = SimulationShader.SetBlendEffect.Increase;
        simShader.selectedEmission = SimulationShader.SetEmission.Increase;
        simShader.selectedBottomToTopProgression = SimulationShader.SetBottomToTopProgression.Decrease;
        simShader.selectedOutlineThickness = SimulationShader.SetOutlineThickness.Increase;
        simShader.selectedOutlineIntensity = SimulationShader.SetOutlineIntensity.Increase;

        // do the reverse stuff
    }

    public void SetInstanceIsFrozen(bool isFrozen){
        playerController.KinematicEnabled(isFrozen);
    }

    public void SetModelIsEnabled(bool enabled){
        playerController.ModelEnabled(enabled);
    }

    public void ResetStats(){
        playerController.ResetStatsAfterDeath();
    }

    public bool isAI(){
        if(m_PlayerInstance.GetComponent<StateController>().enabled){
            return true;
        }
        else{
            return false;
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
           // simShader.enabled = false;
        }

    /// <summary>
    /// Instantiates a prefab given the path to the prefab relative to the Resources folder
    /// Note: Probably need to cast the GameObject to the required type
    /// </summary>
    /// <param name="path">Path to the prefab</param>
    private UnityEngine.Object LoadPrefab(string path)
    {
        return UnityEngine.Object.Instantiate(Resources.Load(path));
    }

    private void SetWeapon()
    {
        // AI players don't have selections, so default to the shotgun
        Weapon = (Weapon == Weapons.None) ? Weapons.Shotgun : Weapon;

        GameObject selectedWeapon = LoadPrefab(WEAPONS_PATH + Utility.GetEnumDescription(Weapon)) as GameObject;
        selectedWeapon.GetComponent<Weapon>().ParentWeapon = m_PlayerInstance.transform.Find("PlayerWeapons/Weapon");
        selectedWeapon.GetComponent<Weapon>().Player = m_PlayerInstance;
        //selectedWeapon.GetComponent<Weapon>().Reticle = m_PlayerInstance.transform.Find("Model/Camera/UI Cam/Reticle").gameObject;
        selectedWeapon.GetComponent<Weapon>().Umbrella = m_PlayerInstance.transform.Find("Model/UmbrellaEx").gameObject;
        selectedWeapon.GetComponent<Weapon>().WeaponBindLocation = playerController.WeaponBindLocation;
        m_PlayerInstance.transform.rotation = new Quaternion(0, 0, 0, 0);
        selectedWeapon.transform.SetParent(m_PlayerInstance.transform.Find("PlayerWeapons/Weapon"));
        playerController.PlayerWeapon = selectedWeapon.GetComponent<Weapon>();
        
        selectedWeapon.SetActive(true);
        
    }

    private void SetUltimate()
    {
        Ultimate = (Ultimate == Ultimates.None) ? Ultimates.BrokenEngine : Ultimate;

        GameObject selectedUlt = LoadPrefab(ULTS_PATH + Utility.GetEnumDescription(Ultimate)) as GameObject;
        selectedUlt.GetComponent<Weapon>().ParentWeapon = playerController.Ultimate.transform;
        selectedUlt.GetComponent<Weapon>().Player = m_PlayerInstance;
        selectedUlt.GetComponent<Weapon>().WeaponBindLocation = playerController.WeaponBindLocation;

        playerController.Ultimate.GetComponent<Ultimate>().GunWeapon =  playerController.PlayerWeapon.gameObject;
        playerController.Ultimate.GetComponent<Ultimate>().UltWeapon = selectedUlt;
        m_PlayerInstance.transform.rotation = new Quaternion(0, 0, 0, 0);
        selectedUlt.transform.SetParent(playerController.Ultimate.transform);
        selectedUlt.SetActive(true);
        
        
    }

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

   
}