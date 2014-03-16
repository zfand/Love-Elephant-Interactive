using UnityEngine;
using System.Collections;

namespace LoveElephant {
	public class TempPoisoner : MonoBehaviour {
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
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
