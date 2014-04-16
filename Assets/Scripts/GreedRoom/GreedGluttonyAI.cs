using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LoveElephant 
{
	public class GreedGluttonyAI : MonoBehaviour {

		public float Speed = 0.1f;
		public float DrinkTime;
		public float IdleMax;
		public float IdleMin;
		private float direction;

		public List<GameObject> PipeObjects; 
		public GameObject CameraObject;
		public GameObject player;
		public GameObject DroolObject;
		public GameObject VomitObject;
		public GameObject DrinkSplashObj;
		public GameObject GreedFireObj;
		public GameObject Stomp;
		public GameObject greed;

		private ParticleSystem DrinkSplash;
		private ParticleSystem GreedFire;
		public List<GameObject> PukeSpots;



		private CameraShake shake;
		private ParticleSystem Drool;
		private List<Pipe> Pipes;
		private float idleCooldown;
		private float MaxPukeValue = 100f;
		private float currentPukeValue = 0f;
		private Color enragedColor = new Color(255, 90, 90);
		Animator anim;
		bool Moving;
		bool Rotating;
		bool Drinking;
		bool Vomiting = false;
		bool waitingforstomp = false;
		bool Enraged = false;

		bool dying = false;
		public GameObject Destination;
		AnimatorStateInfo animinfo;
		bool faceRight;
		bool FaceRight 
		{
			get
			{ 
				return faceRight;
			}
			set
			{
				if(value){
					direction = 1;
				} else {
					direction = -1;
				}
				faceRight = value;
			}

		}
		bool ResetGG = false;
		GGState state;
		GGState nextState;
		// Use this for initialization
		void Start () {
			Random.seed = (int)Time.time;
			idleCooldown = Random.Range(IdleMin, IdleMax);
			anim = GetComponent<Animator> ();
			Drool = DroolObject.particleSystem;	
			DrinkSplash = DrinkSplashObj.particleSystem;
			GreedFire = GreedFireObj.particleSystem;
			state = GGState.Idle;
			FaceRight = transform.forward.x  > 0;

			Pipes = new List<Pipe>();
			foreach(GameObject g in PipeObjects){
				Pipes.Add (g.GetComponent<Pipe>());
			}
			if(player == null){
				player = GameObject.FindGameObjectWithTag("Player");
			}
			shake = CameraObject.GetComponent<CameraShake>();
			//StartCoroutine(StartWalk());
		}

		// Update is called once per frame
		void Update () {
			if(!Enraged && greed == null){
				Enraged = true;
				GreedFire.Play ();
				Speed *= 2;
				IdleMax = IdleMax/2;
				IdleMin = IdleMin/2;
				this.renderer.material.color = Color.red;
				Debug.Log ("Yolo");
			}
			animinfo = anim.GetCurrentAnimatorStateInfo(0);
			if(!dying){
				rigidbody.AddForce(-Vector3.up * 10000f);

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
				if(state == GGState.Drink && !Drinking){
					StartCoroutine(Drink());
				}
				if(state == GGState.Walk){
					if(!FacingDestination() && !Rotating){
						FaceDestination();
					} else if(!Moving){
						StartCoroutine(MoveToDestination());
					}
				}
				if(state == GGState.Vomit && animinfo.IsName("Vomit") && !Vomiting){
					StartCoroutine(VomitLoop());
				}
			} else {
				if (animinfo.IsName ("Dead")) {
					this.GetComponentInChildren<Gluttony> ().Die ();
				}
			}

		}

		void OnCollisionEnter(Collision c){
			if(c.gameObject.CompareTag("Player")){
				Debug.Log("this is working");
			} 

			if(c.collider.gameObject.CompareTag("Floor") && state == GGState.Attack){
				if((animinfo.IsName("StabAttack") || animinfo.IsName("AttackOver"))){
					if(!waitingforstomp){
						StartCoroutine(WaitForStompToEnd());
					}
				}
			}
		}

		IEnumerator WaitForStompToEnd(){
			waitingforstomp = true;
			bool waiting = true;
			while(!animinfo.IsName ("AttackOver") && 
			      !animinfo.IsName ("Idle") && 
			      !(animinfo.IsName("StabAttack") && anim.IsInTransition(0))){
				yield return 0;
			}
			shake.Shake();
			foreach(Pipe p in Pipes){
				p.Open();
			}
			waitingforstomp = false;
			StartCoroutine(StompDamage());
		}

		IEnumerator StompDamage(){

			float frames = 10f;
			float currentFrames = 0f;
			Stomp.SetActive(true);
			while(frames > currentFrames){
				currentFrames +=1;
				yield return 0;
			}
			
			Stomp.SetActive(false);
		}

		IEnumerator VomitLoop(){
			Vomiting = true;
			GameObject vomitcopy = GameObject.Instantiate(VomitObject, VomitObject.transform.position, VomitObject.transform.rotation) as GameObject;
			
			Transform neck = VomitObject.transform.parent;
			vomitcopy.transform.parent = neck;
			ParticleSystem vomit = vomitcopy.particleSystem;
			vomit.Play ();
			Quaternion orig_rotation = neck.rotation;
			float rotatespeed = 0.2f;
			float max_rotate = 80f;
			if(faceRight){
				rotatespeed *= -1;
				max_rotate *= -1;
			}
			float currentrotate = 0f;
			while(currentPukeValue > 0f){
				currentPukeValue -= 0.25f;
				if(currentrotate <= 0f){
					rotatespeed = Mathf.Abs (rotatespeed);
				} else if(currentrotate >= max_rotate){
					rotatespeed = -1 * (Mathf.Abs (rotatespeed));
				}
				currentrotate += Mathf.Abs(rotatespeed);
				vomitcopy.transform.Rotate (Vector3.forward, -rotatespeed, Space.World);
				//vomitcopy.transform.Rotate (Vector3.up, 5, Space.World);
				yield return 0;
			}			
			Vomiting = false;
			vomit.Stop ();
			vomitcopy.transform.parent = null;
			Drool.Stop();
			neck.rotation = orig_rotation;
			Destroy (vomitcopy);
			Idle ();

		}

		void OnTriggerStay(Collider c){
			//if(c.gameObject.CompareTag("Wall")){
			//	if(nextState == GGState.Vomit){
			//		if(animinfo.IsName("Walk")){ 
			//			AtDest = true;
			//		}
			//	} else {
			//		AtDest = true;
			//		ResetGG = true;
			//
			//	}
			//}

		}


		IEnumerator MoveToDestination(){
//			AtDest = false;
			Moving = true;

			if(Destination == null){
				Debug.Log ("Gluttony has no Destination");
				yield break;
			}
			anim.SetTrigger("Walk");
			while(!animinfo.IsName("Walk")){
				yield return 0;
			}
			float distance = 2f;
			if(Destination.CompareTag("Player")){
				distance = 4f;
			} else if(nextState == GGState.Vomit){
				distance = 5f;
			}


			while(Mathf.Abs(this.transform.position.x - Destination.transform.position.x) > distance){//|| Mathf.Abs(this.transform.position.x - Destination.transform.position.x) < mindistance){
				this.transform.position = new Vector3(this.transform.position.x + Speed*direction, transform.position.y, transform.position.z);
				yield return 0;
			}
			if(ResetGG){
				Idle ();
				nextState = GGState.None;
				yield break;
			}
			if(nextState == GGState.Vomit){
				StartCoroutine(Rotate ());
			}
			while(Rotating){
				yield return 0;
			}
			state = nextState;
			nextState = GGState.None;
			Moving = false;
			StartCoroutine(IdleThenTrigger(state.ToString()));
		}

		IEnumerator Drink(){
			Drinking = true;
			currentPukeValue = 0;
			DrinkSplash.Play ();
			while (currentPukeValue < MaxPukeValue){// && currentDrinkTime < DrinkTime){
				//currentPukeValue += PukeChargeRate;
				//currentDrinkTime += Time.deltaTime;
				currentPukeValue += 0.25f;
				yield return 0;
			}
			Drinking = false;
			Idle ();
			Drool.Play ();
			DrinkSplash.Stop ();
			ClosePipes();

		}


		void ClosePipes(){
			foreach(Pipe p in Pipes){
				p.Close ();
			}
		}


		bool FacingDestination(){
			float diff = Destination.transform.position.x - this.transform.position.x;
			if(diff > 0){
				return FaceRight;
			} else {
				return !FaceRight;
			}
		}

		void FaceDestination(){
			StartCoroutine(Rotate ());
		}

		IEnumerator Rotate(){
			
			float currentX = this.transform.eulerAngles.x;
			float currentY = this.transform.eulerAngles.y;
			Vector3 dest = new Vector3 (currentX, -currentY, 0f);
			
			float interval = 5f;
			float totalrot = 0;
			bool rotating = true;
			direction = 0;
			Rotating = true;
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
			FaceRight = transform.forward.x  > 0;
		}
		
		void ResetIdle(){
			
			idleCooldown = Random.Range(IdleMin, IdleMax);
		}

		void PickNewAction(){
			Debug.Log ("Pick new");
			bool pipesopen = false;
			Dictionary<string, float> possible = new Dictionary<string, float>();
			if(currentPukeValue < MaxPukeValue){
				foreach(Pipe p in Pipes){
					if(p.IsOpen()){
						pipesopen = true;
						break;
					}
				}
			} else {
				possible.Add ("Puke", 30f);
			}
			if(pipesopen && !Enraged){
				possible.Add("Drink", 50f);
				possible.Add ("Attack", 50f);
			} else {
				if(Enraged){
					possible.Add ("Attack", 100f);
				}else {
					possible.Add ("Attack", 70f);
				}
			}

			float chance = Random.Range(0, 100);
			float current = 0f;
			string dothis = "";
			foreach(string s in possible.Keys){
				if(possible[s] <= chance){
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
			state = GGState.Walk;
			if(!animinfo.IsName("Idle")){
				StartCoroutine(IdleThenTrigger("Walk"));
				return;
			}
			anim.SetTrigger("Walk");
		}

		void Attack(){			
			state = GGState.Attack;
			if(!animinfo.IsName("Idle")){
				StartCoroutine(IdleThenTrigger("Attack"));
				return;
			}
			anim.SetTrigger("Attack");
		}

		void Vomit(){
			state = GGState.Vomit;
			if(!animinfo.IsName("Idle")){
				StartCoroutine(IdleThenTrigger("Vomit"));
				return;
			}
			anim.SetTrigger("Vomit");
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
        yield break;
			}
			nextState = GGState.None;
			anim.SetTrigger(s);
		}

		public void Dying()
		{
			dying = true;
			anim.SetTrigger ("Die");
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

