using UnityEngine;
using System.Collections;

namespace LoveElephant
{
	public class ShockTrigger : MonoBehaviour 
	{

		//private SlothAI slothAI;
		private bool countdown = false;
		private float currenttime;

		public float CountdownTime;
		public float PreshockLength;
		public float Shocklength;
		private float x;
		// Use this for initialization
		void Start () {
		//	slothAI = this.transform.parent.gameObject.GetComponent<SlothAI>();
			currenttime = CountdownTime;
		}
		
		// Update is called once per frame
		void Update () {
			if(countdown){
				currenttime--;
				if(currenttime <= 0){
					StartCoroutine("ShockPreheat");
				}
			}
		}


		void OnTriggerEnter(Collider c) {
			if(c.transform.CompareTag("Player")) {
				countdown = true;
			}
		}

		void OnTriggerExit(Collider c) {
			if(c.transform.CompareTag("Player") && countdown) {
				countdown = true;
			}
			ResetCountdown();
		}


		private void ResetCountdown() {
			countdown = false;
			currenttime = CountdownTime;
		}


		public IEnumerator ShockPreheat(){
			int i = 10;
			while(i >= 1){
				i--;
				yield return 0;
			}
		}
	}
}
