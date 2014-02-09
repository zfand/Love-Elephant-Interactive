using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class BaseTransition {
	public string fromState;
	public string toState;
	public BaseCondition[] conditions;
	[SerializeField]
	private string title;
	public string Title{
		get{
			return title;
		}
	}
	public BaseTransition(string fromState,string toState, AIController controller){
		this.fromState = fromState;
		this.toState = toState;
		title = controller.GetState (fromState).title + " -> " + controller.GetState (toState).title;
	}

	public virtual bool Validate(AIRuntimeController controller){
		foreach (BaseCondition condition in conditions) {
			if(!condition.Validate(controller)){
				return false;
			}
		}
		return true;
	}

	public virtual void Reset(){
		foreach (BaseCondition condition in conditions) {
			condition.Reset();
		}
	}
}
