using UnityEngine;
using System.Collections;

public class LinkEntities : MonoBehaviour {

	public GameObject[] entities;
	public GameObject[] fires;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		for (int i = 0; i < entities.Length; i++) {
			if (entities[i] == null /*&& fires[i].GetComponent<ParticleSystem>().isPlaying*/) {
				Debug.Log ("Fire on");
				fires[i].GetComponent<ParticleSystem>().Play ();
				fires[i].GetComponentInChildren<ParticleSystem>().Play ();
			}
		}

	}
}
