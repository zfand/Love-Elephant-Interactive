using UnityEngine;
using System.Collections;

namespace LoveElephant {
	public class Bullet : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			this.transform.Translate (Vector3.forward * Time.deltaTime * 5);
		}

		void OnCollisionEnter(Collision col) {
			if (!(col.gameObject.CompareTag("Turret") || col.gameObject.CompareTag("Bullet"))) {
				this.GetComponent<MeshExploder>().Explode();
				Destroy (this.gameObject);
			}
		}
	}
}