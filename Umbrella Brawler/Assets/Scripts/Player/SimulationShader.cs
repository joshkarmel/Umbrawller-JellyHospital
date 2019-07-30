// Author: Zara Zahimi
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyUtility;

public class SimulationShader : MonoBehaviour {

//------------------------------------------------------------------------CONSTANTS:

	public bool VERBOSE = false;
	public bool StartWithSpacebar = false;

	const float MAX_BLEND_EFFECT = 1,
	 			MAX_EMISSION = 2, 
				MAX_BOTTOM_TO_TOP_PROGRESSION = 1,
				MAX_OUTLINE_THICKNESS = 30,
				MAX_OUTLINE_INTENSITY = 1000;

	const float MIN_BLEND_EFFECT = 0,
	 			MIN_EMISSION = 0, 
				MIN_BOTTOM_TO_TOP_PROGRESSION = 0,
				MIN_OUTLINE_THICKNESS = 2,
				MIN_OUTLINE_INTENSITY = 0;

	// Would be nice if these were structs or something, make em expandable. Too late
	const string BLEND_EFFECT = "_Blend_Effect",
				 EMISSION = "Vector1_296250AC", 
				 BOTTOM_TO_TOP_PROGRESSION = "_BottomToTop",
				 EDGE_COLOR = "_Color_Edge", 
				 MODEL_TEXTURE = "_Texture2D_Modle", 
				 MODEL_NORMAL_MAP = "Texture2D_6C1323EF",
				 WIREFRAME_TEXTURE = "_Texture2D_WireFrame",
				 OUTLINE_THICKNESS = "Vector1_A186252",
				 OUTLINE_COLOR = "Color_5E9A9E2D",
				 OUTLINE_INTENSITY = "Vector1_35935772",
				 OUTILNE_ON_OFF = "Boolean_411BA96C",
				 TEXTURE2D = "_SampleTexture2D_6866FF";
				 

//---------------------------------------------------------------------------FIELDS:

	public Material simulationMaterial;
	private Material simMatInst;

	public AnimationCurve IncreaseBlendEffectCurve,
							IncreaseEmissionCurve,
							IncreaseBottomToTopProgressionCurve,
							IncreaseOutlineThicknessCurve,
							IncreaseOutlineIntensityCurve;


	public AnimationCurve DecreaseBlendEffectCurve,
							DecreaseEmissionCurve,
							DecreaseBottomToTopProgressionCurve,
							DecreaseOutlineThicknessCurve,
							DecreaseOutlineIntensityCurve;

	[Range(1, 10000)] public float BlendEffectStepDivision = 20,
									EmissionStepDivision = 40,
									BottomToTopProgressionStepDivision = 20,
									OutlineThicknessStepDivision = 100,
									OutlineIntensityStepDivision = 1000;


	[System.Serializable] public enum SetBlendEffect{
		None,
		Increase,
		Decrease
	}
	[System.Serializable] public enum SetEmission{
		None,
		Increase,
		Decrease
	}
	[System.Serializable] public enum SetBottomToTopProgression{
		None,
		Increase,
		Decrease
	}
	[System.Serializable] public enum SetOutlineThickness{
		None,
		Increase,
		Decrease
	}
	[System.Serializable] public enum SetOutlineIntensity{
		None,
		Increase,
		Decrease
	}

	public SetBlendEffect selectedBlendEffect = SetBlendEffect.Increase;
	public SetEmission selectedEmission = SetEmission.Increase;
	public SetBottomToTopProgression selectedBottomToTopProgression = SetBottomToTopProgression.Increase;
	public SetOutlineThickness selectedOutlineThickness = SetOutlineThickness.Increase;
	public SetOutlineIntensity selectedOutlineIntensity = SetOutlineIntensity.Increase;

	public Color EdgeColor = Color.green;
	public Color OutlineColor = Color.red;
	public bool EnableOutline = true;
	public float InvokeRepeatingTimeStep = 0.02f;

	private float blendEffectStepSize,
					emissionStepSize,
					bottomToTopProgressionStepSize,
					outlineThicknessStepSize,
					outlineIntensityStepSize;

	private int blendEffectCurrentStep,
				emissionCurrentStep,
				bottomToTopProgressionCurrentStep,
				outlineThicknessCurrentStep,
				outlineIntensityCurrentStep;

	private float blendEffectILastFrameValue,
			emissionILastFrameValue,
			bottomToTopProgressionILastFrameValue,
			outlineThicknessILastFrameValue,
			outlineIntensityILastFrameValue;

