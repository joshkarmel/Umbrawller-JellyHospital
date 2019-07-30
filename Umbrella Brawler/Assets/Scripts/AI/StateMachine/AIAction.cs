using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public abstract class AIAction : ScriptableObject {

//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "AIAction";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:

	public abstract void Act(StateController stateController);

//--------------------------------------------------------------------------HELPERS:

private void DLog( string message )
{
	if( VERBOSE )   LOG_TAG.TPrint( message );        
}
}
