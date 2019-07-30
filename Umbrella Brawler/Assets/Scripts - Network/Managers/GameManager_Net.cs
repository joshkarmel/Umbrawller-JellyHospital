using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyUtility;
using UnityEditor;
using Enums;
using UnityEngine.Networking;

//[RequireComponent(typeof(CharacterSetUp))]
public class GameManager_Net : NetworkBehaviour
{
	//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "GameManager";
    public bool VERBOSE = false;
	//---------------------------------------------------------------------------FIELDS:
    public static GameManager_Net Instance;
	public GameMode GameMode { get; set; }
    public Level Level { get; set; }
    public bool m_AIOn = false;
    public float StartDelay = 3f;            
    public float EndDelay = 3f;
    public float SpawnFreezeDelay = 2f;
    public float SpawnDeathDelay= 2f;    
    public Canvas m_Canvas = null; // For scene specific UI
    public List<GameObject> m_PlayerPrefabs;
    public List<PlayerManager_Net> Players_Net;           
    public List<Transform> m_AIWaypoints; // TODO: Replace with WaypointNetwork
    public GameObject CircuitArea;
    public List<Vector3> RandomVector3s;
    [System.NonSerialized] public GameModeTracker gameModeTracker;
    [System.NonSerialized] public int PlayerCounter = 0;   
    [System.NonSerialized] public GameObject MainCanvas;
    [System.NonSerialized] public Queue<PlayerManager_Net> DeadPlayerQueue_Net;

//-------------------------------------------------------------------PRIVATE FIELDS:

    //private GameModeManager[] MGameMode;  
    private WaitForSeconds startWaitForSeconds;  
    private WaitForSeconds endWaitForSeconds;
    private WaitForSeconds spawnWaitForSeconds;      
    private WaitForSeconds deathWaitForSeconds;              
    private PlayerManager_Net playerWinner; // Solo game modes
    private PlayerManager_Net tempToRespawn;
    //private CharacterSetUp characterSetUp;
    private bool isGameLoopRunning;
    private bool firstExecution = true; //Just initial state
    private float respawnDuration = 0;

    
	///<Summary>
    /// Disable GameManager on instantiation so values can be set
	/// but Start() is not called
	///</Summary>
    void Awake()
	{
		//If this is not the server, delete it. This is server only script
		//Done both in awake and start just in case
        if (Instance == null)
        {
            Instance = this;
        }

		if(!isServer)
		{
			Destroy(this);
			return;
		}
        this.enabled = false;
       
        // prevent gameloop before match starts
    }

	void OnEnable(){
        if(firstExecution == false){
            Init();
        }
    }

    private void Init(){
        startWaitForSeconds = new WaitForSeconds(StartDelay);
        endWaitForSeconds = new WaitForSeconds(EndDelay);
        spawnWaitForSeconds = new WaitForSeconds(SpawnFreezeDelay);
        deathWaitForSeconds = new WaitForSeconds(SpawnDeathDelay);
        DeadPlayerQueue_Net = new Queue<PlayerManager_Net>();
        
        MainCanvas = GameObject.FindGameObjectWithTag("Canvas");
        
		//No need to spawn players in online. I think
        //SpawnPlayers();
        //if(m_AIOn){
            //populateWaypointList();
        //}

        // characterSetUp = this.GetComponent<CharacterSetUp>();
        // characterSetUp.playerControllers = new PlayerController_Net[Players_Net.Count];
        // characterSetUp.cameraController = new CameraController_Net[Players_Net.Count];
        // characterSetUp.weapon = new Weapon[Players_Net.Count];
        // for (int i = 0; i < Players_Net.Count; i++)
        // {
        //     characterSetUp.playerControllers[i] = Players_Net[i].m_PlayerInstance.GetComponent<PlayerController_Net>();
        //     characterSetUp.cameraController[i] = characterSetUp.playerControllers[i].CamCon;
        //     characterSetUp.weapon[i] = characterSetUp.playerControllers[i].PlayerWeapon;
        // }
        
        // characterSetUp.enabled = true;
        StartGameLoop();
    }

    // this might be temporary because we already have a hard reset (scene change)
    public void SoftResetSceneTemp(){
        StopGameLoop();
        RespawnAllPlayers();
        StartGameLoop();
    }

	// Use this for initialization
	void Start () 
	{
		//If this is not the server, delete it. This is server only script
		if(!isServer)
		{
			Destroy(this);
			return;
		}

		firstExecution = false;
        DLog("GameManager started");
        Init();
	}
	
	public void DeactivateSelf(){  
        if(this.enabled){
            this.enabled = false;
            DLog("GameManager deactivated.");
        }
        else{
            DLog("GameManager is already disabled.");
        }
    }

    public void ActivateSelf(){
        if(!this.enabled){
            this.enabled = true;
            DLog("GameManager activated.");
        }
        else{
            DLog("GameManager is already enabled.");
        }
    }

