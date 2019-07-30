using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[CreateAssetMenu(menuName = "AI/StateMachine/State")]
public class State : ScriptableObject {
//------------------------------------------------------------------------CONSTANTS:

	private const string LOG_TAG = "State";
	public bool VERBOSE = false;

//---------------------------------------------------------------------------FIELDS:
    public AIAction[] Actions;
    public Transition[] Transitions;
    public Color SceneFocusAreaColor = Color.grey;

//--------------------------------------------------------------------------METHODS:

    public void UpdateState(StateController stateController){
        ExecuteAction(stateController);
        CheckTransitions(stateController);
    }

    private void ExecuteAction(StateController stateController){
        for(int i = 0; i < Actions.Length; i++){
            Actions[i].Act(stateController);
        }
    }

    private void CheckTransitions(StateController stateController){
        for(int i = 0; i < Transitions.Length; i++){
            if(!Transitions[i].DisableTransition){
                bool decisionMade = Transitions[i].Decision.Decide(stateController);

                if(decisionMade){
                    stateController.TransitionToNextState(Transitions[i].trueState);
                }
                else{
                    stateController.TransitionToNextState(Transitions[i].falseState);
                }
            }
        }

    }
//--------------------------------------------------------------------------HELPERS:

    private void DLog( string message )
    {
        if( VERBOSE )   LOG_TAG.TPrint( message );        
    }

}
