using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	GameObject player;

	public int speed;
	public int shootFreq;
	public float projSpeed;
	public GameObject bullet;

	private Quaternion targetRotation;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		StartCoroutine(Shoot());
	}
	
	// Update is called once per frame
	void Update () {

		targetRotation = Quaternion.LookRotation (player.transform.position - this.transform.position);
		float str = Mathf.Min (speed * Time.deltaTime, 1);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
	}

	IEnumerator Shoot() {
		yield return new WaitForSeconds(shootFreq);
		Instantiate(bullet, this.transform.position, this.transform.rotation);

	}

}
