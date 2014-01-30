using UnityEngine;
using System.Collections;

public class ShittyCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("a")) {
			transform.position = new Vector3(transform.position.x-1, transform.position.y, 0f);
		} else if (Input.GetKey("d")) {
			transform.position = new Vector3(transform.position.x+1, transform.position.y, 0f);
		} else if (Input.GetKeyDown("space")) {
			rigidbody.AddForce(Vector3.up*1000);
		}
	}
}
