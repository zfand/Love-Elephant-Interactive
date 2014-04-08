using UnityEngine;
using System.Collections;

public class BugBomb : MonoBehaviour {

	GameObject player;
	public float moveSpeed;

	private bool moving;

	// Use this for initialization
	void Start () {
		moving = true;
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (moving) {
			if (player.transform.position.x > this.transform.position.x) {
				this.rigidbody.AddForce (new Vector3(moveSpeed, 0f, 0f));
			} else if (player.transform.position.x < this.transform.position.x) {
				this.rigidbody.AddForce (new Vector3(-moveSpeed, 0f, 0f));
			}
		} else {
			StartCoroutine(Detonate());
		}
	}

	IEnumerator Detonate() {
		yield return new WaitForSeconds(3f);
		Destroy (this);
	}
}
