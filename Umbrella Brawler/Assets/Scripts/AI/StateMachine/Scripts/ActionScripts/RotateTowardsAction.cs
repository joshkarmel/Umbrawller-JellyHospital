
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
[CreateAssetMenu(menuName = "AI/StateMachine/Actions/RotateTowardsAction")]
public class RotateTowardsAction : AIAction {

	  float speed = 10; // angular speed. make local ai var

	public override void Act(StateController stateController){
		rotateTowards(stateController);
	
	}
	private void rotateTowards(StateController stateController){
		 Vector3 targetDir = -stateController.LastShotFromNormVector;

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(stateController.transform.forward, targetDir, step, 0.0f);

        // Move our position a step closer to the target.
        stateController.transform.rotation = Quaternion.LookRotation(newDir);

 
	}

}