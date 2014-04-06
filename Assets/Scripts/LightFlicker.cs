using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

	public GameObject light;
	private float minTime = 0.1f;
	private float maxTime = 0.6f;
	private int flickers = 3;

	// Use this for initialization
	void Start () {
		Debug.Log ("Start");
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
		while (flickers > 0) {
			float wait = Random.Range (minTime, maxTime);
			light.light.intensity = 0;
			yield return new WaitForSeconds(wait);
			light.light.intensity = 8;
			yield return new WaitForSeconds(wait);
			flickers--;
		}
		light.light.intensity = 0;
	}
}
