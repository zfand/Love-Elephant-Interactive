using UnityEngine;
using System.Collections;


namespace LoveElephant
{
	public class Spawner : MonoBehaviour {

		public GameObject bugBomb;
		public float spawnRate;
		public float animTime;

		// Use this for initialization
		void Start () {
			StartCoroutine(Spawn());
		}
		
		// Update is called once per frame
		void Update () {
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
	}
}