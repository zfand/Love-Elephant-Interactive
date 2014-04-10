using UnityEngine;
using System.Collections;

namespace LoveElephant {
	public class Bullet : MonoBehaviour {
		
		GameObject player;
		public int boomPower;

		// Use this for initialization
		void Start () {
			player = GameObject.FindGameObjectWithTag("Player");
		}
		
		// Update is called once per frame
		void Update () {
			this.transform.Translate (Vector3.forward * Time.deltaTime * 5);
		}

		void OnCollisionEnter(Collision col) {
			if (!(col.gameObject.CompareTag("Turret") || col.gameObject.CompareTag("Bullet"))) {
				player.rigidbody.AddExplosionForce (boomPower, this.transform.position, 3f);
				if (Vector3.Distance (this.transform.position, player.transform.position) < 2)
					player.GetComponent<PlayerStats>().TakeDamage (5);
				this.GetComponent<MeshExploder>().Explode();
				Destroy (this.gameObject);
			}
		}
	}
}