using UnityEngine;
using System.Collections;


namespace LoveElephant
{
	public class Spawner : MonoBehaviour {

		public GameObject bugBomb;
		public float spawnRate;
		public float animTime;

		public int health;

		// Use this for initialization
		void Start () {
			StartCoroutine(Spawn());
		}
		
		// Update is called once per frame
		void Update () {
			if (health == 0)
				StartCoroutine(ShutDown());
		}

		IEnumerator ShutDown() {
			health = -1;
			this.GetComponentInChildren<ParticleSystem>().Play();
			this.GetComponent<MeshExploder>().Explode();
			this.renderer.enabled = false;
			yield return new WaitForSeconds(1f);
			Destroy (this.gameObject);
		}

		IEnumerator Spawn() {
			while (true) {
				yield return new WaitForSeconds(spawnRate);
				StartCoroutine(NewBug());
			}
		}

		IEnumerator NewBug() {
			yield return new WaitForSeconds(animTime);
			int spawnDist = 3;
			if (this.transform.position.x > 0)
				spawnDist *= -1;
			Instantiate (bugBomb, new Vector3(this.transform.position.x + spawnDist, this.transform.position.y, 0), Quaternion.Euler (0,0,90));
			yield return new WaitForSeconds(animTime);
		}
	

		void TakeDamage() {
			health -= 30;
		}

		void OnTriggerEnter(Collider col) {
			if (col.gameObject.CompareTag ("Weapon")) {
				health -= 20;
			}
		}
	}
}