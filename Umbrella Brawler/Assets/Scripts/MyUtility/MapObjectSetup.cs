using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using System;
using UnityEngine.EventSystems;


public class MapObjectSetup : MonoBehaviour{

	// Make a local array of Players, and corresponding player prefabs and spawnpoints
	private List<PlayerManager> tempPlayers = new List<PlayerManager>();
	private List<GameObject> tempPlayerPrefabs = new List<GameObject>();
	private List<Transform> tempSpawnPoints = new List<Transform>();
 	private List<Transform> tempWayPoints = new List<Transform>();
    private GameObject tempCircuitArea; // one thing probably


    // TODO: Clean up. These are very similar
    public void GetObjectsReadyForSceneChange(int numPlayers, string newSceneName, string oldSceneName, bool isMap, string resourcesFolder)
    {
        if(SceneChanger.Instance.IsSceneLoading == false){
            SceneChanger.Instance.IsSceneLoading = true;
            // Create each Player object and corresponding prefab
            // Assumes each player is the same type
            for(int i = 1; i <= numPlayers; i++){
                tempPlayers.Add(new PlayerManager());
                tempPlayers[i - 1].Name = "Player " + i;
                if(isMap){
                // tempPlayerPrefabs.Add(Resources.Load("Player Prefabs/Player 1 Resource") as GameObject);
                    foreach(GameObject el in Resources.LoadAll(resourcesFolder, typeof(GameObject))){
                    tempPlayerPrefabs.Add(el);
                    }
                }
            }

            
            GameManager.Instance.Players = tempPlayers;
            if(isMap){
                GameManager.Instance.m_PlayerPrefabs = tempPlayerPrefabs;
            }

            Action postLoad = () =>
            {
                
                if (!isMap) { 
                    Debug.Log("Scene loaded was not a map");
                    return; 
                }

                searchForSpawnPoints();
                if(GameManager.Instance.m_AIOn){
                    searchForWayPoints();
                    searchForCircuitArea();
                }

                
                GameManager.Instance.ActivateSelf();
                EventSystem.current.enabled = true;
                SceneChanger.Instance.IsSceneLoading = false;

                setGameMode();
            }; 
            
            GameManager.Instance.StartCoroutine(SceneChanger.Instance.ChangeScene(newSceneName, oldSceneName, postLoad)); 
        }
    }

    public void GetObjectsReadyForSceneRestart(int numPlayers, string newSceneName, string oldSceneName, bool isMap, string resourcesFolder, List<PlayerManager> players)
    {
        if(SceneChanger.Instance.IsSceneLoading == false){
            SceneChanger.Instance.IsSceneLoading = true;
            // Create each Player object and corresponding prefab
            // Assumes each player is the same type
            for(int i = 1; i <= numPlayers; i++){
                tempPlayers.Add(new PlayerManager());
                tempPlayers[i - 1].Name = "Player " + i;
                if(isMap){
                // tempPlayerPrefabs.Add(Resources.Load("Player Prefabs/Player 1 Resource") as GameObject);
                    foreach(GameObject el in Resources.LoadAll(resourcesFolder, typeof(GameObject))){
                    tempPlayerPrefabs.Add(el);
                    }
                }
            }

             tempPlayers = players;
            GameManager.Instance.Players = players;
            if(isMap){
                GameManager.Instance.m_PlayerPrefabs = tempPlayerPrefabs;
            }

            Action postLoad = () =>
            {
                
                if (!isMap) { 
                    Debug.Log("Scene loaded was not a map");
                    return; 
                }

                searchForSpawnPoints();
                if(GameManager.Instance.m_AIOn){
                    searchForWayPoints();
                    searchForCircuitArea();
                }

                
                GameManager.Instance.ActivateSelf();
                EventSystem.current.enabled = true;
                SceneChanger.Instance.IsSceneLoading = false;

                setGameMode();
            }; 
            
            GameManager.Instance.StartCoroutine(SceneChanger.Instance.ChangeScene(newSceneName, oldSceneName, postLoad)); 
        }
    }

    // I have no idea why there are 2
    public void GetObjectsReadyForSceneChange(int numPlayers, string oldSceneName, bool isMap, string resourcesFolder, List<PlayerManager> players)
    {
        if (SceneChanger.Instance.IsSceneLoading == false)
        {
            SceneChanger.Instance.IsSceneLoading = true;
            // Create each Player object and corresponding prefab
            // Assumes each player is the same type
            for (int i = 1; i <= numPlayers; i++)
            {
                if (isMap)
                {
                    // tempPlayerPrefabs.Add(Resources.Load("Player Prefabs/Player 1 Resource") as GameObject);
                    foreach (GameObject el in Resources.LoadAll(resourcesFolder, typeof(GameObject)))
                    {
                        tempPlayerPrefabs.Add(el);
                    }
                }
            }

            tempPlayers = players;
            GameManager.Instance.Players = players;
            if (isMap)
            {
                GameManager.Instance.m_PlayerPrefabs = tempPlayerPrefabs;
            }

            Action postLoad = () =>
            {
                if (!isMap)
                {
                    Debug.Log("Scene loaded was not a map");
                    return;
                }

                searchForSpawnPoints();
                if (GameManager.Instance.m_AIOn)
                {
                    searchForWayPoints();
                    searchForCircuitArea();
                }

                GameManager.Instance.ActivateSelf();
                EventSystem.current.enabled = true;
                SceneChanger.Instance.IsSceneLoading = false;

                setGameMode();

            };


            string newSceneName = Utility.GetEnumDescription(GameManager.Instance.Level);
            GameManager.Instance.StartCoroutine(SceneChanger.Instance.ChangeScene(newSceneName, oldSceneName, postLoad));
        }
    }

    private void searchForSpawnPoints(){
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("SpawnPoint")){
                tempSpawnPoints.Add(gameObject.GetComponent<Transform>());
            }
        tempSpawnPoints.Sort((e1, e2) => e1.transform.GetSiblingIndex().CompareTo(e2.transform.GetSiblingIndex()));
        for(int i = 0; i < tempPlayers.Count; i++){
            GameManager.Instance.Players[i].m_SpawnPoint =  tempSpawnPoints[i];
        }
	}

	private void searchForWayPoints(){
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("WayPoint")){
                tempWayPoints.Add(gameObject.GetComponent<Transform>());
            }
        tempWayPoints.Sort((e1, e2) => e1.transform.GetSiblingIndex().CompareTo(e2.transform.GetSiblingIndex()));
        GameManager.Instance.m_AIWaypoints =  tempWayPoints;     
	}

    private void searchForCircuitArea(){
        tempCircuitArea = GameObject.FindGameObjectWithTag("CircuitArea");
        GameManager.Instance.CircuitArea = tempCircuitArea;
	}

    private void setGameMode()
    {
        if (GameManager.Instance.GameMode == Enums.GameMode.Deathmatch)
        {
            EventSystem.current.GetComponentInParent<DominationMode>().enabled = false;
            EventSystem.current.GetComponentInParent<GameModeTracker>().enabled = true;
        }
        else if (GameManager.Instance.GameMode == Enums.GameMode.Domination)
        {
            EventSystem.current.GetComponentInParent<DominationMode>().enabled = true;
            EventSystem.current.GetComponentInParent<GameModeTracker>().enabled = false;
        }
    }
}

