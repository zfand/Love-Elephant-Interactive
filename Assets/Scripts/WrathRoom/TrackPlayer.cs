using UnityEngine;
using System.Collections;

public class TrackPlayer : MonoBehaviour {

	public GameObject chargeShot;
	public GameObject beam;
	GameObject player;
	public float speed = 0.5f;
	private Quaternion targetRotation;
	private Vector3 targetPosition;

	public float shotSpeed = 10f;
	private bool firing;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		targetRotation = Quaternion.identity;
		targetPosition = this.transform.position;
		firing = false;
		StartCoroutine (LookAndFire());

	}
	
	// Update is called once per frame
	void Update () {
		if (!firing) {
			targetRotation = Quaternion.LookRotation (player.transform.position - this.transform.position);
			targetPosition = player.transform.position;
			float str = Mathf.Min (speed * Time.deltaTime, 1);
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
			targetPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
			transform.position = Vector3.Lerp (transform.position, targetPosition, str/2);
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
