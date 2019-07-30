using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;

public class TwoPlayers : MonoBehaviour {

    //------------------------------------------------------------------------CONSTANTS:

    private const string LOG_TAG = "TwoPlayers";
    public bool VERBOSE = false;

    //---------------------------------------------------------------------------FIELDS:

    public Text devText;

    //---------------------------------------------------------------------MONO METHODS:

    void Start()
    {
#if UNITY_EDITOR
    devText.enabled = true;
    devText.text += "\n" + Utility.GetEnumDescription(GameManager.Instance.GameMode);
#endif
    }

    void Update()
    {

    }

    //--------------------------------------------------------------------------METHODS:

    //--------------------------------------------------------------------------HELPERS:

    private void DLog(string message)
    {
        if (VERBOSE) LOG_TAG.TPrint(message);
    }
}

