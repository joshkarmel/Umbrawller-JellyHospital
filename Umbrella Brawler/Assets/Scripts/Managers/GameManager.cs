using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyUtility;
using UnityEditor;
using Enums;

[RequireComponent(typeof(CharacterSetUp))]
public class GameManager : Singleton<GameManager>
{



//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "GameManager";
    public bool VERBOSE = false;
//---------------------------------------------------------------------------FIELDS:

    
    public GameMode GameMode { get; set; }
    public Level Level { get; set; }
    public bool m_AIOn = false;
    public float StartDelay = 5f;            
    public float EndDelay = 3f;
    public float SpawnFreezeDelay = 1.5f;
    public float SpawnDeathDelay= 2.5f;    
    public int NumberOfTeams; //How many different teams exist in scene
    public Canvas m_Canvas = null; // For scene specific UI
    public List<GameObject> m_PlayerPrefabs;
    public List<PlayerManager> Players;           
    public List<Transform> m_AIWaypoints; // TODO: Replace with WaypointNetwork
    public GameObject CircuitArea;
    public List<Vector3> RandomVector3s;
    [System.NonSerialized] public GameModeTracker gameModeTracker;
    [System.NonSerialized] public int PlayerCounter = 0;   
    [System.NonSerialized] public GameObject MainCanvas;
    [System.NonSerialized] public Queue<PlayerManager> DeadPlayerQueue;

    // I put these here for now cause it's the easiest way to access them
    // Also have default values
    public float MatchTime = 300;
    public int MaxScore = 3;
    public bool playToMaxKills = true;

//-------------------------------------------------------------------PRIVATE FIELDS:

    //private GameModeManager[] MGameMode;  
    private WaitForSeconds startWaitForSeconds;  
    private WaitForSeconds endWaitForSeconds;
    private WaitForSeconds spawnWaitForSeconds;      
    private WaitForSeconds deathWaitForSeconds;              
    private PlayerManager playerWinner; // Solo game modes
    private PlayerManager tempToRespawn;
    private CharacterSetUp characterSetUp;
    private bool isGameLoopRunning;
    private bool firstExecution = true; //Just initial state
    private float respawnDuration = 0;
    private GameObject verticalLine;
    private GameObject horizontalLine;


//--------------------------------------------------------------------------METHODS:

    ///<Summary>
    /// Deisables GameManager on instantiation so values can be set
	/// but Start() is not called
	///</Summary>
    void Awake(){
        this.enabled = false;
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
        DeadPlayerQueue = new Queue<PlayerManager>();
        
        MainCanvas = GameObject.FindGameObjectWithTag("Canvas");
        verticalLine = MainCanvas.transform.Find("Vertical Screen Divider").gameObject;
        horizontalLine = MainCanvas.transform.Find("Horizontal Screen Divider").gameObject;

        ScreenDividerCheck();
        SpawnPlayers();
        if(m_AIOn){


        }

        characterSetUp = this.GetComponent<CharacterSetUp>();
        characterSetUp.playerControllers = new PlayerController[Players.Count];
        characterSetUp.cameraController = new CameraController[Players.Count];
        characterSetUp.weapon = new Weapon[Players.Count];
        for (int i = 0; i < Players.Count; i++)
        {
            characterSetUp.playerControllers[i] = Players[i].m_PlayerInstance.GetComponent<PlayerController>();
            characterSetUp.cameraController[i] = characterSetUp.playerControllers[i].CamCon;
            characterSetUp.weapon[i] = characterSetUp.playerControllers[i].PlayerWeapon;
        }
        
        characterSetUp.enabled = true;
        SetAmountOfTeams();
        StartGameLoop();
    }

    // this might be temporary because we already have a hard reset (scene change)
    public void SoftResetSceneTemp(){
        StopGameLoop();
        RespawnAllPlayers();
        StartGameLoop();
    }

