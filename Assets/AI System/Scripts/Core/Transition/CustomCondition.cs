using UnityEngine;
using System.Collections;

[System.Serializable]
public class CustomCondition : ScriptableObject {
	public virtual bool Validate(AIRuntimeController controller){
		return false;
	}
}
