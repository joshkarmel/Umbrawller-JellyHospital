using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyUtility;
using UnityEditor;
using Enums;

[RequireComponent(typeof(CharacterSetUp))]
public class MatchManager : MonoBehaviour
{



//------------------------------------------------------------------------CONSTANTS:
    private const string LOG_TAG = "MatchManager";
    public bool VERBOSE = false;
//---------------------------------------------------------------------------FIELDS:


 // The gameloop in GameManager will eventually go here instead.
 // Among other things. Stuff that keeps track of the match. Will have a GameModeObject!!!
    
 
private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
