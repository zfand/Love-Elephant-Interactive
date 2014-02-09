using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class BaseStateAction {
	public StateActionType type;
	public TargetType targetType;
	public string stringValue;
	public GameObject gameObjectValue;
	public Vector3 vector3Value;
	public float floatValue;
	public float floatValue2;
	public int intValue;
	public bool boolValue;
	public AudioClip audioClipValue;
	public SetFloatType setFloatType;
	public CustomAction customActionValue;

	private Transform childTransform;

	public virtual IEnumerator Execute(AIRuntimeController controller,Animator animator, State state){

		ExecuteInUpdate (controller, animator, state);

		switch (type) {
			case StateActionType.WaitForSeconds:
			yield return new WaitForSeconds(floatValue);
			break;
		case StateActionType.Custom:
			yield return controller.StartCoroutine(customActionValue.Execute(controller,animator,state));
			break;
		}
	}

	public virtual void ExecuteInUpdate(AIRuntimeController controller,Animator animator, State state){
		switch (type) {
		case StateActionType.Instantiate:
			Instantiate(controller);
			break;
		case StateActionType.LookAt:
			switch(targetType){
			case TargetType.None:
				controller.transform.LookAt(new Vector3(vector3Value.x,controller.transform.position.y,vector3Value.z));
				break;
			case TargetType.Target:
				if(controller.target != null){
					controller.transform.LookAt(new Vector3(controller.target.position.x,controller.transform.position.y,controller.target.position.z));
				}else{
					NoTargetSet(state);
				}
				break;
			case TargetType.Child:
				if(childTransform == null){
					childTransform=FindChild(controller.transform,stringValue);
				}
				if(childTransform != null){
					controller.transform.LookAt(new Vector3(childTransform.position.x,controller.transform.position.y,childTransform.position.z));
				}
				break;
			}
			break;
		case StateActionType.SendMessage:
			switch(targetType){
			case TargetType.Self:
				controller.SendMessage(stringValue,state,SendMessageOptions.DontRequireReceiver);
				break;
			case TargetType.Target:
				if(controller.target != null){
					controller.target.BroadcastMessage(stringValue,state,SendMessageOptions.DontRequireReceiver);
				}else{
					NoTargetSet(state);
				}
				break;
			case TargetType.Child:
				controller.BroadcastMessage(stringValue,state,SendMessageOptions.DontRequireReceiver);
				break;
			}
			break;
		case StateActionType.SetTarget:
			GameObject[] tagged=GameObject.FindGameObjectsWithTag(stringValue);
			GameObject closest=null; 
			float distance = Mathf.Infinity; 
			Vector3 position = controller.transform.position; 
			foreach (GameObject go in tagged)  { 
				Vector3 diff = (go.transform.position - position);
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					closest = go; 
					distance = curDistance; 
				} 
			} 
			
			if(closest != null){
				controller.target=closest.transform;
			}
			break;
		case StateActionType.Destroy:
			switch(targetType){
			case TargetType.Self:
				GameObject.Destroy(controller.gameObject,floatValue);
				break;
			case TargetType.Target:
				if(controller.target != null){
					GameObject.Destroy(controller.target.gameObject,floatValue);
				}else{
					NoTargetSet(state);
				}
				break;
			case TargetType.Child:
				if(childTransform == null){
					childTransform=FindChild(controller.transform,stringValue);
				}
				if(childTransform != null){
					GameObject.Destroy(childTransform,floatValue);
				}
				break;
			}
			break;
		case StateActionType.AddAttribute:
			BaseAttribute addAttribute=controller.GetAttribute(stringValue);
			if(addAttribute != null){
				addAttribute.Add(intValue);
			}else{
				Debug.Log("Such attribute does not exist, you should add one in the general section: "+stringValue);
			}
			break;
		case StateActionType.ConsumeAttribute:
			BaseAttribute consumeAttribute=controller.GetAttribute(stringValue);
			if(consumeAttribute != null){
				consumeAttribute.Add(intValue);
			}else{
				Debug.Log("Such attribute does not exist, you should add one in the general section: "+stringValue);
			}
			break;
		case StateActionType.SetFloat:
			switch(setFloatType){
			case SetFloatType.Constant:
				animator.SetFloat(stringValue,floatValue,0.15f,Time.deltaTime);
				break;
			case SetFloatType.ForwardVelocity:
				animator.SetFloat(stringValue,Vector3.Project(controller.navMeshAgent.desiredVelocity, controller.transform.forward).magnitude,floatValue,Time.deltaTime);
				break;
			case SetFloatType.Random:
				animator.SetFloat(stringValue,(int)Random.Range(floatValue,floatValue2));
				break;
			case SetFloatType.TargetAngle:
				float angle=0;
				if(controller.target != null){
					angle = FindAngle(controller.transform.forward, new Vector3(controller.target.position.x,0,controller.target.position.z) - new Vector3(controller.transform.position.x,0,controller.transform.position.z), controller.transform.up);
				}else{
					angle = FindAngle(controller.transform.forward, controller.navMeshAgent.desiredVelocity, controller.transform.up);
				}
				float angularSpeed = angle / 0.6f;
				animator.SetFloat(stringValue, angularSpeed, floatValue, Time.deltaTime);
				break;
			case SetFloatType.AngleVelocity:
				angle = FindAngle(controller.transform.forward, controller.navMeshAgent.desiredVelocity, controller.transform.up);
				angularSpeed = angle / 0.6f;
				animator.SetFloat(stringValue, angularSpeed, floatValue, Time.deltaTime);
				break;
			}
			break;
		case StateActionType.SetBool:
			animator.SetBool(stringValue,boolValue);
			break;
		case StateActionType.SetInt:
			animator.SetInteger(stringValue,intValue);
			break;
		case StateActionType.SetTrigger:
			//Debug.Log("Set: "+stringValue);
			animator.SetTrigger(stringValue);
			break;
		case StateActionType.PlaySound:
			AudioSource audio=controller.GetComponent<AudioSource>();
			if(audio == null){
				audio=controller.gameObject.AddComponent<AudioSource>();
			}
			audio.volume=floatValue;
			audio.clip=audioClipValue;
			audio.Play();
			break;
		case StateActionType.CrossFade:
			animator.CrossFade(stringValue,floatValue);
			break;
		case StateActionType.SetLayerWeight:
			animator.SetLayerWeight(intValue,floatValue);
			break;
		}
	}

	private void Instantiate(AIRuntimeController controller){
		Vector3 offset=vector3Value;
		switch(targetType){
		case TargetType.Self:
			offset=controller.transform.position+controller.transform.right*offset.x+Vector3.up*offset.y+controller.transform.forward*offset.z;
			break;
		case TargetType.Target:
			if(controller.target != null){
				offset=controller.target.position+controller.target.right*offset.x+Vector3.up*offset.y+controller.target.forward*offset.z;;
			}
			break;
		case TargetType.Child:
			if(childTransform == null){
				childTransform=FindChild(controller.transform,stringValue);
			}
			offset=childTransform!= null?childTransform.position:controller.transform.position;
			break;
		}
		if(gameObjectValue != null){
			GameObject go=(GameObject)GameObject.Instantiate(gameObjectValue,offset,controller.transform.rotation);
			if(controller.target != null){
				go.transform.LookAt (controller.target.position+vector3Value);
			}
		}
	}

	private float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		// If the vector the angle is being calculated to is 0...
		if(toVector == Vector3.zero)
			// ... the angle between them is 0.
			return 0f;
		// Create a float to store the angle between the facing of the enemy and the direction it's travelling.
		float angle = Vector3.Angle(fromVector, toVector);
		// Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
		Vector3 normal = Vector3.Cross(fromVector, toVector);
		// The dot product of the normal with the upVector will be positive if they point in the same direction.
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
		// We need to convert the angle we've found from degrees to radians.
		angle *= Mathf.Deg2Rad;
		return angle;
	}

	private Transform FindChild(Transform target, string tag)
	{
		if (target.tag == tag) return target;
		
		for (int i = 0; i < target.childCount; ++i)
		{
			Transform result = FindChild(target.GetChild(i), tag);
			
			if (result != null) return result;
		}
		
		return null;
	}

	private void NoTargetSet(State state){
		Debug.LogError("No target set, you should set a target before you call it in "+state.title);
	}

}

public enum StateActionType{
	SendMessage,
	Instantiate,
	LookAt,
	SetTarget,
	WaitForSeconds,
	Destroy,
	AddAttribute,
	ConsumeAttribute,
	SetFloat,
	SetInt,
	SetBool,
	SetTrigger,
	PlaySound,
	CrossFade,
	SetLayerWeight,
	Custom

}

public enum SetFloatType{
	Constant,
	ForwardVelocity,
	Random,
	TargetAngle,
	GetFloat,
	AngleVelocity
}

public enum TargetType{
	None,
	Target,
	Child,
	Self,
}
