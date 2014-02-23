using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	public GameObject cam;
	public float widthBarrier;

	// Use this for initialization
	void Start () {
		cam.transform.position = new Vector3(widthBarrier, cam.transform.position.y, cam.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

		float playerX = GameObject.FindGameObjectWithTag ("Player").transform.position.x;

		if (playerX > -widthBarrier && playerX < widthBarrier) {
			cam.transform.position = new Vector3(playerX, cam.transform.position.y, cam.transform.position.z);
		} else if (playerX < -widthBarrier) {
			cam.transform.position = new Vector3(-widthBarrier, cam.transform.position.y, cam.transform.position.z);
		} else if (playerX > widthBarrier) {
			cam.transform.position = new Vector3(widthBarrier, cam.transform.position.y, cam.transform.position.z);
		}
	}
}