	private float blendEffectDLastFrameValue,
			emissionDLastFrameValue,
			bottomToTopProgressionDLastFrameValue,
			outlineThicknessDLastFrameValue,
			outlineIntensityDLastFrameValue;

	private bool hasStarted = false;

	private bool blendEffectCurveEnded,
					emissionCurveEnded,
					bottomToTopProgressionCurveEnded,
					outlineThicknessCurveEnded,
					outlineIntensityCurveEnded;


	[HideInInspector] public float blendEffect,
									emission, 
									bottomToTopProgression, 
									outlineThickness, 
									outlineIntensity;
	
//---------------------------------------------------------------------MONO METHODS:

	void Start(){ 
		init();
		hasStarted = true;
	}

	void OnEnable(){
		if(hasStarted){
			init();
		}
	}

	void OnDisable(){
		CancelInvoke();
	}

//---------------------------------------------------------------------METHODS:

	private void init(){
		simMatInst = Instantiate(simulationMaterial);
		this.GetComponent<Renderer>().material = simMatInst;
		simMatInst.SetFloat(OUTILNE_ON_OFF, EnableOutline ? 1 : 0);

		setEdgeColor(EdgeColor);
		setOutlineColor(OutlineColor);
		setCurrentSteps(0);
		setCurveEnded(false);
		setInitialPropertyValues();
		calculateStepSizes();
		GetLastKeyFrameValues();

		if(isAllSetToNone()){
			DLog("All enums set to none. Disabling SimulationShader.cs");
			this.enabled = false;
			return;
		}

		if(!StartWithSpacebar){
			InvokeRepeating("RepeatingFunc", 0.0f, InvokeRepeatingTimeStep);
		}
	}

	private void Update(){
		if(StartWithSpacebar){
			if(Input.GetKeyDown(KeyCode.Space)){
				Debug.Log("SimShader INVOKED with Spacebar!");
				InvokeRepeating("RepeatingFunc", 0.0f, InvokeRepeatingTimeStep);
			}
		}
	}

	private bool isAllSetToNone(){
		if(selectedBlendEffect == SetBlendEffect.None &&
		selectedEmission == SetEmission.None &&
		selectedBottomToTopProgression == SetBottomToTopProgression.None &&
		selectedOutlineThickness == SetOutlineThickness.None &&
		selectedOutlineIntensity == SetOutlineIntensity.None){
			return true;
		}
		return false;
	}

	private void setCurrentSteps(int num){
		emissionCurrentStep = num;
		blendEffectCurrentStep = num;
		bottomToTopProgressionCurrentStep = num;
		outlineThicknessCurrentStep = num;
		outlineIntensityCurrentStep = num;
	}

	private void setCurveEnded(bool curveEnded){
		blendEffectCurveEnded = curveEnded;
		emissionCurveEnded = curveEnded;
		bottomToTopProgressionCurveEnded = curveEnded;
		outlineThicknessCurveEnded = curveEnded;
		outlineIntensityCurveEnded = curveEnded;
	}

