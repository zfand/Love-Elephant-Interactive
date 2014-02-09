using UnityEngine;
using System.Collections;

[System.Serializable]
public class FleeState : FollowState {
	
	public FleeState():base(){
	}

	public override void HandleState (AIRuntimeController controller)	{
		if (controller.navMeshAgent != null) {
			Vector3 directionToTarget = controller.target.position - controller.transform.position;
			float angel = Vector3.Angle(controller.target.forward, directionToTarget);
			Vector3 fleePosition= controller.transform.position + controller.target.forward*5; 
			
			if (Mathf.Abs(angel) < 90 || Mathf.Abs(angel) > 270){
				fleePosition= controller.transform.position - controller.target.forward*5; 
			}
			SetProperties(controller.navMeshAgent);
			controller.navMeshAgent.SetDestination(fleePosition);
		}
	}
}