    private void Start()
    {
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
        //Dev utility
        if(Input.GetKeyUp(KeyCode.C))
		{
			characterSetUp.ChangePlayers();
		}
    }

    /// <Summary>
	/// Disables or enables all players.
	/// </Summary>
    public void PauseAllPlayers(bool disable)
    {
        if(disable)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].GetPlayerController().DisablePlayer();
            }
        }
        else
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].GetPlayerController().EnablePlayer();
            }
        }
    }

    public void ScreenDividerCheck()
    {
        if (Players.Count < 2)
        {
            verticalLine.SetActive(false);
            horizontalLine.SetActive(false);
        }
        else if (Players.Count < 3)
            horizontalLine.SetActive(false);
    }

    ///<Summary>
    /// Get all the players team enum
    /// Count how many unique teams exist in the match and set NumberOfTeams
    ///</Summary>
    public void SetAmountOfTeams()
    {
        Enums.Team[] Teams = new Team[Players.Count];
        NumberOfTeams = 2;
        
        for (int i = 0; i < Players.Count; i++)
        {
            Teams[i] = Players[i].GetPlayerInformation().TeamNumber;
            if(i > 1) //Only check if there's at least 3 players
            {    
                //If the array does not contain the enum string, then increase number of teams    
                if(!Teams.ToString().Contains(Teams[i].ToString()))
                {
                    NumberOfTeams++;
                }
            }
        }
    }
//--------------------------------------------------------------------------HELPERS:
    private void SpawnPlayers(){
      //  if(m_AIOn){
            // Players[0].m_PlayerInstance = Instantiate(m_PlayerPrefabs[0],
            // Players[0].m_SpawnPoint.position,
            // Players[0].m_SpawnPoint.rotation) as GameObject;
            // Players[0].m_PlayerNumber = 0;
            // Players[0].PlayerSetup();
            // PlayerCounter++; 

            // for(int i = 1; i < Players.Count; i++){
            //     Players[i].m_PlayerInstance = Instantiate(m_PlayerPrefabs[i],
            //     Players[i].m_SpawnPoint.position,
            //     Players[i].m_SpawnPoint.rotation) as GameObject;
            //     Players[i].m_PlayerNumber = i;
            //     Players[i].AIPlayerSetup(m_AIWaypoints);
            //     PlayerCounter++; 
            // }   
            // DLog("AI successfully instantiated");   


            for(int i = 0; i < Players.Count; i++){
                Players[i].m_PlayerInstance = Instantiate(m_PlayerPrefabs[i],
                Players[i].m_SpawnPoint.position,
                Players[i].m_SpawnPoint.rotation) as GameObject;
                Players[i].m_PlayerNumber = i;
                if(Players[i].IsAIPlayer){
                     Players[i].AIPlayerSetup(m_AIWaypoints);
                }
                else{
                      Players[i].PlayerSetup();
                }
                PlayerCounter++;
            }
            if(PlayerCounter == 3) //If 3 players, set up a black screen for 4th player slot
            {
                MainCanvas.transform.Find("3 Player Extra Screen").gameObject.SetActive(true);
            }
        // }
        // else{
        //     for(int i = 0; i < Players.Count; i++){
        //         Players[i].m_PlayerInstance = Instantiate(m_PlayerPrefabs[i],
        //         Players[i].m_SpawnPoint.position,
        //         Players[i].m_SpawnPoint.rotation) as GameObject;
        //         Players[i].m_PlayerNumber = i;
        //         Players[i].PlayerSetup();
        //         PlayerCounter++; 
        //     }
        // }
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

        RespawnAllPlayers();
        DisableAllPlayerControl();

        yield return startWaitForSeconds; 
       
    }


    private IEnumerator MatchPlaying (){
        DLog("Match is playing");
        
        if(Input.GetKeyUp(KeyCode.C))
		{
			characterSetUp.ChangePlayers();
		}

        UnFreezeAllPlayers();
        EnableAllPlayerControl();

        gameModeTracker = GameObject.Find("EventSystem").gameObject.GetComponent<GameModeTracker>();

        // It's enabled in MapObjectSetup's postLoad now
        //gameModeTracker.enabled = true;

        while (!GameHasEnded()) {   // While no winner yet, continue
            DeathAndRespawnCycle();
            // setAICanMove(false);
            // setAIGravity(false);
           // setAIMovementWhenUmbrellaIsOpen();
            //setAIMovementWhenCanDive();
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
    

    private PlayerManager GetMatchWinner(){
    
        for (int i = 0; i < Players.Count; i++){

            /* 

            if (there is a winner){
                return Players[i];
            }

            */

        }
        return null;
    }

    private PlayerManager GetTeamMatchWinner(){
        
        for (int i = 0; i < Players.Count; i++){
          
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
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].Respawn();
        }
    }

    public void FreezeAllPlayers(){
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].SetInstanceIsFrozen(true);
        }
    }

    public void UnFreezeAllPlayers(){
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].SetInstanceIsFrozen(false);
        }
    }

    private void EnableAllPlayerControl()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].EnableInstance();
        }  
    }

    private void DisableAllPlayerControl()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].DisableInstance();
        }
    }
