using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseIKAction {
	public IKActionType type;
	public AvatarIKGoal ikGoal;
	public float weight;
	public float bodyWeight;
	public float headWeight;
	public float eyesWeight;
	public float clampWeight;

	public Vector3 vector3Value;
	public string stringValue;
	private Transform ikTarget;
	public SetFloatType setFloatType;
	
	public void  OnAnimatorIK(AIRuntimeController controller,Animator animator){
		switch (type) {
		case IKActionType.SetIKPosition:
			if(ikTarget != null){
				animator.SetIKPosition(ikGoal,ikTarget.position+ vector3Value);
			}else{
				ikTarget=FindIKTarget(controller);
				if(ikTarget== null){
					ikTarget=FindChild(controller.transform,stringValue);
				}
			}
			break;
		case IKActionType.SetIKPositionWeight:

			switch(setFloatType){
			case SetFloatType.Constant:
				animator.SetIKPositionWeight(ikGoal,weight);
				break;
			case SetFloatType.ForwardVelocity:
				animator.SetIKPositionWeight(ikGoal,Vector3.Project(controller.navMeshAgent.desiredVelocity, controller.transform.forward).magnitude);
				break;
			case SetFloatType.GetFloat:
				animator.SetIKPositionWeight(ikGoal,animator.GetFloat(stringValue));
				break;
			case SetFloatType.Random:
				animator.SetIKPositionWeight(ikGoal,Random.Range(weight,bodyWeight));
				break;
			case SetFloatType.TargetAngle:
				float angle=0;
				if(controller.target != null){
					angle = FindAngle(controller.transform.forward, new Vector3(controller.target.position.x,0,controller.target.position.z) - controller.transform.position, controller.transform.up);
				}else{
					angle = FindAngle(controller.transform.forward, controller.navMeshAgent.desiredVelocity, controller.transform.up);
				}
				animator.SetIKPositionWeight(ikGoal,angle / 0.6f);
				break;
			}
			break;
		case IKActionType.SetIKRotation:
			if(ikTarget != null){
				animator.SetIKRotation(ikGoal,ikTarget.rotation);
			}
			break;
		case IKActionType.SetIKRotationWeight:
			animator.SetIKRotationWeight(ikGoal,weight);
			break;
		case IKActionType.SetLookAtPosition:
			if(ikTarget != null){
				animator.SetLookAtPosition(ikTarget.position);
			}else{
				ikTarget=FindIKTarget(controller);
				if(ikTarget== null){
					ikTarget=FindChild(controller.transform,stringValue);
				}
			}
			break;
		case IKActionType.SetLookAtWeight:
			animator.SetLookAtWeight(weight,bodyWeight,headWeight,eyesWeight,clampWeight);
			break;
		case IKActionType.SetIKPositionTarget:
			if(controller.target != null){
				animator.SetIKPosition(ikGoal,controller.target.position+vector3Value);
			}
			break;
		}

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

	private Transform FindIKTarget(AIRuntimeController controller){
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
		if (closest != null) {
			return closest.transform;
		} else {
			return null;
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
}

public enum IKActionType{
	SetIKPosition,
	SetIKPositionWeight,
	SetIKRotation,
	SetIKRotationWeight,
	SetLookAtPosition,
	SetLookAtWeight,
	SetIKPositionTarget
}