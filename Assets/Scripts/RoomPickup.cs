using UnityEngine;
using System.Collections;

public class RoomPickup : MonoBehaviour {

	public GameObject player;
	public GameObject roomManager;

	public string rightRoom;
	public string leftRoom;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GetManager() {
		roomManager = GameObject.FindGameObjectWithTag("RoomManager");
	}

	void OnTriggerEnter(Collider hit) {

		if (roomManager == null) {
			GetManager ();
		}

		if (hit.gameObject.tag == "RoomKey") {
			player.GetComponent<PlayerKeys>().addKey(hit.gameObject.name);
			Destroy (hit.gameObject);
		}

		if (hit.gameObject.tag == "RoomExitRight") {
			if (roomManager.GetComponent<RoomManager>().checkRight()) {
				GameObject.FindGameObjectWithTag("Parallax").GetComponent<Parallax>().disable();
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMSaveState", LevelState.Complete);
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMLoadLevel", roomManager.GetComponent<RoomManager>().rightRoom);
				player.transform.position = new Vector3(-10.3f, player.transform.position.y, 0f);
			}
		} else if (hit.gameObject.tag == "RoomExitLeft") {
			if (roomManager.GetComponent<RoomManager>().checkLeft()) {
				GameObject.FindGameObjectWithTag("Parallax").GetComponent<Parallax>().disable();
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMSaveState", LevelState.Complete);
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMLoadLevel", roomManager.GetComponent<RoomManager>().leftRoom);
				player.transform.position = new Vector3(10.3f, player.transform.position.y, 0f);
			}
		} else if (hit.gameObject.tag == "RoomExitMidB") {
			if (roomManager.GetComponent<RoomManager>().checkMidB()) {
				GameObject.FindGameObjectWithTag("Parallax").GetComponent<Parallax>().disable();
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMSaveState", LevelState.Complete);
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMLoadLevel", roomManager.GetComponent<RoomManager>().midRoomB);
				player.transform.position = new Vector3(player.transform.position.x, 0f, 0f);
			}
		} else if (hit.gameObject.tag == "RoomExitMidT") {
			if (roomManager.GetComponent<RoomManager>().checkMidT()) {
				GameObject.FindGameObjectWithTag("Parallax").GetComponent<Parallax>().disable();
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMSaveState", LevelState.Complete);
				GameObject.FindGameObjectWithTag("SceneManager").SendMessage("SMLoadLevel", roomManager.GetComponent<RoomManager>().midRoomT);
				player.transform.position = new Vector3(player.transform.position.x, 0f, 0f);
			}
		}
	}
}
