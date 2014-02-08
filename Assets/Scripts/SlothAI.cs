using UnityEngine;
using System.Collections;

public class SlothAI : MonoBehaviour {

	public GameObject player;
	private Animation anim;

	public float Health = 100;

	private bool faceLeft;

	// Use this for initialization
	void Start	 () {
		anim = GetComponent<Animation>();
		faceLeft = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (!anim.IsPlaying ("Sloth_Charge"))
			facePlayer();

		if (sameLevel()) {
			if (anim.IsPlaying("Sloth_BeginCharge")) {
				anim.PlayQueued ("Sloth_Charge", QueueMode.CompleteOthers);
			} else if (anim.IsPlaying("Sloth_Charge")) {
				charge(faceLeft);
			} else if (anim.IsPlaying("Sloth_EndCharge")) {
				anim.PlayQueued("Sloth_Idle", QueueMode.CompleteOthers);
			} else if (anim.IsPlaying("Sloth_Idle")) {
				anim.PlayQueued ("Sloth_BeginCharge", QueueMode.PlayNow);
			}
		} else {
			if (!anim.IsPlaying("Sloth_Idle") && !anim.IsPlaying ("Sloth_Charge")) {
				anim.Stop();
				anim.PlayQueued ("Sloth_Idle", QueueMode.PlayNow);
			}
		}

	}

	void charge(bool lookL) {
		float xPos = transform.position.x;
		float yPos = transform.position.y;

		if (faceLeft) {
			transform.position = new Vector3(xPos - 0.5f, yPos, 0f);
		} else {
			transform.position = new Vector3(xPos + 0.5f, yPos, 0f);
		}
	}

	void OnCollisionEnter(Collision hit) {
		if (hit.gameObject.tag == "Wall") {
			facePlayer ();
			anim.PlayQueued("Sloth_EndCharge", QueueMode.PlayNow);
		}
	}
	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Sword"){
			float starthealth = Health;
			Sword sword = player.GetComponent<Sword>();
			Health -= sword.Damage;
			sword.Hit();
			if(starthealth >= 50 && Health <= 50){
				this.transform.Find ("Smoke1").particleSystem.Play ();
			}

		}
	}
	//Face the player
	void facePlayer() {

		float currentX = this.transform.eulerAngles.x;
		float currentY = this.transform.eulerAngles.y;

		if (player.transform.position.x < this.transform.position.x) {
			if (!faceLeft) {
				faceLeft = true;
				this.transform.eulerAngles = new Vector3(currentX, -currentY, 0f);
			}
		} else if (faceLeft) {
			faceLeft = false;
			this.transform.eulerAngles = new Vector3(currentX, -currentY, 0f);
		}
	}

	//If this is at the same level as the player
	bool sameLevel() {

		float heightDiff = Mathf.Abs (player.transform.position.y - this.transform.position.y);

		if (heightDiff < 4.5) {
			return true;
		}

		return false;
	}
}
