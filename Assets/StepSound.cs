using UnityEngine;
using System.Collections;

public class StepSound : MonoBehaviour {

	public AudioSource audio;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c){
		if(c.gameObject.CompareTag("Floor")){
		//	audio.Play();
		}
	}
}
