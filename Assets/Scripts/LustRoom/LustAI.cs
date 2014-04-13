using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LustAI : MonoBehaviour {

	public GameObject ShootPreCumObject;	
	public GameObject Player;
	public GameObject projectileSpawn;
	public GameObject projectile;
	public float IdleMax;
	public float IdleMin;
	public float Speed;
	public float ProjectileSpeed;
	public float DiveSpeed;
	public List<GameObject> WayPoints;
	public GameObject currentWaypoint;

	private bool atWayPoint = false;

	private float idleCooldown;

	private ParticleSystem ShootPrecum;



	private LustState state;
	private LustState nextstate;

	private bool Diving = false;
	private bool Moving = false;
	private bool MidDive = false;
	private bool turning = false;
	private bool shooting = false;
	private bool stopDiving = false;

	private Animator anim;
	private AnimatorStateInfo animinfo;


	// Use this for initialization
	void Start () {	
		Random.seed = (int)Time.time;
		if(currentWaypoint == null){
			currentWaypoint = WayPoints[Random.Range (0, WayPoints.Count -1)];
		}
		ShootPrecum = ShootPreCumObject.particleSystem;
		state = LustState.Moving;
		StartCoroutine(MoveToWaypoint());
		idleCooldown = Random.Range(IdleMin, IdleMax);
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		Random.seed = (int)Time.time;
		animinfo = anim.GetCurrentAnimatorStateInfo(0);

		
		if(!FacingPlayer() && !turning){
			StartCoroutine(FacePlayer());
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
			} else if(roll > 20){
				StartCoroutine(Shoot());
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
		ShootPrecum.Play ();

		while(ShootPrecum.isPlaying){
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