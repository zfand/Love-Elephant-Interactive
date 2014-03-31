using UnityEngine;
using System.Collections;
using LoveElephant;

namespace LoveElephant
{
	public class PipeTrigger : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnTriggerEnter(Collider hit) {
			if (hit.gameObject.tag == "PipeTrigger") {
				foreach (GameObject p in GameObject.FindGameObjectsWithTag("Pipe")) {
					p.GetComponent<Pipe>().Open ();
				}
			}
		}
	}
}