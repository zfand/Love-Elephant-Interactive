using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject bugBomb;

	// Use this for initialization
	void Start () {
		StartCoroutine(Spawn());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Spawn() {
		yield return new WaitForSeconds(10f);
		Debug.Log ("Play opening animation");
		yield return new WaitForSeconds(3f);
		Debug.Log ("Spawn new bugbomb");
		Instantiate (bugBomb, new Vector3(this.transform.position.x + 3, this.transform.position.y, 0), Quaternion.Euler (0,0,90));
		yield return new WaitForSeconds(3f);
		Debug.Log ("Play closing animation");

	}
}
