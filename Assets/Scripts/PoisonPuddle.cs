using UnityEngine;
using System.Collections;

namespace LoveElephant {
	public class PoisonPuddle : MonoBehaviour {

		public float FadeTime;
		public float scaleinterval;
		public float MaxScale;
		private float currscale = 0;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			if(currscale < MaxScale){
				this.transform.localScale = new Vector3(this.transform.localScale.x + scaleinterval,
				                                        this.transform.localScale.y + scaleinterval,
				                                        this.transform.localScale.z);
				currscale += scaleinterval;
			}
		}

		void OnCollisionEnter(Collision other){

			if(other.gameObject.CompareTag("Player")) {
				Transform old_poisoner = other.transform.FindChild("Poisoner");
				if(old_poisoner == null) {
					GameObject poison = Instantiate (Resources.Load<GameObject> ("Items/StatusEffects/Poisoner")) as GameObject;
					poison.transform.position = other.transform.position;
					poison.transform.parent = other.transform;
					poison.name = "Poisoner";
					poison.GetComponent<Poisoner>().BeginPoison();
				} else {
					old_poisoner.GetComponent<Poisoner>().Reset();
				}
			}
		}

		public void FadeOut(){
			StartCoroutine(DestroyPuddle());
		}

		private IEnumerator DestroyPuddle(){
			float fadeouttime = 0;

			if(currscale < MaxScale){
				yield return new WaitForSeconds(1);
			}
			while(fadeouttime < FadeTime){

				this.transform.position = new Vector3(this.transform.position.x, 
				                                      this.transform.position.y - 0.01f,
				                                      this.transform.position.z);
				fadeouttime++;
				yield return 0;
			}

			Destroy(this.gameObject);
		}


		void OnCollisionStay(Collision other){
			if(other.gameObject.CompareTag("Player")) {
				Transform poisoner = other.transform.FindChild("Poisoner");
				if(poisoner != null) {
					poisoner.GetComponent<Poisoner>().Reset();
				}
			}
		}

	}
}
