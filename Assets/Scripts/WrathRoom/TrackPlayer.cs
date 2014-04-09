using UnityEngine;
using System.Collections;

namespace LoveElephant
{
	public class TrackPlayer : MonoBehaviour {

		public GameObject chargeShot;
		public GameObject beam;
		GameObject player;
		public float speed;
		private Quaternion targetRotation;
		private Vector3 targetPosition;

		public float shotSpeed;
		private bool firing;

		void Start () {
			player = GameObject.FindGameObjectWithTag("Player");
			targetRotation = Quaternion.identity;
			targetPosition = this.transform.position;
			firing = false;
			this.animation.PlayQueued("Wrath_Idle", QueueMode.PlayNow);
			StartCoroutine(LookAndFire());

		}

		IEnumerator LookAndFire() {
			while (true) {
				if (!firing) {
					StartCoroutine(Look());
					yield return new WaitForSeconds(shotSpeed);
					firing = true;
				} else {
					if (firing) {
						StartCoroutine (Fire ());
						yield return new WaitForSeconds(2f);
						chargeShot.renderer.enabled = false;
						beam.particleSystem.Stop ();
						GetComponent<Animation>().Play("Wrath_Idle");
					}
				}

			}
		}

		IEnumerator Look() {
			while (true) {
				targetRotation = Quaternion.LookRotation (player.transform.position - this.transform.position);
				targetPosition = player.transform.position;
				float str = Mathf.Min (speed * Time.deltaTime, 1);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
				targetPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
				transform.position = Vector3.Lerp (transform.position, targetPosition, str/2);
				yield return new WaitForSeconds(0f);
			}
		}
		
		IEnumerator Fire() {
			GetComponent<Animation>().Play ("Wrath_Shoot");
			chargeShot.renderer.enabled = true;
			beam.particleSystem.Play ();
			yield return new WaitForSeconds(2f);
			firing = false;
		}
	}
}
