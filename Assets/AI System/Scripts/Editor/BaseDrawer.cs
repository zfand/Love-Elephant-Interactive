using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

public class BaseDrawer : PropertyDrawer {
	private string[] triggerNames;
	private string[] intNames;
	private string[] boolNames;
	private string[] floatNames;
	protected string[] stateNames;
	private bool executed;
	protected RuntimeAnimatorController animator;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		animator=GetAIController(property).runtimeAnimatorController;
		if (!executed && animator != null) {
			
			FillStateNames(animator);
			FillParameterArray(animator as AnimatorController,AnimatorControllerParameterType.Float);
			FillParameterArray(animator as AnimatorController,AnimatorControllerParameterType.Bool);
			FillParameterArray(animator as AnimatorController,AnimatorControllerParameterType.Int);
			FillParameterArray(animator as AnimatorController,AnimatorControllerParameterType.Trigger);
			
			executed=true;
		}
	}

	public void FillStateNames(RuntimeAnimatorController animator){
		List<string> names = new List<string> ();
		int layerCount =(animator as AnimatorController).layerCount;
		for (int layer = 0; layer < layerCount; layer++) {
			StateMachine stateMachine = (animator as AnimatorController).GetLayer(layer).stateMachine;
			int stateCount=stateMachine.stateCount;
			for (int state=0;state<stateCount;state++) {
				names.Add(stateMachine.GetState(state).uniqueName);
			}
		}
		stateNames = names.ToArray ();
	}
	
	
	public void FillParameterArray(AnimatorController animatorController,AnimatorControllerParameterType type){
		List<string> parameterNames = new List<string> ();
		if (animatorController.parameterCount > 0) {
			for (int i=0; i< animatorController.parameterCount; i++) {
				if (animatorController.GetParameter (i).type == type) {
					parameterNames.Add (animatorController.GetParameter (i).name);
				}
			}
			switch(type){
			case AnimatorControllerParameterType.Bool:
				boolNames = parameterNames.ToArray ();
				break;
			case AnimatorControllerParameterType.Float:
				floatNames = parameterNames.ToArray ();
				break;
			case AnimatorControllerParameterType.Int:
				intNames = parameterNames.ToArray ();
				break;
			case AnimatorControllerParameterType.Trigger:
				triggerNames = parameterNames.ToArray ();
				break;
				
			}
		}
	}
	
	public string[] GetParameterNames(AnimatorControllerParameterType type){
		switch(type){
		case AnimatorControllerParameterType.Bool:
			return boolNames;
		case AnimatorControllerParameterType.Float:
			return floatNames;
		case AnimatorControllerParameterType.Int:
			return intNames;
		case AnimatorControllerParameterType.Trigger:
			return triggerNames;
		default:
			Debug.Log("No Parameter");
			return new string[0];
		}
	}
	
	public AIController GetAIController(SerializedProperty property)
	{
		AIController controller = property.serializedObject.targetObject as AIController;
		
		if (controller == null)
		{
			throw new InvalidCastException("Couldn't cast targetObject");
		}
		
		return controller;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 21;
	}

}
