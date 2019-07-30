using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/////DONT USE!!! SCRIPT IS OBSOLETE!!!!!!!//////////////////
[CreateAssetMenu(menuName = "AI/StateMachine/Actions/CloseUmbrellaAction")]
public class CloseUmbrellaAction : AIAction {

	public override void Act(StateController stateController){
	//	Debug.Log("I TOLD YOU NOT TO USE THIS ACTION!!!!!!!!!!!!!!!!!!");

		
		CloseUmbrella(stateController);
		Debug.Log("calling closssss umbrella act");
	
	}

	// remember do this only once
	private void CloseUmbrella(StateController stateController){
		
			Debug.Log("CLOSE UMBRELLA");
			if(stateController.PlayerControllerAI.Umbrella.IsUmbrellaOpen){
				toggleUmbrella(stateController);
			}
			
		}
	

	private void toggleUmbrella(StateController stateController){
		if(stateController.PlayerControllerAI.CanUseUmbrella){
		stateController.PlayerControllerAI.UmbrellaState(true);
			// if(stateController.PlayerControllerAI.Umbrella.CanActivateShield){
			// 	stateController.PlayerControllerAI.ShieldState(true);
			// }
		}		
	}

	// private void CloseUmbrella(StateController stateController){
	// 	if(stateController.PlayerControllerAI.Umbrella.IsUmbrellaOpen){
		
	// 		stateController.timer += Time.deltaTime;
	// 	//	Debug.Log("insidecloseumbrella action, timer is at" + stateController.timer);

	// 		if(stateController.timer >= 5.0f){
	// 			Debug.Log("LOOK! UMBRELLA SHOULD CLOSE WHEN YOU SEE THIS MESSAGE!");
			


	
	// 			if(stateController.PlayerControllerAI.CanUseUmbrella){
	// 				stateController.PlayerControllerAI.UmbrellaState(true);

	// 				// if(stateController.PlayerControllerAI.Umbrella.CanActivateShield){
	// 				// 	stateController.PlayerControllerAI.ShieldState(true);
	// 				// }
	// 			}
	// 		}
			
			
	// 	}
		
	// }

	//private void DelayClose(){}
}
