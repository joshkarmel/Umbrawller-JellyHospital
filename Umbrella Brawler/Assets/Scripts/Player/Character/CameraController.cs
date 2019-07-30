using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public class CameraController : MonoBehaviour 
{
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "CameraController";
	public bool VERBOSE = false;
	public Axes.Action XAxis, YAxis;
//---------------------------------------------------------------------------FIELDS:

	[System.NonSerialized]
	public PlayerController PC;
	[System.NonSerialized]
	public PlayerController_Net PCN;
	//[System.NonSerialized]
	public bool CanUseCamera = true;
	public GameObject CameraFocus; //For camera movement against walls
	public float
		CamDistance=5,
		CamXDistance=1,
		CamSensitivity=200,
		CamMaxAngle=80,
		CamMinAngle=-70,
		CamHeightFactor=1.2f,
		CamVertAngle=20,
		CamHorAngle=0;

//---------------------------------------------------------------------MONO METHODS:

	void Start() 
	{
		if(transform.parent.parent.GetComponent<PlayerController>() != null)
			PC = transform.parent.parent.GetComponent<PlayerController>();
		if(transform.parent.parent.GetComponent<PlayerController_Net>() != null)
			PCN = transform.parent.parent.GetComponent<PlayerController_Net>();
		
	}
		
	void LateUpdate()
	{
		if(PCN == null && PC == null)
		{
			Destroy(this.gameObject);
			return;
		}
		if(CanUseCamera)
		{
			CamHorAngle += CamSensitivity * Time.deltaTime * Input.GetAxis(Axes.toStr[XAxis]);

			CamVertAngle += CamSensitivity * Time.deltaTime * Input.GetAxis(Axes.toStr[YAxis]);
		}
		
		CamVertAngle = Mathf.Clamp(CamVertAngle, CamMinAngle, CamMaxAngle);

		Vector3 pos = CamDistance * Vector3.forward;

		pos = Quaternion.AngleAxis(-CamVertAngle, Vector3.right) * pos;
		pos = Quaternion.AngleAxis(CamHorAngle, Vector3.up) * pos;

		//make sure camera stays in front of walls
		float dist = CamDistance;
		//RaycastHit[] hits = Physics.RaycastAll(cameraFocus.transform.position, pos, camDistance);
		RaycastHit[] hits = Physics.SphereCastAll(CameraFocus.transform.position, 0.5f , pos, CamDistance, -1);
		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.tag == "Wall" || hit.transform.tag == "Player")
			{
				dist = Mathf.Min(dist, hit.distance);
			}
		}
				
		
		pos = pos.normalized * dist;
		float distanceRatio = (dist / CamDistance);
		//Vector3 focusDisplacement = new Vector3((CamXDistance ), distanceRatio, 0);
		//if(PCN != null && PC == null)
			//CameraFocus.transform.position = PCN.transform.position + (Vector3.up * CamHeightFactor) + focusDisplacement;
		//else if(PC != null)
			//CameraFocus.transform.position = PC.transform.position + (Vector3.up * CamHeightFactor) + focusDisplacement;

		
		transform.position = CameraFocus.transform.position+pos;
		transform.LookAt(CameraFocus.transform);
		
	}
	void Update()
    {

    }

//--------------------------------------------------------------------------METHODS:

//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }
}