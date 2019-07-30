using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetUp : MonoBehaviour 
{
	public Camera Cam;
	public Material CameraMaterial;

	// Use this for initialization
	void Start () 
	{
		if(Cam.targetTexture != null)
		{
			Cam.targetTexture.Release();
		}
		Cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
		CameraMaterial.mainTexture = Cam.targetTexture;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