	private void setInitialPropertyValues(){
		if(selectedBlendEffect == SetBlendEffect.Increase){
			blendEffect = MIN_BLEND_EFFECT; 
			simMatInst.SetFloat(BLEND_EFFECT, MIN_BLEND_EFFECT);
		}
		if(selectedBlendEffect == SetBlendEffect.Decrease){
			blendEffect = MAX_BLEND_EFFECT;
			simMatInst.SetFloat(BLEND_EFFECT, MAX_BLEND_EFFECT);
		}
		if(selectedEmission == SetEmission.Increase){
			emission = MIN_EMISSION;
			simMatInst.SetFloat(EMISSION, MIN_EMISSION);
		}
		if(selectedEmission == SetEmission.Decrease){
			emission = MAX_EMISSION;
			simMatInst.SetFloat(EMISSION, MAX_EMISSION); 
		}

		if(selectedBottomToTopProgression == SetBottomToTopProgression.Increase){
			bottomToTopProgression = MIN_BOTTOM_TO_TOP_PROGRESSION;
			simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MIN_BOTTOM_TO_TOP_PROGRESSION);
		}
		if(selectedBottomToTopProgression == SetBottomToTopProgression.Decrease){
			bottomToTopProgression = MAX_BOTTOM_TO_TOP_PROGRESSION;
			simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MAX_BOTTOM_TO_TOP_PROGRESSION); 
		}

		if(selectedOutlineThickness == SetOutlineThickness.Increase){
			outlineThickness = MIN_OUTLINE_THICKNESS;
			simMatInst.SetFloat(OUTLINE_THICKNESS, MIN_OUTLINE_THICKNESS);
		}
		if(selectedOutlineThickness == SetOutlineThickness.Decrease){
			outlineThickness = MAX_OUTLINE_THICKNESS;
			simMatInst.SetFloat(OUTLINE_THICKNESS, MAX_OUTLINE_THICKNESS); 
		}

		if(selectedOutlineIntensity == SetOutlineIntensity.Increase){
			outlineIntensity = MIN_OUTLINE_INTENSITY;
			simMatInst.SetFloat(OUTLINE_INTENSITY, MIN_OUTLINE_INTENSITY);
		}
		if(selectedOutlineIntensity == SetOutlineIntensity.Decrease){
			outlineIntensity = MAX_OUTLINE_INTENSITY;
			simMatInst.SetFloat(OUTLINE_INTENSITY, MAX_OUTLINE_INTENSITY); 
		}
	}

	private void RepeatingFunc(){

		if(selectedBlendEffect != SetBlendEffect.None){
			adjustBlendEffect();			
		}

		if(selectedEmission != SetEmission.None){
			adjustEmission();			
		}

		if(selectedBottomToTopProgression != SetBottomToTopProgression.None){
			adjustBottomToTopProgression();			
		}

		if(selectedOutlineThickness != SetOutlineThickness.None){
			adjustOutlineThickness();			
		}

		if(selectedOutlineIntensity != SetOutlineIntensity.None){
			adjustOutlineIntensity();			
		}

		if(checkAllReachedLastKey()){
			DLog("All AnimationCurves have completed. Calling CancellInvoke()");
			CancelInvoke();
		}
	}

	private bool checkAllReachedLastKey(){
		if(blendEffectCurveEnded &&
			emissionCurveEnded &&
			bottomToTopProgressionCurveEnded &&
			outlineThicknessCurveEnded &&
			outlineIntensityCurveEnded){
			return true;
		}
		return false;
	}

	private bool checkAllMax(){
		if(simMatInst.GetFloat(BLEND_EFFECT) == MAX_BLEND_EFFECT &&
		simMatInst.GetFloat(EMISSION) == MAX_EMISSION &&
		simMatInst.GetFloat(BOTTOM_TO_TOP_PROGRESSION) == MAX_BOTTOM_TO_TOP_PROGRESSION &&
		simMatInst.GetFloat(OUTLINE_THICKNESS) == MAX_OUTLINE_THICKNESS &&
		simMatInst.GetFloat(OUTLINE_INTENSITY) == MAX_OUTLINE_INTENSITY){
			return true;
		}
		return false;
	}

	private bool checkAllMin(){
		if(simMatInst.GetFloat(BLEND_EFFECT) == MIN_BLEND_EFFECT &&
		simMatInst.GetFloat(EMISSION) == MIN_EMISSION &&
		simMatInst.GetFloat(BOTTOM_TO_TOP_PROGRESSION) == MIN_BOTTOM_TO_TOP_PROGRESSION &&
		simMatInst.GetFloat(OUTLINE_THICKNESS) == MIN_OUTLINE_THICKNESS &&
		simMatInst.GetFloat(OUTLINE_INTENSITY) == MIN_OUTLINE_INTENSITY){
			return true;
		}
		return false;
	}

	private void calculateStepSizes(){
		blendEffectStepSize = MAX_BLEND_EFFECT / BlendEffectStepDivision;
		DLog("Blend effect step size: " + blendEffectStepSize);

		emissionStepSize = MAX_EMISSION / EmissionStepDivision;
		DLog("Emission step size: " + emissionStepSize);

		bottomToTopProgressionStepSize = MAX_BOTTOM_TO_TOP_PROGRESSION / BottomToTopProgressionStepDivision;
		DLog("BottomToTopProgression step size: " + bottomToTopProgressionStepSize);

		outlineThicknessStepSize = MAX_OUTLINE_THICKNESS / OutlineThicknessStepDivision;
		DLog("OutlineThickness step size: " + outlineThicknessStepSize);

		outlineIntensityStepSize = MAX_OUTLINE_INTENSITY / OutlineIntensityStepDivision;
		DLog("OutlineIntensity step size: " + outlineIntensityStepSize);
		
	}

	private float getCurveEvalFromCurrentStep(float currentStep, float totalSteps, float multiplier){
		if(currentStep >= 0){
			float curveStep = (currentStep / totalSteps) * multiplier;
			return curveStep;
		}
		else{ 
			DLog("Current step is less than 0. Returning 0");
			return 0f;
		}
	}

	private void GetLastKeyFrameValues(){

		blendEffectILastFrameValue = IncreaseBlendEffectCurve[IncreaseBlendEffectCurve.length - 1].value;
		emissionILastFrameValue = IncreaseEmissionCurve[IncreaseEmissionCurve.length - 1].value;
		bottomToTopProgressionILastFrameValue = IncreaseBottomToTopProgressionCurve[IncreaseBottomToTopProgressionCurve.length - 1].value;
		outlineThicknessILastFrameValue = IncreaseOutlineThicknessCurve[IncreaseOutlineThicknessCurve.length - 1].value;
		outlineIntensityILastFrameValue = IncreaseOutlineIntensityCurve[IncreaseOutlineIntensityCurve.length - 1].value;

		blendEffectDLastFrameValue = DecreaseBlendEffectCurve[DecreaseBlendEffectCurve.length - 1].value;
		emissionDLastFrameValue = DecreaseEmissionCurve[DecreaseEmissionCurve.length - 1].value;
		bottomToTopProgressionDLastFrameValue = DecreaseBottomToTopProgressionCurve[DecreaseBottomToTopProgressionCurve.length - 1].value;
		outlineThicknessDLastFrameValue = DecreaseOutlineThicknessCurve[DecreaseOutlineThicknessCurve.length - 1].value;
		outlineIntensityDLastFrameValue = DecreaseOutlineIntensityCurve[DecreaseOutlineIntensityCurve.length - 1].value;
	}

	public void setEdgeColor(Color col){
		simMatInst.SetColor(EDGE_COLOR, col);
	}

	public void setOutlineColor(Color col){
		simMatInst.SetColor(OUTLINE_COLOR, col);
	}

	private void adjustBlendEffect(){
		blendEffectCurrentStep++;
		if(blendEffect <= MAX_BLEND_EFFECT && (selectedBlendEffect == SetBlendEffect.Increase)){
			float increment = IncreaseBlendEffectCurve.Evaluate(getCurveEvalFromCurrentStep(blendEffectCurrentStep, BlendEffectStepDivision, blendEffectStepSize));
			blendEffect = increment;
			simMatInst.SetFloat(BLEND_EFFECT, blendEffect);
			
			if(increment == blendEffectILastFrameValue){
				blendEffectCurveEnded = true;
			}
		}
		if(blendEffect >= MIN_BLEND_EFFECT && (selectedBlendEffect == SetBlendEffect.Decrease)){ 
			float decrement = DecreaseBlendEffectCurve.Evaluate(getCurveEvalFromCurrentStep(blendEffectCurrentStep, BlendEffectStepDivision, blendEffectStepSize));
			blendEffect = decrement;
			simMatInst.SetFloat(BLEND_EFFECT, blendEffect);
			
			if(decrement == blendEffectDLastFrameValue){
				blendEffectCurveEnded = true;
			}
		}

		if(blendEffect <= MIN_BLEND_EFFECT || blendEffect >= MAX_BLEND_EFFECT){
			if(selectedBlendEffect == SetBlendEffect.Decrease){
				blendEffect = MIN_BLEND_EFFECT;
				simMatInst.SetFloat(BLEND_EFFECT, MIN_BLEND_EFFECT);
			}
			if(selectedBlendEffect == SetBlendEffect.Increase){
				if(blendEffect == MIN_BLEND_EFFECT){
					simMatInst.SetFloat(BLEND_EFFECT, MIN_BLEND_EFFECT);
					
				}else{
					blendEffect = MAX_BLEND_EFFECT;
					simMatInst.SetFloat(BLEND_EFFECT, MAX_BLEND_EFFECT);
				}
			}
		}

		// if(bottomToTopProgression <= MIN_BOTTOM_TO_TOP_PROGRESSION || bottomToTopProgression >= MAX_BOTTOM_TO_TOP_PROGRESSION){
		// 	if(selectedBottomToTopProgression == SetBottomToTopProgression.Decrease){
		// 		if(bottomToTopProgression == MAX_BLEND_EFFECT){
		// 			simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MAX_BLEND_EFFECT);
		// 		}else{
		// 			bottomToTopProgression = MIN_BOTTOM_TO_TOP_PROGRESSION;
		// 			simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MIN_BOTTOM_TO_TOP_PROGRESSION);
		// 		}
		// 	}
		// 	if(selectedBottomToTopProgression == SetBottomToTopProgression.Increase){
		// 		bottomToTopProgression = MAX_BOTTOM_TO_TOP_PROGRESSION;
		// 		simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MAX_BOTTOM_TO_TOP_PROGRESSION);
		// 	}
		// }
	}


	private void adjustEmission(){
		emissionCurrentStep++;
		if(emission <= MAX_EMISSION && (selectedEmission == SetEmission.Increase)){
			float increment = IncreaseEmissionCurve.Evaluate(getCurveEvalFromCurrentStep(emissionCurrentStep, EmissionStepDivision, emissionStepSize));
			emission = increment;
			simMatInst.SetFloat(EMISSION, emission);
			
			if(increment == emissionILastFrameValue){
				emissionCurveEnded = true;
			}
		}
		if(emission >= MIN_EMISSION && (selectedEmission == SetEmission.Decrease)){ 
			float decrement = DecreaseEmissionCurve.Evaluate(getCurveEvalFromCurrentStep(emissionCurrentStep, EmissionStepDivision, emissionStepSize));
			emission = decrement;
			simMatInst.SetFloat(EMISSION, emission);
		
			if(decrement == emissionDLastFrameValue){
				emissionCurveEnded = true;
			}
		}

		if(emission <= MIN_EMISSION || emission >= MAX_EMISSION){
			if(selectedEmission == SetEmission.Decrease){
				emission = MIN_EMISSION;
				simMatInst.SetFloat(EMISSION, MIN_EMISSION);
			}
			if(selectedEmission == SetEmission.Increase){
				emission = MAX_EMISSION;
				simMatInst.SetFloat(EMISSION, MAX_EMISSION);
			}
		}
	}

	private void adjustBottomToTopProgression(){
		bottomToTopProgressionCurrentStep++;
		if(bottomToTopProgression <= MAX_BOTTOM_TO_TOP_PROGRESSION && (selectedBottomToTopProgression == SetBottomToTopProgression.Increase)){
			float increment = IncreaseBottomToTopProgressionCurve.Evaluate(getCurveEvalFromCurrentStep(bottomToTopProgressionCurrentStep, BottomToTopProgressionStepDivision, bottomToTopProgressionStepSize));
			bottomToTopProgression = increment;
			simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, bottomToTopProgression);
			
			if(increment == bottomToTopProgressionILastFrameValue){
				bottomToTopProgressionCurveEnded = true;
			}
		}
		if(bottomToTopProgression >= MIN_BOTTOM_TO_TOP_PROGRESSION && (selectedBottomToTopProgression == SetBottomToTopProgression.Decrease)){ 
			float decrement = DecreaseBottomToTopProgressionCurve.Evaluate(getCurveEvalFromCurrentStep(bottomToTopProgressionCurrentStep, BottomToTopProgressionStepDivision, bottomToTopProgressionStepSize));
			bottomToTopProgression = decrement;
			simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, bottomToTopProgression);
			
			if(decrement == bottomToTopProgressionDLastFrameValue){
				bottomToTopProgressionCurveEnded = true;
			}
		}

		if(bottomToTopProgression <= MIN_BOTTOM_TO_TOP_PROGRESSION || bottomToTopProgression >= MAX_BOTTOM_TO_TOP_PROGRESSION){
			if(selectedBottomToTopProgression == SetBottomToTopProgression.Decrease){
				if(bottomToTopProgression == MAX_BLEND_EFFECT){
					simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MAX_BLEND_EFFECT);
				}else{
					bottomToTopProgression = MIN_BOTTOM_TO_TOP_PROGRESSION;
					simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MIN_BOTTOM_TO_TOP_PROGRESSION);
				}
			}
			if(selectedBottomToTopProgression == SetBottomToTopProgression.Increase){
				bottomToTopProgression = MAX_BOTTOM_TO_TOP_PROGRESSION;
				simMatInst.SetFloat(BOTTOM_TO_TOP_PROGRESSION, MAX_BOTTOM_TO_TOP_PROGRESSION);
			}
		}
	}

	private void adjustOutlineThickness(){
		outlineThicknessCurrentStep++;
		if(outlineThickness <= MAX_OUTLINE_THICKNESS && (selectedOutlineThickness == SetOutlineThickness.Increase)){
			float increment = IncreaseOutlineThicknessCurve.Evaluate(getCurveEvalFromCurrentStep(outlineThicknessCurrentStep, OutlineThicknessStepDivision, outlineThicknessStepSize));
			outlineThickness = increment;
			simMatInst.SetFloat(OUTLINE_THICKNESS, outlineThickness);
			
			if(increment == outlineThicknessILastFrameValue){
				outlineThicknessCurveEnded = true;
			}
		}
		if(outlineThickness >= MIN_OUTLINE_THICKNESS && (selectedOutlineThickness == SetOutlineThickness.Decrease)){ 
			float decrement = DecreaseOutlineThicknessCurve.Evaluate(getCurveEvalFromCurrentStep(outlineThicknessCurrentStep, OutlineThicknessStepDivision, outlineThicknessStepSize));
			outlineThickness = decrement;
			simMatInst.SetFloat(OUTLINE_THICKNESS, outlineThickness);
			
			if(decrement == outlineThicknessDLastFrameValue){
				outlineThicknessCurveEnded = true;
			}
		}

		if(outlineThickness <= MIN_OUTLINE_THICKNESS || outlineThickness >= MAX_OUTLINE_THICKNESS){
			if(selectedOutlineThickness == SetOutlineThickness.Decrease){
				outlineThickness = MIN_OUTLINE_THICKNESS;
				simMatInst.SetFloat(OUTLINE_THICKNESS, MIN_OUTLINE_THICKNESS);
			}
			if(selectedOutlineThickness == SetOutlineThickness.Increase){
				if(outlineThickness == MIN_OUTLINE_THICKNESS){
					simMatInst.SetFloat(OUTLINE_THICKNESS, MIN_OUTLINE_THICKNESS);
				}else{
					outlineThickness = MAX_OUTLINE_THICKNESS;
					simMatInst.SetFloat(OUTLINE_THICKNESS, MAX_OUTLINE_THICKNESS);
				}
			}
		}
	}

	private void adjustOutlineIntensity(){
		outlineIntensityCurrentStep++;
		if(outlineIntensity <= MAX_OUTLINE_INTENSITY && (selectedOutlineIntensity == SetOutlineIntensity.Increase)){
			float increment = IncreaseOutlineIntensityCurve.Evaluate(getCurveEvalFromCurrentStep(outlineIntensityCurrentStep, OutlineIntensityStepDivision, outlineIntensityStepSize));
			outlineIntensity = increment;
			simMatInst.SetFloat(OUTLINE_INTENSITY, outlineIntensity);
		
			if(increment == outlineIntensityILastFrameValue){
				outlineIntensityCurveEnded = true;
			}
		}
		if(outlineIntensity >= MIN_OUTLINE_INTENSITY && (selectedOutlineIntensity == SetOutlineIntensity.Decrease)){ 
			float decrement = DecreaseOutlineIntensityCurve.Evaluate(getCurveEvalFromCurrentStep(outlineIntensityCurrentStep, OutlineIntensityStepDivision, outlineIntensityStepSize));
			outlineIntensity = decrement;
			simMatInst.SetFloat(OUTLINE_INTENSITY, outlineIntensity);
			
			if(decrement == outlineIntensityDLastFrameValue){
				outlineIntensityCurveEnded = true;
			}
		}

		if(outlineIntensity <= MIN_OUTLINE_INTENSITY || outlineIntensity >= MAX_OUTLINE_INTENSITY){
			if(selectedOutlineIntensity == SetOutlineIntensity.Decrease){
				outlineIntensity = MIN_OUTLINE_INTENSITY;
				simMatInst.SetFloat(OUTLINE_INTENSITY, MIN_OUTLINE_INTENSITY);
			}
			if(selectedOutlineIntensity == SetOutlineIntensity.Increase){
				if(outlineIntensity == MIN_OUTLINE_INTENSITY){
					simMatInst.SetFloat(OUTLINE_INTENSITY, MIN_OUTLINE_INTENSITY);
				}else{
					outlineIntensity = MAX_OUTLINE_INTENSITY;
					simMatInst.SetFloat(OUTLINE_INTENSITY, MAX_OUTLINE_INTENSITY);
				}
			}
		}
	}



	//--------------------------------------------------------------------------HELPERS:
	
    private void DLog( string message )
    {
        if( VERBOSE )   this.GetType().Name.TPrint( message );        
    }
}
