using UnityEngine;
using System.Collections;

public class SwordStats : MonoBehaviour {

	public float Damage;
	public float SwingAngle;
	// Use this for initialization
	void Start () {
		if(Damage == null){
			Debug.LogError("Damage not assigned to " + this.name);
		}
		if(SwingAngle == null){
			Debug.LogError("SwingAngle not assigned to " + this.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
