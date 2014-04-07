using UnityEngine;
using System.Collections;

public class FacePlayer : MonoBehaviour {

	GameObject player;
	public float speed = 0.5f;
	private Quaternion targetRotation;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		targetRotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {

		targetRotation = Quaternion.LookRotation (player.transform.position - this.transform.position);
		
		float str = Mathf.Min (speed * Time.deltaTime, 1);
		
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
		
	}
}
