using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 20f;				// The speed the rocket will fire at.


	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	private Vector3 hitPos;
	private bool isDashing;
	private LineRenderer lr;

	void Start()
	{
		lr = this.GetComponent<LineRenderer>();
		isDashing = false;
	}

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = transform.root.GetComponent<PlayerControl>();
	}

	void OnDrawGizmos() {
		if (hitPos != null) {
			//Gizmos.DrawSphere(hitPos, 1);
			//Gizmos.DrawRay(transform.position, (hitPos - transform.position)*10);
			Gizmos.DrawLine(transform.position, hitPos);
		}
	}

	void Update ()
	{
		// If the fire button is pressed...
		if(Input.GetButtonDown("Fire1") && !isDashing)
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
				StartCoroutine(Dash(hitPos));
			} else {
				Debug.Log("MISS");
			}
			//Debug.Break();
		}

		if (Input.GetButtonUp("Fire1")) {
			isDashing = false;
			lr.enabled = false;
		}

		if (isDashing) {
			lr.SetPosition(0,transform.position);
			lr.SetPosition(1,hitPos);
			lr.enabled = true;
		}
	}

	IEnumerator Dash(Vector3 pos)
	{
		float deltaTime = 0;
		float dashSpeed = 0.5f;
		Vector3 startPos = transform.position;
		isDashing = true;
		while (deltaTime < dashSpeed)
		//while (Input.GetMouseButtonDown(0))
		{
			if (isDashing) {
				transform.position = Vector3.Lerp(transform.position, pos, deltaTime/dashSpeed);
			}
			deltaTime += Time.deltaTime;
			yield return 0;
		}
		while (isDashing) {
			transform.position = pos;
			yield return 0;
		}
	}
}
