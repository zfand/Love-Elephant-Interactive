using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WalkState : FollowState {

	public WalkState():base(){
		properties.Add(new SerializedStringValuePair("Range",0.0f));
		properties.Add (new SerializedStringValuePair ("Threshold", 0.1f));
	}

	public float Range{
		get{
			return (float)GetProperty("Range").floatValue;
		}
	}

	public float Threshold{
		get{
			return (float)GetProperty("Threshold").floatValue;
		}
	}

	private Vector3 walkPosition;
	public override void HandleState (AIRuntimeController controller)
	{
		if (walkPosition == Vector3.zero || Vector3.Distance(controller.transform.position,walkPosition)<Threshold) {
			walkPosition=controller.GetPointInRange(Range,true);
			NavMeshHit hit;
			NavMesh.SamplePosition(walkPosition, out hit, Range, 1);
			walkPosition = hit.position;
		}

		if (controller.navMeshAgent != null) {
			SetProperties(controller.navMeshAgent);
			controller.navMeshAgent.SetDestination(walkPosition);
		}
	}

	public override void Reset (AIRuntimeController controller){
		base.Reset (controller);
		walkPosition = Vector3.zero;
	}
}
