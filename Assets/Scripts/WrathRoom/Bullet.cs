using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate (Vector3.forward * Time.deltaTime * 5);
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "floor" ||
		    col.gameObject.tag == "wall" ||
		    col.gameObject.tag == "ceiling")
			Destroy (this.gameObject);
	}
}
