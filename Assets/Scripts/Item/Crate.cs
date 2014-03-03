using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider hit) {
		if (hit.gameObject.tag == "Weapon") {
			((MeshExploder) this.GetComponent("MeshExploder")).Explode(); 
			this.gameObject.SetActive(false);
		}
	}
}
