using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

//------------------------------------------------------------------------CONSTANTS:

private const string LOG_TAG = "Oscillator";
public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:

    public Vector3 movementVector =  new Vector3(10f, 10f, 10f);
    [Range(1,100)] public float period = 2f;

    float movementFactor;
    Vector3 startingPos;

//---------------------------------------------------------------------MONO METHODS:

	// Use this for initialization
	void Start () {
                startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
                // set movement factor automatically
                //todo protect against period is zero
                if (period <= Mathf.Epsilon) { return; }
                float cycles = Time.time / period; // grows continually from 0

                float sinWave = Mathf.Sin(cycles * 2 * Mathf.PI); //goes from -1 to +1

                movementFactor = sinWave / 2f + 0.5f;

                Vector3 offset = movementFactor * movementVector;
                transform.position = startingPos + offset;
	}

//--------------------------------------------------------------------------HELPERS:

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
