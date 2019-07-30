using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public class MapSpawnerObject : MonoBehaviour {

//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	

	[Range (0.1f,100f)] public float SpawnDelay = 1f;
	[Range(0.1f, 20f)] public float AbsoluteGravity = 9.81f;
	[Range(0f, 1000f)] public float AbsoluteCenterToXEdge = 20f;
	[Range(0f, 1000f)] public float AbsoluteCenterToZEdge = 20f;
	[Range (1f,10f)] public float ObjectScale = 1f;
	[Range (1f,50f)] public int MaxObjectsInScene = 10;
	public bool RandomInitialRotation = false;
	public bool Recycle; // Not yet implemented
	public string ResourceFolderPath = "Falling Object Prefabs";


	private List<GameObject> Prefabs = new List<GameObject>();
	private ConstantForce currentObjectCf;
	private Transform currentObjectScale;
	private Vector3 parentLocation;
	private GameObject instantiatedObject;
	private GameObject nextInst;
	private int prefabIndex;
	private int degrees;

//---------------------------------------------------------------------MONO METHODS:

	void Awake(){
		foreach(GameObject el in Resources.LoadAll(ResourceFolderPath, typeof(GameObject))){
                Prefabs.Add(el);
                }
	}

	void Start () {
		
		InvokeRepeating("InstantiateObjectAsChild", 0, SpawnDelay);
	}
	
	void Update(){
		parentLocation = gameObject.transform.position;
		int childCount = transform.childCount;

		if(childCount > MaxObjectsInScene){
			CancelInvoke();
		}

		if(childCount <= MaxObjectsInScene){
			InvokeRepeating("InstantiateObjectAsChild", 0, SpawnDelay);
		}

	}


	private void getRandomIndex(){
		prefabIndex = UnityEngine.Random.Range(0, Prefabs.Count);
	}

	private void setObjectScale(float objectScale){
		nextInst = Prefabs[prefabIndex];
		currentObjectScale = nextInst.GetComponent<Transform>();
		currentObjectScale.localScale = new Vector3(objectScale, objectScale, objectScale);

	}

	private void setGravityOnObject(){
		nextInst = Prefabs[prefabIndex];
		currentObjectCf = nextInst.GetComponent<ConstantForce>();
		currentObjectCf.force = new Vector3(0, -AbsoluteGravity, 0);
	}

	private void checkRotationBool(){
		if(RandomInitialRotation){
			degrees = 360;
		}
		else{
			degrees = 0;
		}
	}
	

	void InstantiateObjectAsChild() {

		int childCount = transform.childCount;
		getRandomIndex();
		setGravityOnObject();
		checkRotationBool();
		setObjectScale(ObjectScale);

		instantiatedObject = (GameObject)Instantiate(nextInst, 
		new Vector3(Random.Range(parentLocation.x - AbsoluteCenterToXEdge, parentLocation.x + AbsoluteCenterToXEdge),
		parentLocation.y,
		Random.Range(parentLocation.x - AbsoluteCenterToZEdge, parentLocation.x + AbsoluteCenterToZEdge)),
		Quaternion.Euler(Random.Range(0, degrees),  Random.Range(0, degrees), Random.Range(0, degrees)));
		instantiatedObject.gameObject.tag = "FallingObject"; 
		instantiatedObject.transform.parent = gameObject.transform;
	}

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }

}
