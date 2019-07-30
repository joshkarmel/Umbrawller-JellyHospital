using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour {
	private MapObjectSetup mapObjectSetup = new MapObjectSetup();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/// <summary>
    /// THIS FUNCTION IS TEMPORARY
    /// Goes directly to a level selection screen and also dev to load specific scenes
    /// </summary>
    /// <param name="sceneName"></param>
    public void TEMP_DevGameStart(string sceneName)
    {
        GameManager.Instance.Level = (Level) 0;
        mapObjectSetup.GetObjectsReadyForSceneChange(2, sceneName,this.gameObject.scene.name, true, "Player Prefab");
    }
}
