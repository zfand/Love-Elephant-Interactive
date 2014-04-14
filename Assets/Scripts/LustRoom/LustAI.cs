using UnityEngine;
using LoveElephant;
using System.Collections;
using System.Collections.Generic;

public class LustAI : MonoBehaviour {

	public GameObject ShootTelegraphObject;	
	public GameObject DockingParticleObject;
	private ParticleSystem DockingParticle;
	public GameObject Player;
	public GameObject projectileSpawn;
	public GameObject projectile;
	public float IdleMax;
	public float IdleMin;
	public float Speed;
	public float ProjectileSpeed;
	public float DiveSpeed;
	public float DockTime;
	private float dockTimer;
	public List<GameObject> WayPoints;
	public GameObject currentWaypoint;
	public List<GameObject> DockingStations;
	private GameObject currentDock;
	public int MaxCharges = 10;
	private int Charges;

	private bool atWayPoint = false;

	private float idleCooldown;

	private ParticleSystem ShootTelegraph;



	private LustState state;
	private LustState nextstate;

	private bool Diving = false;
	private bool Moving = false;
	private bool MidDive = false;
	private bool turning = false;
	private bool shooting = false;
	private bool stopDiving = false;
	private bool facePlayer = true;

	private Animator anim;
	private AnimatorStateInfo animinfo;


	// Use this for initialization
	void Start () {	
		Random.seed = (int)Time.time;
		if(currentWaypoint == null){
			currentWaypoint = WayPoints[Random.Range (0, WayPoints.Count -1)];
		}
		ShootTelegraph = ShootTelegraphObject.particleSystem;
		DockingParticle = DockingParticleObject.particleSystem;
		state = LustState.Moving;
		StartCoroutine(MoveToWaypoint());
		idleCooldown = Random.Range(IdleMin, IdleMax);
		anim = this.GetComponent<Animator>();
		Charges = MaxCharges;
	}

	
	// Update is called once per frame
	void Update () {
		Random.seed = (int)Time.time;
		animinfo = anim.GetCurrentAnimatorStateInfo(0);

		if(facePlayer){
			if(!FacingPlayer() && !turning){
				StartCoroutine(FacePlayer());
			}
		} else if(state == LustState.Dock){
			if(currentDock != null && !FacingDock() && !turning){
				StartCoroutine(FaceObject(currentDock));
			}
		}
		if(!shooting){
			if(state == LustState.Idle){
				if(idleCooldown <= 0){
					ResetIdle();
					PickNewAction();
				} else {
					idleCooldown--;
				}
			} else if(state == LustState.Dive && !Diving){
				StartCoroutine(Dive());
			} else if(state == LustState.Moving){
				if(!Moving){
					StartCoroutine(MoveToWaypoint());
				}
			}
		}
	}

	IEnumerator Dive(){

		Diving = true;

		anim.SetTrigger("Dive");
		while(!animinfo.IsName("ChargeHold")){
			yield return 0;
		}
		Vector3 dir = Vector3.Normalize(Player.transform.position - projectileSpawn.transform.position);
		float Dashtime = 0;
		while(Dashtime < 1 && !stopDiving){
			this.transform.position = this.transform.position + dir * DiveSpeed;
			Dashtime += Time.deltaTime;
			yield return 0;
		}
		anim.SetTrigger("DiveEnd");
		stopDiving = false;
		MoveToWaypoint();
		Idle ();
		Diving = false;

	}

	IEnumerator MoveToWaypoint(){
		Moving = true;
		while(Vector3.Distance(this.transform.position, currentWaypoint.transform.position) > 1){
			this.transform.position = this.transform.position + (Vector3.Normalize(currentWaypoint.transform.position - this.transform.position)*Speed);
			yield return 0;
		}
		Moving = false;
		Idle ();
	}

	void Idle(){
		anim.SetTrigger("Idle");
		state = LustState.Idle;
	}

	void ResetIdle(){
		
		idleCooldown = Random.Range(IdleMin, IdleMax);
	}

	void PickNewAction(){
		Debug.Log ("Pick new");
		Dictionary<string, float> possible = new Dictionary<string, float>();

		
		float roll = Random.Range (0,100);

		if(Player.transform.position.y < transform.position.y){
			if(roll > 60){
				state = LustState.Dive;
				return;
			} else if(true){//roll > 20){
				if(Charges > 0){
					StartCoroutine(Shoot());
				}
				else {
					StartCoroutine (Dock());
				}
				return;
			} else {
				currentWaypoint = currentWaypoint.GetComponent<LustWaypoint>().getRandomAdjacent();
				state = LustState.Moving;
				return;
			}
		} else {
			if(roll > 20){			
				currentWaypoint = currentWaypoint.GetComponent<LustWaypoint>().getRandomAdjacent();
				state = LustState.Moving;
				return;
			} else { 
				Idle ();
			}
		}
	}


