using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public abstract class Decision : ScriptableObject {

//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "Decision";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------METHODS:

	public abstract bool Decide(StateController stateController);

//--------------------------------------------------------------------------HELPERS:
	private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}
