using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LustAI : MonoBehaviour {

	public GameObject ShootPreCumObject;	
	public float IdleMax;
	public float IdleMin;
	public float Speed;
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

	private Animator anim;
	private AnimatorStateInfo animinfo;


	// Use this for initialization
	void Start () {	
		Random.seed = (int)Time.time;
		if(currentWaypoint == null){
			currentWaypoint = WayPoints[Random.Range (0, WayPoints.Count -1)];
		}
		
		state = LustState.Moving;
		StartCoroutine(MoveToWaypoint());
		idleCooldown = Random.Range(IdleMin, IdleMax);
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		animinfo = anim.GetCurrentAnimatorStateInfo(0);

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

	IEnumerator Dive(){

		Diving = true;

		anim.SetTrigger("Dive");
		while(!animinfo.IsName("ChargeHold")){
			yield return 0;
		}
		StartCoroutine(DiveAtPlayer());

		while(MidDive){
			yield return 0;
		}

		Idle();
		Diving = false;

	}

	IEnumerator DiveAtPlayer(){
		MidDive = true;
		yield return new WaitForSeconds(1);

		MidDive = false;
	}

	IEnumerator MoveToWaypoint(){
		Moving = true;
		while(Vector3.Distance(this.transform.position, currentWaypoint.transform.position) > 1){
			this.transform.position = this.transform.position + (Vector3.Normalize(currentWaypoint.transform.position - this.transform.position)*Speed);
			yield return 0;
		}
		Moving = false;
		state = LustState.Idle;
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
		bool pipesopen = false;
		Dictionary<string, float> possible = new Dictionary<string, float>();

		
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