using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 20f;				// The speed the rocket will fire at.


	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	private Vector3 hitPos;
	private Vector3 dirPos;
	private bool isSwinging;
	private LineRenderer lr;

	void Start()
	{
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
		if(Input.GetButtonDown("Fire1") && !isSwinging)
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
				StartCoroutine(Swing(hitPos));
			} else {
				Debug.Log("MISS");
			}
			//Debug.Break();
		}

		if (Input.GetButtonUp("Fire1")) {
			isSwinging = false;
			lr.enabled = false;
		}

		if (isSwinging) {
			lr.SetPosition(0,transform.position);
			lr.SetPosition(1,hitPos);
			lr.enabled = true;
		}
	}

	IEnumerator Swing(Vector3 pos)
	{
		float deltaTime = 0;
		float dashSpeed = 0.5f;
		isSwinging = true;
		Vector3 dir = (pos - transform.position).normalized;;

		Vector3 perp = Vector3.Cross(transform.forward, dir);
		bool swingingRight = Vector3.Dot(perp,Vector3.up) > 0;


		while (deltaTime < dashSpeed)
		{
			if (isSwinging) {

				transform.position = Vector3.Lerp(transform.position, pos, deltaTime/dashSpeed);

				dir = (pos - transform.position).normalized;
				//make up be pos
				transform.rotation = Quaternion.LookRotation( transform.forward, dir );

				if (swingingRight) {
					dirPos = transform.right;
				} else {
					dirPos = transform.right*-1;
				}
				transform.position += dirPos;

			}
			deltaTime += Time.deltaTime;
			yield return 0;
		}
	}
}