////////////////////////////////////////////////////////////////////////////////////////
///////////////The below should be moved to other scripts eventually///////////////////
//////////////////////////////////////////////////////////////////////////////////////

    public Vector3 createVector(GameObject shape){
        if(m_AIOn){
            return VectorGenerator.GenerateVector3InSpehere(CircuitArea);
        }

        return new Vector3(0,0,0);
    }


    // Uses tempDeadPlayer to insert the latest player to the front of the queue
    private IEnumerator EnqueueDeadPlayersInScene(){
        DLog("Executing EnqueueDeadPlayers");
        PlayerManager tempDeadPlayer;
        for (int i = 0; i < Players.Count; i++)
        {   
            // if a player is dead and not in the middle of spawning
            // put into a death queue
            if (Players[i].IsInstanceDead() && !Players[i].IsInstanceSpawning()){
                tempDeadPlayer = Players[i];
                DLog(tempDeadPlayer.m_PlayerNumber + " JUST DIED!");
                //tempDeadPlayer.SetInstanceDeathSimulation(false);
                tempDeadPlayer.PlayDeathSim();
                tempDeadPlayer.SetInstanceIsSpawning(true);
                tempDeadPlayer.DisableInstance();
                yield return StartCoroutine(WaitForSpawnCountdown(tempDeadPlayer));
                tempDeadPlayer.SetModelIsEnabled(false);
                DeadPlayerQueue.Enqueue(tempDeadPlayer);
            }
        }
        yield return null;
    }

    // Extract first in line
    private IEnumerator DequeueDeadPlayer(){
        DLog("Executing DequeueDeadPlayers");
        if(DeadPlayerQueue.Count != 0){
            tempToRespawn = (PlayerManager)DeadPlayerQueue.Dequeue();
        }
        yield return null;
    }

    private IEnumerator ReviveDeadPlayer(PlayerManager player){
        if(player != null
            && player.IsInstanceDead()
            && player.IsInstanceSpawning()){
            player.RestartRespawnSim();
            DLog("Player" + player.m_PlayerNumber + " is waiting to respawn.");
            yield return StartCoroutine(WaitToRespawn(player));
        }
    }
    
    private IEnumerator WaitToRespawn(PlayerManager player){
 
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

    private IEnumerator WaitForUnFreeze(PlayerManager player){

        yield return spawnWaitForSeconds;
        player.SetInstanceIsFrozen(false);
        
    }

    private IEnumerator WaitForSpawnCountdown(PlayerManager player){

        yield return deathWaitForSeconds;
        DLog("Animation or ragdoll or whatever goes here");
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


private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
