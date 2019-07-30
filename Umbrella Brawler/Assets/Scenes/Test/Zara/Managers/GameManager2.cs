// // Dev Note: Still in progress
// // GameManager2 is now for testing purposes. M
// // Merged with GameManager under Umbrella Brawler\Assets\Scripts\


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// using MyUtility;
// using System.ComponentModel; // For Enum DescriptionAttribute

// public class GameManager2 : Singleton<GameManager>
// {

// //----------------------------------------------------------------------------ENUMS:
// public enum GameMode
// {
//     [Description("Free for All")]
//     FFA,

//     [Description("Team Deathmatch")]
//     TDM
// }

// //------------------------------------------------------------------------CONSTANTS:
//     private const string LOG_TAG = "GameManager";
//     public bool VERBOSE = false;
// //---------------------------------------------------------------------------FIELDS:

    
//     public GameMode Mode { get; set; }
//     public Player[] Players { get; set; }
//     public bool m_AIOn = false;
//     public float m_StartDelay = 3f;            
//     public float m_EndDelay = 3f;     
//     public Canvas m_Canvas = null; // For scene specific UI
//     public GameObject[] m_PlayerPrefabs;
//     public PlayerManager[] m_Players;              
//     public List<Transform> m_AIWaypoints; // TODO: Replace with WaypointNetwork
//     public int PlayerCounter = 0;   // Do not change this value in editor!
//                                     // TODO: Modify editor to make this readonly

// //-------------------------------------------------------------------PRIVATE FIELDS:

//     //private GameModeManager[] MGameMode;  // Need game mode manager in the future
//                                             // GameManager and GameModeManager
//                                             // may be dependent on each other
//     private WaitForSeconds startWaitForSeconds;  
//     private WaitForSeconds endWaitForSeconds;                    
//     private PlayerManager playerWinner; // Solo game modes
//     //private TeamManager teamWinner;  // Team game modes
//                                      // TODO: Need a team manager
           

// //--------------------------------------------------------------------------METHODS:

//     ///<Summary>
//     /// Spawns players, including AI if AI prefabs a
// 	/// 
// 	///</Summary>
//     private void Start()
//     {
//         // Delays can be tweaked in the Unity editor
//         startWaitForSeconds = new WaitForSeconds (m_StartDelay);
//         endWaitForSeconds = new WaitForSeconds (m_EndDelay);

//         // Might have to restrict max players to 4/8
//         SpawnPlayers();
        
//         StartCoroutine (GameLoop());
//     }

//     private void SpawnPlayers(){
        
//         for(int i = 0; i < m_Players.Length; i++){
//             m_Players[i].m_PlayerInstance = Instantiate(m_PlayerPrefabs[i],
//             m_Players[i].m_SpawnPoint.position,
//             m_Players[i].m_SpawnPoint.rotation) as GameObject;
//             m_Players[i].m_PlayerNumber = i + 1;
//             m_Players[i].PlayerSetup();
//             PlayerCounter++; 
            
//             // TODO: spawn AI and concat to human list
//         }
//     }

//     private IEnumerator GameLoop()
//     {
//         // Don't return until coroutines are done
//         yield return StartCoroutine (MatchStarting());
//         yield return StartCoroutine (MatchPlaying());
//         yield return StartCoroutine (MatchEnding());
        
//         if (playerWinner != null)
//         {
//             // Do whatever when there's a winner
//             Debug.Log("somebody won now");
//         }
//         // else if(teamWinner != null){
//         //     // Do whatever when a team wins
//         //     Debug.Log("some team won now");
//         // }
//         else
//         {   // Keep going through the loop if nobody's won
//             StartCoroutine (GameLoop());
//         }
//     }

//     private IEnumerator MatchStarting()
//     {
//         ResetAllPlayers();
//         DisablePlayerControl();
//         yield return startWaitForSeconds;
//     }

//     private IEnumerator MatchPlaying ()
//     {
//         EnablePlayerControl ();

//         // TODO: Clear scnene specific canvas overlay

//         // While no winner yet, continue
//         while (!LastPlayerStanding())
//         {
//             yield return null;
//         }
//     }

//     private IEnumerator MatchEnding ()
//     {
//         DisablePlayerControl (); // We don't necessarilly disable control,
//                                  // Mabybe play a "loser" animation while 
//                                  // Only be able to run around

//         // teamWinner = null;
//         playerWinner = GetMatchWinner();

//         // Do whatever when a player wins
//         if (playerWinner != null){
//             Debug.Log("Player" + playerWinner.m_PlayerNumber + "wins!");
//             // TODO: Multiple player winners if team win
//             // TODO: Method for team win?
//         }

//         // Display canvas after win
//         EndMatchCanvas();
       

//         // Wait for the specified length of time until yielding control back to the game loop.
//         yield return endWaitForSeconds;
//     }

//     // Check for the last player in scene
//     private bool LastPlayerStanding()
//     {
//         int playersLeft = 0;

//         for (int i = 0; i < m_Players.Length; i++)
//         {
//             if (m_Players[i].m_PlayerInstance.activeSelf)
//                 playersLeft++;
//         }
//         // Only one can win, but in case a player dies after the last enemy was killed
//         // 0 players will suffice too
//         return playersLeft <= 1 ? true : false;
//     }
    
//     // Find a winner
//     // Right now, this method only gets the last player standing winner
//     private PlayerManager GetMatchWinner()
//     {
    
//         for (int i = 0; i < m_Players.Length; i++)
//         {
//           // if one of them is active,
//             if (m_Players[i].m_PlayerInstance.activeSelf){
//                 return m_Players[i];
//             }
//         }
//         // No winner, nothing
//         return null;
//     }

//         private PlayerManager GetTeamMatchWinner()
//     {
        
//         for (int i = 0; i < m_Players.Length; i++)
//         {
//             // TODO: check for remaining team
//         }
//         // No winner, nothing
//         return null;
//     }

//     // Display something onscreen when match ends
//     private void EndMatchCanvas()
//     {
//         // By the end of the match
//         if(playerWinner == null){
//             Debug.Log("nobody won now"); // if time runs out, or tie
//         }
//         else if (playerWinner != null){
//             Debug.Log(playerWinner + " has won now");
//         }
//     }

//     private void ResetAllPlayers()
//     {
//         for (int i = 0; i < m_Players.Length; i++)
//         {
//             m_Players[i].Reset();
//         }
//     }

//     private void EnablePlayerControl()
//     {
//         for (int i = 0; i < m_Players.Length; i++)
//         {
//             m_Players[i].Enable();
//         }  
//     }

//     private void DisablePlayerControl()
//     {
//         for (int i = 0; i < m_Players.Length; i++)
//         {
//             m_Players[i].Disable();
//         }
//     }

// private void DLog( string message )
//     {
//         if( VERBOSE )   LOG_TAG.TPrint( message );        
//     }

// }
