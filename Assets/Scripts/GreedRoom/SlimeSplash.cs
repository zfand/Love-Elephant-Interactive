using UnityEngine;
using System.Collections;

public class SlimeSplash : MonoBehaviour {

	GameObject splash;
	// Use this for initialization
	void Start () {
		splash = (GameObject)Resources.Load ("Particles/Splash");
		splash.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnParticleCollision(GameObject g) {
		GameObject newsplash = Instantiate(splash) as GameObject;
		newsplash.transform.position = new Vector3(g.transform.position.x, this.transform.position.y + this.renderer.bounds.size.y/2, g.transform.position.z);
		newsplash.SetActive(true);
	}

	void AddPoisonPuddle(){

	}
}
