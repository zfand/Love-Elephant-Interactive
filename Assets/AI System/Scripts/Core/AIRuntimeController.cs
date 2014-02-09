using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (NavMeshAgent))]
public class AIRuntimeController : MonoBehaviour {
	[SerializeField]
	private AIController controller;
	public int level;

	[HideInInspector]
	public NavMeshAgent navMeshAgent;
	[HideInInspector]
	public Transform target;
	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public Vector3 initialPosition;

	private int stateIndex;
	private AnyState anyState;
	private List<BaseAttribute> attributes;

	private void Awake () {
		enabled = (controller != null && controller.states.Count > 0);
		if (enabled) {
			controller=(AIController)Instantiate(controller);
			animator = GetComponent<Animator> ();
			navMeshAgent = GetComponent<NavMeshAgent> ();
			controller.Initialize ();
			stateIndex = controller.states.FindIndex (state => state.isDefaultState == true);
			controller.states [stateIndex].Reset (this);
			anyState = (AnyState)controller.states.Find (state => state.StateType == typeof(AnyState));
			attributes = new List<BaseAttribute> ();
			initialPosition=transform.position;
			foreach (BaseAttribute attribute in controller.attributes) {
				attributes.Add (new BaseAttribute (attribute, level));
			}
			StartCoroutine("ExecuteActions");
		} else {
			Debug.Log("AIController disabled, there are not enough states.");
		}
	}
	
	private void Update () {
		anyState.ExecuteInUpdate (this, animator);
		CurrentState.ExecuteInUpdate (this,animator);
		CurrentState.HandleState (this);

		string id = CurrentState.ValidateTransition (this);
		string overrideId = anyState.ValidateTransition (this);
		id = (string.IsNullOrEmpty (overrideId) ? id : overrideId);
		if (!string.IsNullOrEmpty (id) && id != CurrentState.id) {
			CurrentState.Reset(this);
			stateIndex = controller.states.FindIndex (x => x.id == id);
			CurrentState.Reset (this);
		}
	}
	
	private IEnumerator ExecuteActions(){
		while (true) {
			yield return StartCoroutine (CurrentState.Execute (this, animator));
			foreach(BaseTransition transition in CurrentState.transitions){
				foreach(BaseCondition condition in transition.conditions){
					condition.executed=true;
				}
			}
		}
	}

	private void OnAnimatorIK(int layerIndex) {
		CurrentState.OnAnimatorIK (layerIndex,this,animator);
	}

	private void OnAnimatorMove() {
		CurrentState.OnAnimatorMove (this, animator, navMeshAgent);
	}

	public Vector3 GetPointInRange(float range, bool groundDetection){
		Vector3 random = new Vector3 (initialPosition.x + Random.Range (-range, range), initialPosition.y, initialPosition.z + Random.Range (-range, range)); 
		if (groundDetection) {
			RaycastHit hit;
			if (Physics.Raycast (random + Vector3.up * 500, Vector3.down, out hit)) {
				random.y = hit.point.y;
			}
		}
		return random;
	}

	public BaseAttribute GetAttribute(string name){
		return attributes.Find (attribute => attribute.name == name);
	}

	public State GetStateByName(string name){
		return controller.states.Find (state => state.title == name);
	}

	public State GetStateById(string id){
		return controller.states.Find (state => state.id == id);
	}

	public State CurrentState{
		get{
			return controller.states[stateIndex];
		}
	}

	public Formula GetFormula(string name){
		return controller.formula.Find (x => x.name == name);
	}
}



public static class AIRuntimeControllerExtension{
	public static AIRuntimeController AIRuntimeController(this GameObject gameObject){
		return gameObject.GetComponent<AIRuntimeController> ();
	}

	public static AIRuntimeController AIRuntimeController(this MonoBehaviour behaviour){
		return behaviour.GetComponent<AIRuntimeController> ();
	}

	public static AIRuntimeController AIRuntimeController(this Collider collider){
		return collider.GetComponent<AIRuntimeController> ();
	}

	public static AIRuntimeController AIRuntimeController(this Transform transform){
		return transform.GetComponent<AIRuntimeController> ();
	}


}
