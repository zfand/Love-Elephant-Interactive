using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	GameObject player;

	public float speed;
	public int shootFreq;
	public float projSpeed;
	public GameObject bullet;

	public GameObject barrel;
	private Quaternion targetRotation;

	public LayerMask mask;

	private bool shooting;

	public int health;

	// Use this for initialization
	void Start () {
		shooting = false;
		player = GameObject.FindGameObjectWithTag("Player");
		InvokeRepeating("Shoot", 0f, shootFreq);
	}
	
	// Update is called once per frame
	void Update () {
		if (health == 0) {
			StartCoroutine(Destruct());
		} else if (player.transform.position.y < this.transform.position.y && health > 0) {

			RaycastHit play = new RaycastHit();

			if (Physics.SphereCast (barrel.transform.position, 3f, barrel.transform.forward, out play, 50f, mask)) {
				if (play.collider.gameObject.CompareTag("Player") && !shooting) {
					shooting = true;
				} else {
					shooting = false;
					Quaternion oldRot = this.transform.rotation;
					Quaternion newRot = Quaternion.LookRotation(player.transform.position - this.transform.position);

					this.transform.rotation = Quaternion.Lerp (oldRot, newRot, speed);
				}
			}
		}
	}

	IEnumerator Destruct() {
		health = -1;
		barrel.renderer.enabled = false;
		this.GetComponent<ParticleSystem>().Play ();
		this.GetComponentInChildren<ParticleSystem>().Play ();
		yield return new WaitForSeconds(.5f);
		Destroy (this.gameObject);
	}

	void Shoot() {
		if (shooting) {
			GameObject tempBul = (GameObject)Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
			tempBul.rigidbody.AddForce (barrel.transform.forward * projSpeed);
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.CompareTag("Weapon")) {
			health -= 10;
		}
	}
}
