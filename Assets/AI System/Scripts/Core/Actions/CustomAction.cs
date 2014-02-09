using UnityEngine;
using System.Collections;

[System.Serializable]
public class CustomAction : ScriptableObject {

	public virtual IEnumerator Execute(AIRuntimeController controller,Animator animator, State state){
		yield return null;
	}
}
