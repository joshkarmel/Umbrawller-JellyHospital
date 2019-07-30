
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
[CreateAssetMenu(menuName = "AI/StateMachine/Actions/ShootAction")]
public class ShootAction : AIAction {

	public override void Act(StateController stateController){
		
		 stateController.SetAIGravity(true);
		 stateController.SetAICanMove(true);
		 stateController.SetAIIsKinematic(false);
		Shoot(stateController);
	
	}
	private void Shoot(StateController stateController){

		Debug.Log("yer currently in pursue action i hope");
		
		if(Input.GetMouseButton(0)){
			stateController.WeaponAI.tempshoot(true);
		}


	//	DelayShoot(stateController);

			Debug.Log("waiting to  shoot...");
			bool timeElapsed = false;

		timeElapsed = stateController.CheckTimeElapsedInState(1.5f);

		if(timeElapsed){
			// stateController.FocusArea.rotation = Quaternion.Slerp(stateController.FocusArea.rotation, stateController.GoToTarget.transform.rotation, Time.deltaTime);
			stateController.FocusArea.transform.LookAt(stateController.GoToTarget.transform);
			stateController.WeaponAI.tempshoot(true);
			timeElapsed = false;
		}

 
	}

	private void DelayShoot(StateController stateController){
		// stateController.SetAICanMove(true);
		// stateController.SetAIGravity(true);
		// stateController.SetAIIsKinematic(false);
			Debug.Log("waiting to  shoot...");
			bool timeElapsed = false;

		timeElapsed = stateController.CheckTimeElapsedInState(1.5f);

		if(timeElapsed){
			// stateController.FocusArea.rotation = Quaternion.Slerp(stateController.FocusArea.rotation, stateController.GoToTarget.transform.rotation, Time.deltaTime);
			stateController.FocusArea.transform.LookAt(stateController.GoToTarget.transform);
			stateController.WeaponAI.tempshoot(true);
			timeElapsed = false;
		}

	}
}