	IEnumerator Shoot(){
		shooting = true;
		ShootTelegraph.Play ();
		Charges--;

		while(ShootTelegraph.isPlaying){
			yield return 0;
		}
		anim.SetTrigger("Shoot");
		while(animinfo.IsName("Idle")){
			yield return 0;
		}

		while(animinfo.IsName("Shoot")){
			yield return 0;
		}
		
		Vector3 direction = Vector3.Normalize(Player.transform.position - projectileSpawn.transform.position);
		GameObject shot = GameObject.Instantiate(projectile) as GameObject;
		shot.transform.position = projectileSpawn.transform.position;
		shot.rigidbody.velocity = direction * ProjectileSpeed;
		shooting = false;
		Idle ();
		
	}

	IEnumerator Dock(){
		state = LustState.Dock;
		facePlayer = false;
		Moving = true;
		currentDock = DockingStations[Random.Range((int)0, (int)DockingStations.Count)];
		float dist = Vector3.Distance(this.transform.position, currentDock.transform.position);
		while(dist > 0.5){
			dist = Vector3.Distance(this.transform.position, currentDock.transform.position);
			this.transform.position = this.transform.position + (Vector3.Normalize(currentDock.transform.position - this.transform.position)*Speed);
			yield return 0;
		}
  		Moving = false;
		anim.SetTrigger ("Dock");
		dockTimer = 0;
		DockingParticle.Play ();
		while(!animinfo.IsName("DockHold")){
			yield return 0;
		}
		while(animinfo.IsName("DockHold")){
			yield return 0;
		}
		DockingParticle.Stop ();
		Charges = MaxCharges;
		facePlayer = true;
		Idle ();
	}
	
	bool FacingDock(){
		float diff = this.transform.position.x - currentDock.transform.position.x;
		if(diff > 0 && this.transform.forward.x > 0){
			return false;
		} else if(diff < 0 && this.transform.forward.x < 0){
			return false;
		}
		
		return true;
	}
	
	
	
	IEnumerator FaceObject(GameObject obj)
	{
		float interval = 5f;
		float totalrot = 0;
		bool rotating = true;
		Vector3 dest = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y + 180, this.transform.eulerAngles.z);
		turning = true;
		
		while (rotating) { 
			totalrot += interval;
			if (totalrot >= 180) {
				rotating = false;
				yield return 0;
			}
			transform.Rotate (new Vector3 (0f, interval, 0f));
			yield return 0;
		}
		turning = false;
		
		this.transform.eulerAngles = dest;
	}


	bool FacingPlayer(){
		float diff = this.transform.position.x - Player.transform.position.x;
		if(diff > 0 && this.transform.forward.x > 0){
			return false;
		} else if(diff < 0 && this.transform.forward.x < 0){
			return false;
		}

		return true;
	}

	
	
	IEnumerator FacePlayer()
	{
		float interval = 5f;
		float totalrot = 0;
		bool rotating = true;
		Vector3 dest = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y + 180, this.transform.eulerAngles.z);
		turning = true;
		
		while (rotating) { 
			totalrot += interval;
			if (totalrot >= 180) {
				rotating = false;
				yield return 0;
			}
			transform.Rotate (new Vector3 (0f, interval, 0f));
			yield return 0;
		}
		turning = false;
		
		this.transform.eulerAngles = dest;
	}

	void OnCollisionEnter(Collision c){
		if(Diving){
			if(c.gameObject.CompareTag("Player")){
				stopDiving = true;
				Player.GetComponent<PlayerStats>().TakeDamage(10);
				Player.GetComponent<PlayerController>().grounded = true;
				Player.rigidbody.AddExplosionForce(1000, projectileSpawn.transform.position, 5);
			} else if(c.gameObject.CompareTag("Wall") ||
			          c.gameObject.CompareTag("Ceiling") ||
			          c.gameObject.CompareTag("Floor") ||
			          c.gameObject.CompareTag("Platform")){
				stopDiving = true;
			}
		}
	}
}

public enum LustState {
	Idle,
	Shoot,
	Dive,
	Dock,
	Melee,
	Dead,
	Moving,
	None
}