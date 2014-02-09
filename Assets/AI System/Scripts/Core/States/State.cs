using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class State:Node{
	public string description;
	public BaseStateAction[] stateActions;
	public BaseTransition[] transitions;
	public BaseStateAction[] update;
	public BaseIKAction[] ikActions;

	public bool isDefaultState;
	public List<SerializedStringValuePair> properties;
	private bool runningActions;
	public bool applyRootMotion;

	public State():base(){
		properties = new List<SerializedStringValuePair> ();
	}
	

	public virtual void HandleState(AIRuntimeController controller){

	}

	public virtual void OnAnimatorMove(AIRuntimeController controller,Animator animator,NavMeshAgent agent){
		agent.updateRotation=!applyRootMotion;
		if (applyRootMotion) {
			controller.transform.rotation=animator.rootRotation;
			agent.velocity = animator.deltaPosition / Time.deltaTime;
		}
	}

	public virtual void OnAnimatorIK(int layerIndex,AIRuntimeController controller,Animator animator){
		foreach (BaseIKAction ik in ikActions) {
			ik.OnAnimatorIK(controller,animator);
		}
	}

	public virtual void Reset(AIRuntimeController controller){
		foreach (BaseTransition transition in transitions) {
			transition.Reset();
		}
	}

	public virtual void ExecuteInUpdate(AIRuntimeController controller,Animator animator){
		foreach (BaseStateAction action in update) {
			action.ExecuteInUpdate(controller,animator,this);
		}
	}

	public virtual IEnumerator Execute(AIRuntimeController controller, Animator animator){
		foreach (BaseStateAction action in stateActions) {
			yield return controller.StartCoroutine (action.Execute (controller,animator,this));
		}
		yield return null;
	}

	public string ValidateTransition(AIRuntimeController controller){
		foreach (BaseTransition transition in transitions) {
			if(transition.Validate(controller)){
				return transition.toState;
			}
		}
		return string.Empty;
	} 

	public BaseTransition GetTransitionAtIndex(int index){
		if (transitions != null && transitions.Length > 0) {
			for(int i=0; i< transitions.Length;i++){
				if(i==index){
					return transitions[i];
				}
			}
		}
		return null;
	}

	public SerializedStringValuePair GetProperty(string key){
		return properties.Find(property=>property.key==key);
	}

}
