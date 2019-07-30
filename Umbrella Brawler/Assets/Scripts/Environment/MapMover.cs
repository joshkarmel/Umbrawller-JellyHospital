using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public class MapMover : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	public float MaxDistance, Timer = 10, Magnitude, MaxSpeed, MinSpeed;

	[Range(0, 30)]
	public float MovingVelocity;
	public Vector3 CenterLocation;
	public GameObject TestObject, TestObject2;
	public GameObject[] MovingObjects;
	private List<PlayerManager> players;
	private GameObject[] playersLocations;
	private bool firstFrame = false;

	private Vector3 fakeCenterLocation;
	private Rigidbody movingRB;
	
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		StartCoroutine( LateGet());
		
	}
		
	void Update()
    {
		if(firstFrame && Time.timeSinceLevelLoad >= Timer)
			GetAverageDistance();

		
    }

//--------------------------------------------------------------------------METHODS:
	public void GetAverageDistance()
	{
		CenterLocation = Utility.FindCenter(playersLocations);
		Magnitude = FindYDifference(playersLocations);
		TestObject.transform.position = CenterLocation;
		if(CheckIfCentered())
		{		
			MoveObjects();
		}
	}

	public void MoveObjects()
	{
		DLog("Object moving");
		movingRB.isKinematic = false;
		//for(int i = 0; i< MovingObjects.Length; i++)
		//{
			//if( i == 0)
				movingRB.velocity = Vector3.down * Time.deltaTime * MovingVelocity * VelocityFluctiation();
			//else
			//{
			//	movingRB.velocity = Vector3.zero;
				// MovingObjects[i].transform.position = new Vector3(MovingObjects[i].transform.position.x,
			 	// 											CenterLocation.y,
			 	// 											MovingObjects[i].transform.position.z);
			//}
		//}
	}
	public float FindYDifference(GameObject[] gameObjects)
	{
		float sum = 0;
		foreach( GameObject gameObject in gameObjects )
		{
			if(sum == 0)
				sum = gameObject.transform.position.y;
			else
				sum -= gameObject.transform.position.y;
		}

		if( gameObjects.Length > 0 )
		{
			return sum;
		}
		return 0;
	}
//--------------------------------------------------------------------------HELPERS:
	private bool CheckIfCentered()
	{
		if(Magnitude >= MaxDistance)
		{
			DLog("Not moving");
			movingRB.velocity = Vector3.zero;
			return false;
		}
		else
		{
			DLog("Moving");		
			return true;
		}
	}

	/// <Summary>
	/// Create a value in between max and min speed depending on the distance
	/// of all players
	/// The farther away, the slower the value
	/// Makes the game more dynamic
	/// </Summary>
	private float VelocityFluctiation()
	{
		float sum = 0;

		sum = Mathf.Abs(MaxDistance - Magnitude);

		if(sum >= MaxSpeed)	
			sum = MaxSpeed;
		else if(sum <= MinSpeed)
			sum = MinSpeed;
		DLog("Extraspeed: " +sum);
		return sum;
	}

	private IEnumerator LateGet()
	{
		yield return new WaitForEndOfFrame();
		players = GameManager.Instance.Players;
		playersLocations = new GameObject[players.Count];
		movingRB = MovingObjects[0].GetComponent<Rigidbody>();

		for (int i = 0; i < players.Count; i++)
		{
			playersLocations[i] = players[i].m_PlayerInstance;
		}
		firstFrame = true;
	}
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
