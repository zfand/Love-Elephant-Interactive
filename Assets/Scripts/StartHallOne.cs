using UnityEngine;
using System.Collections;

public class StartHallOne : MonoBehaviour {

	public GameObject roomKey;

	// Use this for initialization
	void Start () {

		if (GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>().currentLevelState != LevelState.Complete) {
			Instantiate(roomKey, new Vector3(-25.5f, -3.3f, 0f), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
