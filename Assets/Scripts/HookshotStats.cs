using UnityEngine;
using System.Collections;

public class HookshotStats : MonoBehaviour {

	public float FireSpeed;
	public float ReelSpeed;
	public float Damage;
	public float MaxLength;
	// Use this for initialization
	void Start () {
		if(FireSpeed == null){
			Debug.LogError("FireSpeed not assigned to " + this.name);
		}
		if(ReelSpeed == null){
			Debug.LogError("ReelSpeed not assigned to " + this.name);
		}
		if(Damage == null){
			Debug.LogError("Damage not assigned to " + this.name);
		}
		if(MaxLength == null){
			Debug.LogError("MaxLength not assigned to " + this.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
