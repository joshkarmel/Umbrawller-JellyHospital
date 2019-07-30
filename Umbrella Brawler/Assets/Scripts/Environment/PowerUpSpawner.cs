using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour {

	public List<GameObject> ThingsToSpawn = new List<GameObject>();
	private List<GameObject> SpawnAreas = new List<GameObject>();
	private List<Vector3> spawnVectors = new List<Vector3>();
	private List<GameObject> instThings = new List<GameObject>();
	private string tagString = "SpawnArea";

	// Use this for initialization
	// void Start () {
		
	// 	SpawnThings();
	// }

	void OnEnable(){
		SpawnThings();
	}

	void OnDisable(){
		// in case you wanna despawn the area spawns
		foreach(GameObject inst in instThings){
			Destroy(inst);
		}
		spawnVectors.Clear();
	}

	public void SpawnThings(){
	

		//if(SpawnAreas == null){
			SpawnAreas.AddRange(GameObject.FindGameObjectsWithTag(tagString));
		

			for(int i = 0; i < SpawnAreas.Count; i++){
				Debug.Log("SPAWN AREAS: " + SpawnAreas[i]);
				spawnVectors.Add(VectorGenerator.GenerateVector3InSpehere(SpawnAreas[i]));
				Debug.Log("SPAWN VECTORS: " + spawnVectors[i]);
			}

			for(int i = 0; i < spawnVectors.Count; i++){
				instThings.Add(Instantiate(ThingsToSpawn[Mathf.RoundToInt(Random.Range(0, ThingsToSpawn.Count))], spawnVectors[i], Quaternion.identity));
			}
	//	}
	}
}
