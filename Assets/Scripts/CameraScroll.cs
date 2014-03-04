using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	public GameObject cam;
	public float widthBarrier;
	public bool tallRoom;

	// Use this for initialization
	void Start () {
		cam.transform.position = new Vector3(widthBarrier, cam.transform.position.y, cam.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

		float playerX = GameObject.FindGameObjectWithTag ("Player").transform.position.x;
		float playerY = GameObject.FindGameObjectWithTag ("Player").transform.position.y;

		float newX = playerX;
		float newY = cam.transform.position.y;

		if (tallRoom) {
			newY = playerY + 6.5f;
		}

		if (newX < -widthBarrier) {
			newX = -widthBarrier;
		} else if (newX > widthBarrier) {
			newX = widthBarrier;
		}

		if (newY > widthBarrier + 6.5f) {
			newY = widthBarrier + 6.5f;
		}

		if (playerX > -widthBarrier && playerX < widthBarrier) {
			cam.transform.position = new Vector3(playerX, newY, cam.transform.position.z);
		} else if (playerX < -widthBarrier) {
			cam.transform.position = new Vector3(-widthBarrier, newY, cam.transform.position.z);
		} else if (playerX > widthBarrier) {
			cam.transform.position = new Vector3(widthBarrier, newY, cam.transform.position.z);
		}
	}
}
