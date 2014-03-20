using UnityEngine;
using System.Collections;

public class GreedGluttonyAI : MonoBehaviour {

	public float Speed = 0.1f;
	public float DrinkTime;
	Animator anim;
	bool Moving;
	bool Rotating;
	public GameObject Destination;
	bool AtDest;
	AnimatorStateInfo animinfo;
	bool FaceRight;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		FaceRight = transform.forward.x  > 0;
		StartCoroutine(StartWalk());
	}

	IEnumerator StartWalk(){
		yield return new WaitForSeconds(5);
		anim.SetTrigger("Walk");

	}
	// Update is called once per frame
	void Update () {
		animinfo = anim.GetCurrentAnimatorStateInfo(0);
		if(!Moving && animinfo.IsName("Walk") && !Rotating){
			if(FacingDestination()){
				StartCoroutine(MoveToDestination());
			} else {
				StartCoroutine(FaceDestination());
			}
		} else if(Moving){
			if(!FacingDestination()){
				AtDest = true;
			}
		}
	}
	void OnCollisionEnter(Collision c){
		if(c.gameObject.CompareTag("Player")){
			Debug.Log("this is working");
		}

	}
	void OnTriggerStay(Collider c){
		if(c.gameObject.CompareTag("Player")){
			if(animinfo.IsName("Walk")){
				AtDest = true;
			} else if(animinfo.IsName("Idle")){
				anim.SetTrigger("Attack");
			}
		}

	}


	IEnumerator MoveToDestination(){
		AtDest = false;
		Moving = true;
		float speed = Speed;
		if(!FaceRight){
			speed = -speed;
		}
		if(Destination == null){
			Debug.Log ("Gluttony has no Destination");
			yield break;
		}
		anim.SetTrigger("Walk");
		while(!animinfo.IsName("Walk")){
			yield return 0;
		}
		while(!AtDest){
			this.transform.position = new Vector3(this.transform.position.x + speed, transform.position.y, transform.position.z);
			yield return 0;
		}
		Moving = false;
		anim.SetTrigger("Idle");
	}

	bool FacingDestination(){
		float diff = Destination.transform.position.x - this.transform.position.x;
		if(diff > 0){
			return FaceRight;
		} else {
			return !FaceRight;
		}
	}

	IEnumerator FaceDestination(){
		float currentX = this.transform.eulerAngles.x;
		float currentY = this.transform.eulerAngles.y;
		Vector3 dest = new Vector3 (currentX, -currentY, 0f);

		float interval = 5f;
		float totalrot = 0;
		bool rotating = true;
		Rotating = true;
		if(animinfo.IsName("Walk")){
			anim.SetTrigger("Idle");
		}
		while (!animinfo.IsName("Idle")){
			yield return 0;
		}
		anim.SetTrigger("Rotate");
		while (!animinfo.IsName("Rotate")){
			yield return 0;
		}
		while (rotating) { 
			totalrot += interval;
			if (totalrot >= 180) {
				rotating = false;
				yield return 0;
			}
			transform.Rotate (new Vector3 (0f, interval, 0f));
			yield return 0;
		}
		Rotating = false;
		anim.SetTrigger("Idle");
		this.transform.eulerAngles = dest;
	}
	
	
	
}
