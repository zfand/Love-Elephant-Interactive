using UnityEngine;
using System.Collections;

public class FacePlayer : MonoBehaviour {

	GameObject player;
	public float speed = 0.5f;
	private Quaternion targetRotation;

	public float shotSpeed = 5f;
	private bool firing;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		targetRotation = Quaternion.identity;
		firing = false;
		StartCoroutine (LookAndFire());

	}
	
	// Update is called once per frame
	void Update () {
		if (!firing) {
			//if (!GetComponent<Animation>().isPlaying("Wrath_Idle"))
			GetComponent<Animation>().Play ("Wrath_Idle", PlayMode.StopAll);
			targetRotation = Quaternion.LookRotation (player.transform.position - this.transform.position);
			float str = Mathf.Min (speed * Time.deltaTime, 1);
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
		}
		
	}

	IEnumerator LookAndFire() {
		while (true) {
			firing = true;
			yield return new WaitForSeconds(shotSpeed);
			GetComponent<Animation>().Play ("Wrath_Shoot", PlayMode.StopAll);
			firing = false;

		}
	}
}