    private void StartGameLoop(){
        // TODO: Wrapper to stop a current GameLoop (if it exists), then restart.
        StartCoroutine(GameLoop());
    }
    

    private void Update(){
    }

    /// <Summary>
	/// Disables or enables all players.
	/// </Summary>
    public void PauseAllPlayers(bool disable)
    {
        if(disable)
        {
            for (int i = 0; i < Players_Net.Count; i++)
            {
                Players_Net[i].GetPlayerController().DisablePlayer();
            }
        }
        else
        {
            for (int i = 0; i < Players_Net.Count; i++)
            {
                Players_Net[i].GetPlayerController().EnablePlayer();
            }
        }
    }
//--------------------------------------------------------------------------HELPERS:
    private void SpawnPlayers(){
        if(m_AIOn){
            Players_Net[0].m_PlayerInstance = Instantiate(m_PlayerPrefabs[0],
            Players_Net[0].m_SpawnPoint.position,
            Players_Net[0].m_SpawnPoint.rotation) as GameObject;
            Players_Net[0].m_PlayerNumber = 0;
            Players_Net[0].PlayerSetup();
            PlayerCounter++; 

            for(int i = 1; i < Players_Net.Count; i++){
                Players_Net[i].m_PlayerInstance = Instantiate(m_PlayerPrefabs[i],
                Players_Net[i].m_SpawnPoint.position,
                Players_Net[i].m_SpawnPoint.rotation) as GameObject;
                Players_Net[i].m_PlayerNumber = i;
                Players_Net[i].AIPlayerSetup(m_AIWaypoints);
                PlayerCounter++; 
            }   
            DLog("AI successfully instantiated");   
        }
        else{
            for(int i = 0; i < Players_Net.Count; i++){
                Players_Net[i].m_PlayerInstance = Instantiate(m_PlayerPrefabs[i],
                Players_Net[i].m_SpawnPoint.position,
                Players_Net[i].m_SpawnPoint.rotation) as GameObject;
                Players_Net[i].m_PlayerNumber = i;
                Players_Net[i].PlayerSetup();
                PlayerCounter++; 
            }
        }
    }

    private IEnumerator GameLoop(){   
        isGameLoopRunning = true;
        FreezeAllPlayers();
        // Don't return until coroutines are done
        yield return StartCoroutine (MatchStarting());
        yield return StartCoroutine (MatchPlaying());
        yield return StartCoroutine (MatchEnding());

        if (playerWinner != null){
            // this doesn't really happpen currently
            DLog("The match has ended with a winner");
        }

        else{   // Keep going through the loop if nobody's won
            DLog("Nobody has won yet. Continue gameloop");
            StartCoroutine (GameLoop());
        }
    }

    private IEnumerator MatchStarting(){
        DLog("Mtach is starting, start delay is " + StartDelay + "f");
       // VectorGenerator.GenerateVector3InSpehere(CircuitArea);
        RespawnAllPlayers();
        DisableAllPlayerControl();

        yield return startWaitForSeconds; 
       
    }

    public Vector3 createVector(GameObject shape){
        if(m_AIOn){
            return VectorGenerator.GenerateVector3InSpehere(CircuitArea);
        }

        return new Vector3(0,0,0);
    }

    private IEnumerator MatchPlaying (){
        DLog("Match is playing");
        
        if(Input.GetKeyUp(KeyCode.C))
		{
			//characterSetUp.ChangePlayers();
		}

        UnFreezeAllPlayers();
        EnableAllPlayerControl();

        gameModeTracker = GameObject.Find("EventSystem").gameObject.GetComponent<GameModeTracker>();
        gameModeTracker.enabled = true;

        while (!GameHasEnded()) {   // While no winner yet, continue
            DeathAndRespawnCycle();
            SetAICanMove(false); // false for now i suppose
            SetAIGravity(false); // same as above
            DLog("Game has not ended. continue..."); 
            yield return null; 
        }
    }
    
    private IEnumerator MatchEnding(){

        DLog("Match is ending");
        DisableAllPlayerControl(); 

        playerWinner = GetMatchWinner();

        // Do whatever when a player/team wins
        if (playerWinner != null){
            DLog("Player or team" + playerWinner.m_PlayerNumber + "wins!");
        }
        EndMatchCanvas();
       
        // Wait for the specified length of time until yielding control back to the game loop.
        yield return endWaitForSeconds;
    }

    public void StopGameLoop(){
        DLog("StopGameLoop() executued.");
        isGameLoopRunning = false;
        this.StopAllCoroutines();
    }

