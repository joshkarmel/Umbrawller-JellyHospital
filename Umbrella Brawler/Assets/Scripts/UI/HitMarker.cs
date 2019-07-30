using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour {
//------------------------------------------------------------------------CONSTANTS:

//---------------------------------------------------------------------------FIELDS:
	public Enums.Player PlayerID;
	public PlayerInformation PInfo;
	public Image ReticleImage;
	public bool isHit;
	public float timer;

//---------------------------------------------------------------------MONO METHODS:
	void Start () 
	{
		SetReticlePlayerID(PInfo.PlayerNumber);
	}

	void Update () 
	{
		if (isHit) 
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				ReticleImage.color = Color.white;
				isHit = false;
			}
		}
	}

//--------------------------------------------------------------------------METHODS:
	public void HitEnemy ()
	{
		ReticleImage.color = Color.red;
		isHit = true;
		timer = 0.5f;
	}

//--------------------------------------------------------------------------HELPERS:
	public void SetReticlePlayerID(Enums.Player player)
    {
        PlayerID = player;
    }
}
