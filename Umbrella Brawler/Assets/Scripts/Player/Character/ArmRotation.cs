using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour 
{
	public CameraController Cam;
	public LayerMask LayerMask;
	private PlayerController playerController;

	void Start()
	{
		playerController = Cam.transform.root.GetComponent<PlayerController>();
	}
	void LateUpdate()
	{
        try
        {
            if (playerController.CanMove)
            {
                Transform retTransform = Cam.transform;
                RaycastHit hit;
                Ray ray = new Ray(retTransform.position, retTransform.forward);
                if (Physics.Raycast(ray, out hit, 1000, LayerMask))
                {

                    Vector3 newPlayerRotLoc = hit.point;
                    //Debug.DrawLine(transform.position, newPlayerRotLoc, Color.red, 5);
                    //Debug.Log(LayerMask.LayerToName(hit.transform.gameObject.layer));

                    //Arm can go crazy depending on the animation state of the character
                    //And the rotation of the player
                    transform.LookAt(newPlayerRotLoc);
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
	}	 	
}
