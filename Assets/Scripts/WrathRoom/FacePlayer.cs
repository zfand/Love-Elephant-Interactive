using UnityEngine;
using System.Collections;

public class FacePlayer : MonoBehaviour {

	public GameObject chargeShot;
	public GameObject beam;
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
			targetRotation = Quaternion.LookRotation (player.transform.position - this.transform.position);
			float str = Mathf.Min (speed * Time.deltaTime, 1);
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
		}
		
	}

	IEnumerator LookAndFire() {
		while (true) {
			firing = true;
			GetComponent<Animation>().Play ("Wrath_Shoot", PlayMode.StopAll);
			chargeShot.renderer.enabled = true;
			beam.particleSystem.Play ();
			yield return new WaitForSeconds(shotSpeed/2f);
			firing = false;
			chargeShot.renderer.enabled = false;
			yield return new WaitForSeconds(shotSpeed);
			beam.particleSystem.Stop ();
			GetComponent<Animation>().Play();

		}
	}
}
