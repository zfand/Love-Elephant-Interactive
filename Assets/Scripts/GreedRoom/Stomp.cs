using UnityEngine;
using LoveElephant;
using System.Collections;

public class Stomp : MonoBehaviour {

	public float Damage = 20f;
	public float ExplosiveForce = 10f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll){
		if(coll.gameObject.CompareTag("Player")){
			coll.GetComponent<PlayerStats>().TakeDamage(Damage);
			coll.rigidbody.AddExplosionForce(ExplosiveForce, this.transform.position, transform.GetComponent<SphereCollider>().radius);
		}
	}
}
