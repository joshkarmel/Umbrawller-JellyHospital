
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
[CreateAssetMenu(menuName = "AI/StateMachine/Actions/BetterShootAction")]
public class BetterShootAction : AIAction {

	public override void Act(StateController stateController){
		
		//  stateController.SetAIGravity(true);
		//  stateController.SetAICanMove(true);
		//  stateController.SetAIIsKinematic(false);
		TempShoot(stateController);
	
	}
	private void TempShoot(StateController stateController){
		Debug.Log("we did the shoot forward thing");

		// pu this in yerm
		//stateController.FocusArea.transform.LookAt(stateController.GoToTarget.transform);
		stateController.WeaponAI.AICheckShootWrapper(false, true);
 
	}

}