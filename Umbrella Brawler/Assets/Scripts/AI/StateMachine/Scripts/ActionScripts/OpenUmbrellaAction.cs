using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/StateMachine/Actions/OpenUmbrella")]
public class OpenUmbrellaAction : AIAction {

	public override void Act(StateController stateController){
		stateController.SetAIGravity(true);
		stateController.SetAICanMove(true);
		stateController.SetAIIsKinematic(false);
		bool timeElapsed = false;

		SetUmbrellaVariables(stateController);
		OpenUmbrella(stateController);
		Debug.Log("calling open umbrella act");
		if(stateController.PlayerControllerAI.Umbrella.IsUmbrellaOpen){
			DelayClose(timeElapsed, stateController);
		}
	
	}

	// remember do this only once
	private void SetUmbrellaVariables(StateController stateController){
		if(!stateController.setVariablesOnce){
			stateController.setVariablesOnce = true;
		}
	}

	private void OpenUmbrella(StateController stateController){

		if(!stateController.PlayerControllerAI.Umbrella.IsUmbrellaOpen){
			if(!stateController.doActionOnce){
			//bool timeElapsed = false;

				toggleUmbrella(stateController);
				stateController.doActionOnce = true;
			}
		}
	}

	private void DelayClose(bool timeElapsed, StateController stateController){

		timeElapsed = stateController.CheckTimeElapsedInState(stateController.LocalAIVariables.UmbrellaOpenDuration);

		if(timeElapsed){
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
}