    // Uses tempDeadPlayer to insert the latest player to the front of the queue
    private IEnumerator EnqueueDeadPlayersInScene(){
        DLog("Executing EnqueueDeadPlayers");
        PlayerManager_Net tempDeadPlayer;
        for (int i = 0; i < Players_Net.Count; i++)
        {   
            // if a player is dead and not in the middle of spawning
            // put into a death queue
            if (Players_Net[i].IsInstanceDead() && !Players_Net[i].IsInstanceSpawning()){
                tempDeadPlayer = Players_Net[i];
                DLog(tempDeadPlayer.m_PlayerNumber + " JUST DIED!");
                tempDeadPlayer.SetInstanceIsSpawning(true);
                tempDeadPlayer.DisableInstance();
                yield return StartCoroutine(WaitForSpawnCountdown(tempDeadPlayer));
                tempDeadPlayer.SetModelIsEnabled(false);
                DeadPlayerQueue_Net.Enqueue(tempDeadPlayer);
            }
        }
        yield return null;
    }

    // Extract first in line
    private IEnumerator DequeueDeadPlayer(){
        DLog("Executing DequeueDeadPlayers");
        if(DeadPlayerQueue_Net.Count != 0){
            tempToRespawn = (PlayerManager_Net)DeadPlayerQueue_Net.Dequeue();
        }
        yield return null;
    }

    private IEnumerator ReviveDeadPlayer(PlayerManager_Net player){
        if(player != null
            && player.IsInstanceDead()
            && player.IsInstanceSpawning()){
            
            DLog("Player" + player.m_PlayerNumber + " is waiting to respawn.");
            yield return StartCoroutine(WaitToRespawn(player));
        }
    }
    
    private IEnumerator WaitToRespawn(PlayerManager_Net player){
 
        yield return new WaitForSeconds(GlobalVars.Instance.RespawnTime);

        player.SetInstanceIsFrozen(true);
        player.ResetStats();
        player.Respawn();
        player.SetModelIsEnabled(true);
        player.EnableInstance();
        player.SetInstanceIsSpawning(false);
        player.SetInstanceIsDead(false);
        StartCoroutine(WaitForUnFreeze(player));

    }

    private IEnumerator WaitForUnFreeze(PlayerManager_Net player){

        yield return spawnWaitForSeconds;
        player.SetInstanceIsFrozen(false);
        
    }

    private IEnumerator WaitForSpawnCountdown(PlayerManager_Net player){

        yield return deathWaitForSeconds;
        DLog("Animation or ragdoll or whatever goes here");
    }

    private void SetAICanMove(bool canMove){
        for(int i = 0; i < Players_Net.Count; i++){
            if(Players_Net[i].isAI()){
                Players_Net[i].SetAIInstanceCanMove(canMove);
            }
        }
    }

    private void SetAIGravity(bool gravityEnabled){
        for(int i = 0; i < Players_Net.Count; i++){
            if(Players_Net[i].isAI()){
                Players_Net[i].SetAIInstanceGravity(gravityEnabled);
            }
        }

    }


    private void DeathAndRespawnCycle(){
        
        DLog("DeathAndRespawnCycle is executing");
        StartCoroutine(EnqueueDeadPlayersInScene());
        StartCoroutine(DequeueDeadPlayer());
        if(tempToRespawn != null){
            StartCoroutine(ReviveDeadPlayer(tempToRespawn));
            tempToRespawn = null;
        }
    }

   
    /// <Summary>
	/// Use the GameMode tracker in conjuction with this
	/// Whatever it is that causes the game to end
	/// </Summary>
    private bool GameHasEnded(){

        /* 

        if(whatever causes a game to end){
            return true;
        }


        */

        return false;
    }
    

    private PlayerManager_Net GetMatchWinner(){
    
        for (int i = 0; i < Players_Net.Count; i++){

            /* 

            if (there is a winner){
                return Players[i];
            }

            */

        }
        return null;
    }

    private PlayerManager_Net GetTeamMatchWinner(){
        
        for (int i = 0; i < Players_Net.Count; i++){
          
            /* 

            if (there is a winner){
                add to list of winners and return them
            }

            */

        }
        return null;
    }

    // Display something onscreen when match ends
    private void EndMatchCanvas()
    {
        // By the end of the match
        if(playerWinner == null){
            Debug.Log("Nobody won now"); // if time runs out, or tie
        }
        else if (playerWinner != null){
            Debug.Log(playerWinner + " has won now");
        }
    }

    private void RespawnAllPlayers()
    {
        for (int i = 0; i < Players_Net.Count; i++)
        {
            Players_Net[i].Respawn();
        }
    }

    public void FreezeAllPlayers(){
        for (int i = 0; i < Players_Net.Count; i++)
        {
            Players_Net[i].SetInstanceIsFrozen(true);
        }
    }

    public void UnFreezeAllPlayers(){
        for (int i = 0; i < Players_Net.Count; i++)
        {
            Players_Net[i].SetInstanceIsFrozen(false);
        }
    }

    private void EnableAllPlayerControl()
    {
        for (int i = 0; i < Players_Net.Count; i++)
        {
            Players_Net[i].EnableInstance();
        }  
    }

    private void DisableAllPlayerControl()
    {
        for (int i = 0; i < Players_Net.Count; i++)
        {
            Players_Net[i].DisableInstance();
        }
    }

private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
