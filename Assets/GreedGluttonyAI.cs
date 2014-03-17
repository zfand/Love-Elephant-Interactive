using UnityEngine;
using System.Collections;

public class GreedGluttonyAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnCollisionEnter(Collision c){
		Debug.Log ("Testing");
		Debug.Log(c.collider.name);
	}
	void OnTriggerEnter(Collider c){
		Debug.Log ("Testing");
		Debug.Log(c.collider.name);
	}
}
