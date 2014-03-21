using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Boss 
{
	public class GreedGluttonyAI : MonoBehaviour {

		public float Speed = 0.1f;
		public float DrinkTime;
		public float IdleMax;
		public float IdleMin;
		public List<GameObject> PipeObjects; 
		private List<Pipe> Pipes;
		private float idleCooldown;
		Animator anim;
		bool Moving;
		bool Rotating;
		public GameObject Destination;
		bool AtDest;
		AnimatorStateInfo animinfo;
		bool FaceRight;
		GGState state;
		// Use this for initialization
		void Start () {
			Random.seed = (int)Time.time;
			idleCooldown = Random.Range(IdleMin, IdleMax);
			anim = GetComponent<Animator> ();
			state = GGState.Idle;
			Attack ();
			FaceRight = transform.forward.x  > 0;
			Pipes = new List<Pipe>();
			foreach(GameObject g in PipeObjects){
				Pipes.Add (g.GetComponent<Pipe>());
			}
			//StartCoroutine(StartWalk());
		}

		IEnumerator StartWalk(){
			yield return new WaitForSeconds(5);
			Walk();

		}
		// Update is called once per frame
		void Update () {
			animinfo = anim.GetCurrentAnimatorStateInfo(0);
			if(state == GGState.Idle){
				if(idleCooldown <= 0){
					ResetIdle();
					PickNewAction();
				} else {
					idleCooldown--;
				}
			}
			if(state == GGState.Attack){
				if((animinfo.IsName("Attack") && anim.IsInTransition(0)) || animinfo.IsName("AttackOver")){ 
					foreach(Pipe p in Pipes){
						p.Open();
					}
					Idle ();
				}
			}
			if(!Moving && animinfo.IsName("Walk") && !Rotating && state == GGState.Walk){
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
					//anim.SetTrigger("Attack");
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

			Idle();
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
				if(animinfo.IsName("Walk")){
					Debug.Log ("I'm gonna scream");
				}
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
			this.transform.eulerAngles = dest;
			Rotating = false;
			Idle ();
		}
		
		void ResetIdle(){
			
			idleCooldown = Random.Range(IdleMin, IdleMax);
		}

		void PickNewAction(){


		}

		void Idle(){
			anim.SetTrigger ("Idle");
			state = GGState.Idle;
		}

		void Walk(){			
			if(!animinfo.IsName("Idle")){
				StartCoroutine(IdleThenTrigger("Walk"));
				return;
			}
			anim.SetTrigger("Walk");
			state = GGState.Walk;
		}

		void Attack(){			
			if(!animinfo.IsName("Idle")){
				StartCoroutine(IdleThenTrigger("Attack"));
				return;
			}
			anim.SetTrigger("Attack");
			state = GGState.Attack;
		}

		void Vomit(){
			if(!animinfo.IsName("Idle")){
				StartCoroutine(IdleThenTrigger("Vomit"));
				return;
			}
			anim.SetTrigger("Vomit");
			state = GGState.Vomit;
		}

		//handle extraneous animation issues
		IEnumerator IdleThenTrigger(string s){
			anim.SetTrigger("Idle");
			float Exitcountdown = 100;
			while (!animinfo.IsName("Idle") || Exitcountdown <= 0){
				Exitcountdown -= 1;
				yield return 0;
			}
			if(Exitcountdown <= 0){
				anim.Play ("Idle");
				state = GGState.Idle;
				return true;
			}
			switch(s) {
				case "Walk":
					state = GGState.Walk;
					break;
				case "Drink":
					state = GGState.Drink;
					break;
				case "Vomit":
					state = GGState.Vomit;
					break;
				case "Rotate":
					state = GGState.Rotate;
					break;
				case "Attack":
					state = GGState.Attack;
					break;
				default:
					state = GGState.Idle;
					break;
			}
			
			anim.SetTrigger(s);
		}
	}

	public enum GGState {
		Idle,
		Walk,
		Drink,
		Vomit,
		Rotate,
		Attack
	}
}

