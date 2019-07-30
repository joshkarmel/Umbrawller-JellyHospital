using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS

public class PortalSystem : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
	[Tooltip ("The other side of the portal")]
	public GameObject Exit; 

	private Vector3 offset;
//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
	}
		
	void Update()
    {

    }
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player" || col.tag == "projectile")
		{
			offset = new Vector3(col.transform.position.x, Exit.transform.position.y, col.transform.position.z);
			col.transform.position = offset;
		}
	}
//--------------------------------------------------------------------------METHODS:
	
//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}