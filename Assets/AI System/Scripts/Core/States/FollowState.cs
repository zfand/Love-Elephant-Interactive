using UnityEngine;
using System.Collections;

[System.Serializable]
public class FollowState : State {

	public FollowState():base(){
		properties.Add(new SerializedStringValuePair("Speed",0.0f));
		properties.Add(new SerializedStringValuePair("Rotation",0.0f));	
	}
	
	public float Speed{
		get{
			return GetProperty("Speed").floatValue;
		}
	}
	
	public float Rotation{
		get{
			return GetProperty("Rotation").floatValue;
		}
	}

	public void SetProperties(NavMeshAgent agent){
		agent.speed = Speed;
		agent.angularSpeed = Rotation;
	}

	public override void HandleState (AIRuntimeController controller)	{
		if (controller.navMeshAgent != null && controller.target != null) {
			SetProperties(controller.navMeshAgent);
			controller.navMeshAgent.SetDestination(controller.target.position);
		}

	}

	public override void Reset (AIRuntimeController controller){
		base.Reset (controller);
		if (controller.navMeshAgent != null) {
			controller.navMeshAgent.Stop();
		}
	}
}
