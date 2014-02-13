using UnityEngine;
using System.Collections;

public class RoomOnePickup : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider hit) {
		if (hit.gameObject.tag == "RoomKey") {
			Destroy(hit.gameObject);
			player.GetComponent<PlayerKeys>().white = true;
		} else if (hit.gameObject.tag == "RoomExitRight" && player.GetComponent<PlayerKeys>().white) {
			Debug.Log ("Exit Room");
		}
	}
}
