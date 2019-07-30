using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


[System.Serializable]
public class Transition {

//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "Transition";
	public bool VERBOSE = false;
	public bool DisableTransition = false;

//---------------------------------------------------------------------------FIELDS:
	public Decision Decision;
	public State trueState;
	public State falseState;

//--------------------------------------------------------------------------HELPERS:

	public bool returnTransitionEnabled()
    {
        if(DisableTransition){
			return true;
		}
		else{
			return false;
		}
    }

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}
