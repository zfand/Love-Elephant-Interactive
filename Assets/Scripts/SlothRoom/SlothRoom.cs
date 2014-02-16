using UnityEngine;
using System.Collections;

public class SlothRoom : MonoBehaviour {


	public GameObject slothBoss;

	// Use this for initialization
	void Start () {

		if (GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>().currentLevelState != LevelState.Complete) {

			Quaternion slothRot = Quaternion.Euler(0f,270f,0f);

			Instantiate(slothBoss, new Vector3(11.27f, -6.17f, 0f), slothRot);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
