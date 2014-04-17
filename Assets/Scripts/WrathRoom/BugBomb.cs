using UnityEngine;
using System.Collections;

namespace LoveElephant
{
	public class BugBomb : MonoBehaviour {

		GameObject player;
		public float moveSpeed;
		public float boomPower;

		private bool moving;

		private int lifespan;
		public int maxAge;

		// Use this for initialization
		void Start () {
			lifespan = 0;
			moving = true;
			player = GameObject.FindGameObjectWithTag("Player");
		}
		
		// Update is called once per frame
		void Update () {
			if (moving) {
				if (Vector3.Distance(this.transform.position, player.transform.position) < 2) {
					moving = false;
					StartCoroutine(Detonate());
				} else if (player.transform.position.x > this.transform.position.x) {
					this.rigidbody.AddForce (new Vector3(moveSpeed, 0f, 0f));
				} else if (player.transform.position.x < this.transform.position.x) {
					this.rigidbody.AddForce (new Vector3(-moveSpeed, 0f, 0f));
				}
			}
		}

		void FixedUpdate() {
			lifespan++;
			if (lifespan > maxAge) {
				StartCoroutine(Detonate());
				lifespan = 0;
			}
		}

		public IEnumerator Detonate() {
			yield return new WaitForSeconds(2f);
			this.GetComponent<MeshExploder>().Explode();
			this.GetComponent<ParticleSystem>().Play();
			player.rigidbody.AddExplosionForce (boomPower, this.transform.position, 3f);
			this.renderer.enabled = false;
			this.collider.enabled = false;
			if (Vector3.Distance (this.transform.position, player.transform.position) < 2)
				player.GetComponent<PlayerStats>().TakeDamage (5);
			yield return new WaitForSeconds(1f);
			Collider[] nearbyBombs = Physics.OverlapSphere (this.transform.position, 10f);
			int bombCount = 0;
			foreach (Collider c in nearbyBombs) {
				if (c.gameObject.CompareTag("BugBomb")) {
					bombCount++;
					c.GetComponent<BugBomb>().Detonate();
				}
			}
			Destroy (this.gameObject);
		}
	}
}