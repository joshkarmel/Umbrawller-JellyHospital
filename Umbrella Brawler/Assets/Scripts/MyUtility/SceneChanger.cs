using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using MyUtility;

public class SceneChanger : Singleton<SceneChanger> {

//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "SceneChanger";
    public bool VERBOSE = false;

//------------------------------------------------------------------------FIELDS:

	public bool IsSceneLoading = false;

//--------------------------------------------------------------------------METHODS:

	public IEnumerator ChangeScene(string newSceneName, string oldSceneName, Action postLoad)
	{
		if (Application.CanStreamedLevelBeLoaded(newSceneName))
		{	
			if(IsSceneLoading == true){
			//yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
			Debug.Log("Scene loaded.");
				 
			}
		}

		postLoad();
		
		if (IsSceneLoading == false)
        {
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
			IsSceneLoading = false;
			Debug.Log("Scene Active.");
		}
	
		
	
		// if (Application.CanStreamedLevelBeLoaded(newSceneName)
		// 	&& !string.IsNullOrEmpty(newSceneName))
		// {
		// 	yield return new WaitForEndOfFrame();
		// 	SceneManager.UnloadSceneAsync(oldSceneName);
		// }
	}
	
	  // If the level is not already loading, load it (prevents double tap loads)
        // if(!isLoading)
        // yield return StartCoroutine("LoadLevelAsync");
    
	//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
