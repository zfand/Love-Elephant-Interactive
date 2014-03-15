using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {

	public GameObject resetPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "Player") {
			c.gameObject.transform.position = resetPos.transform.position;
			c.gameObject.rigidbody.velocity = Vector3.zero;
		}
	}
}
