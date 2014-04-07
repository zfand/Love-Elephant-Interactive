using UnityEngine;
using System.Collections;

public class WrathAttack : MonoBehaviour {

	bool shot = false;

	// Use this for initialization
	void Start () {
		this.GetComponent<Animation>().PlayQueued("Wrath_Shoot", QueueMode.PlayNow);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
