using UnityEngine;
using System.Collections;

public class IconIdleScript : MonoBehaviour {

	bool pulsing;
	bool pulseIn;
	bool pulseOut;
	float mult = 1f;
	public float maxScale;
	public float minScale;
	public float interval;
	Vector3 originalScale;
	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		pulseIn = true;
		pulseOut = false;
		pulsing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!pulsing){
			if(pulseIn){
				StartCoroutine(PulseIn ());
			} else {
				StartCoroutine(PulseOut ());
			}
		}
	}

	IEnumerator PulseIn(){
		pulsing = true;
		pulseIn = false;
		while(mult <= maxScale){
			mult += interval;
			this.transform.localScale = originalScale * mult;
			yield return 0;
		}
		pulsing = false;
		pulseOut = true;
	}

	IEnumerator PulseOut(){
		pulsing = true;
		pulseOut = false;
		while(mult >= minScale){
			mult -= interval;
			this.transform.localScale = originalScale * mult;
			yield return 0;
		}

		pulsing = false;
		pulseIn = true;
	}


}
