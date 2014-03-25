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
		public GameObject CameraObject;
		public GameObject player;
		public List<GameObject> PukeSpots;
		private CameraShake shake;
		
		private List<Pipe> Pipes;
		private float idleCooldown;
		private float MaxPukeValue = 100f;
		private float currentPukeValue = 0f;
		Animator anim;
		bool Moving;
		bool Rotating;
		public GameObject Destination;
		bool AtDest;
		AnimatorStateInfo animinfo;
		bool FaceRight;
		bool ResetGG = false;
		GGState state;
		GGState nextState;
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

			shake = CameraObject.GetComponent<CameraShake>();
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
				if(animinfo.IsName("AttackOver")){
					Idle ();
				}
			}
			if(state == GGState.Walk){

				if(!FacingDestination() && !Rotating){
					Moving = false;
					StartCoroutine(FaceDestination());
				} else if(!Moving){
					StartCoroutine(MoveToDestination());

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
			} else if(c.collider.gameObject.CompareTag("Floor") && state == GGState.Attack){
				
				shake.Shake();
				foreach(Pipe p in Pipes){
					p.Open();
				}
			}

		}
		void OnTriggerStay(Collider c){
			if(c.gameObject.CompareTag("Player") && nextState == GGState.Attack){
				if(animinfo.IsName("Walk")){ 
					AtDest = true;
				}
			}			
			if(c.gameObject.CompareTag("Pipe") && nextState == GGState.Drink){
				if(animinfo.IsName("Walk")){ 
					AtDest = true;
				}
			}

			if(c.gameObject.CompareTag("Wall")){
				if(nextState == GGState.Vomit){
					if(animinfo.IsName("Walk")){ 
						AtDest = true;
					}
				} else {
					AtDest = true;
					ResetGG = true;

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
			if(ResetGG){
				Idle ();
				yield break;
			}
			Moving = false;
			StartCoroutine(IdleThenTrigger(nextState.ToString()));
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
			Debug.Log ("Pick new");
			bool pipesopen = false;
			Dictionary<string, float> possible = new Dictionary<string, float>();
			if(currentPukeValue <= MaxPukeValue){
				foreach(Pipe p in Pipes){
					if(p.IsOpen()){
						pipesopen = true;
						break;
					}
				}
			} else {
				possible.Add ("Puke", 0.3f);
			}
			if(pipesopen){
				possible.Add("Drink", 0.5f);
				possible.Add ("Attack", 0.2f);
			} else {
				possible.Add ("Attack", 0.7f);
			}

			float chance = Random.value;
			float current = 0f;
			string dothis = "";
			foreach(string s in possible.Keys){
				if(possible[s] + current <= chance){
					dothis = s;
					break;
				} else {
					current += possible[s];
				}
			}

			switch(dothis){
			case "Drink":
				int index = Mathf.FloorToInt (Random.value * PipeObjects.Count);
				if(index == PipeObjects.Count){
					index--;
				}
				Destination = PipeObjects[index];
				nextState = GGState.Drink;
				break;
			case "Attack":
				Destination = player;
				nextState = GGState.Attack;
				break;
			case "Puke":
				index = Mathf.FloorToInt (Random.value * PukeSpots.Count);
				if(index == PukeSpots.Count){
					index--;
				}
				Destination = PukeSpots[index];
				nextState = GGState.Vomit;
				break;
			default:
				Destination = player;
				nextState = GGState.Attack;
				break;
			}
			state = GGState.Walk;
			Debug.Log ("dothis: " + dothis);


		}

		void Idle(){
			anim.SetTrigger ("Idle");
			state = GGState.Idle;
			ResetGG = false;
			ResetIdle();
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
			nextState = GGState.None;
			anim.SetTrigger(s);
		}
	}

	public enum GGState {
		None,
		Idle,
		Walk,
		Drink,
		Vomit,
		Rotate,
		Attack
	}
}

