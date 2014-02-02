using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 20f;			// The speed the rocket will fire at.


	public float ropeLen = 0f;
	public float dashSpeed = 0f;


	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	private Vector3 hitPos;
	private Vector3 dirPos;
	private bool isSwinging;
	private LineRenderer lr;
	SpringJoint2D spring;

	
	void Start()
	{
		spring = this.GetComponent<SpringJoint2D>();
		lr = this.GetComponent<LineRenderer>();
		isSwinging = false;
	}

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = transform.root.GetComponent<PlayerControl>();
	}

	void OnDrawGizmos() {
		if (hitPos != null && hitPos != Vector3.zero) {
			//Gizmos.DrawSphere(hitPos, 1);
			//Gizmos.DrawRay(transform.position, (hitPos - transform.position)*10);
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, hitPos);
		}
		if (dirPos != null && dirPos != Vector3.zero) {
			Gizmos.color = Color.green;
			Gizmos.DrawRay(transform.position, dirPos*5);
			Gizmos.color = Color.black;
			Gizmos.DrawRay(transform.position, Vector3.up*5);
			Gizmos.DrawRay(transform.position, Vector3.down*5);
			Gizmos.DrawRay(transform.position, Vector3.right*5);
			Gizmos.DrawRay(transform.position, Vector3.left*5);
		}
	}

	void Update ()
	{
		// If the fire button is pressed...
		//if(Input.GetButtonDown("Fire1") && !isSwinging)
		if(Input.GetButtonDown("Fire1"))
		{
			// ... set the animator Shoot trigger parameter and play the audioclip.
			anim.SetTrigger("Shoot");
			audio.Play();

			Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			hitPos = clickedPosition;
			hitPos.z = 0;
			Debug.Log(hitPos);

			LayerMask layermask = ~(1 << LayerMask.NameToLayer("Player"));
			RaycastHit2D hit = Physics2D.Raycast(transform.position, hitPos - transform.position, 100, layermask);
			if (hit.collider) {
				hitPos = new Vector3(hit.point.x,hit.point.y,-1);
				StartCoroutine("Swing", hitPos);
			} else {
				Debug.Log("MISS");
			}
			//Debug.Break();
		}

		if (!Input.GetButton("Fire1")) {
			isSwinging = false;
			lr.enabled = false;
			spring.enabled = false;
			StopCoroutine("Swing");
		}

		if (isSwinging) {
			lr.SetPosition(0,transform.position);
			lr.SetPosition(1,hitPos);
			lr.enabled = true;
		}
	}


}
