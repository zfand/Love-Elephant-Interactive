using UnityEngine;
using System.Collections;

public class BootStats : MonoBehaviour {

	public float SpeedMod;
	public float JumpMod;
	// Use this for initialization
	void Start () {
		if(SpeedMod == null){
			Debug.LogError("Damage not assigned to " + this.name);
		}
		if(JumpMod == null){
			Debug.LogError("SwingAngle not assigned to " + this.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
