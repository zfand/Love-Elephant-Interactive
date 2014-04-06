using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

	//GameObjects with rigidbodys and single Light children
	public GameObject[] lights;
	private float minTime = 0.1f;
	private float maxTime = 0.6f;
	private int flickers = 3;

	// Use this for initialization
	void Start () {
		lights = GameObject.FindGameObjectsWithTag("RoomLight");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "RoomKey") {
			StartCoroutine ("flicker");
		}
	}

	IEnumerator flicker() {
		foreach (GameObject l in lights) {

			Light light = l.GetComponentInChildren<Light>();

			while (flickers > 0) {
				float wait = Random.Range (minTime, maxTime);
				light.light.intensity = 0;
				yield return new WaitForSeconds(wait);
				light.light.intensity = 8;
				yield return new WaitForSeconds(wait);
				flickers--;
			}

			light.light.intensity = 0;
			l.rigidbody.useGravity = true;

			flickers = 3;
		}
	}
}
