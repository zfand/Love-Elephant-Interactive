using UnityEngine;
using System.Collections;

public class GrappleController : MonoBehaviour
{
	/// <summary>
    ///  Reference to the Animator component.
	/// </summary>
	private Animator anim;
	/// <summary>
	/// Draws the line for the grappling rope
	/// </summary>
	private LineRenderer lr;
	/// <summary>
	/// The position the Grappling hook hit
	/// </summary>
	private Vector3 hitPos;
	/// <summary>
	/// Reference to the SpringJoint
	/// </summary>
	private HingeJoint hinge;
	/// <summary>
	/// Whether the player is swinging
	/// </summary>
	private bool isSwinging;
	private bool isYanking;

	private Vector3 startPos;
	private float startGrappleTime;
	private float ropeLen = 0f;
	public float dashSpeed = 100f;
	public GameObject grapplePivot;

	private Transform ropePos;

	void Start ()
	{
		lr = this.GetComponent<LineRenderer>();
		isSwinging = false;
		isYanking = false;
	}

	void OnDrawGizmos() {
		if (hitPos != null && hitPos != Vector3.zero) {
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, hitPos);
		}
	}

	private void Update() {

		if (Input.GetButton ("Fire1") && isYanking) {
			float distanceTraveled = (Time.time - startGrappleTime) * dashSpeed;
			float percentDone = distanceTraveled/ropeLen;
			transform.position = Vector3.Lerp(startPos,grapplePivot.transform.position,percentDone);

			if (percentDone > .6f) {
				isYanking = false;
				isSwinging = true;
				hinge = gameObject.AddComponent<HingeJoint>();

				hinge.useLimits = false;
				hinge.axis = new Vector3(1,1,0);
			}

		} else if (!Input.GetButtonDown ("Fire1") && isYanking) {
			isYanking = false;
			Vector3 aftershock = hitPos - transform.position;
			rigidbody.AddForce(aftershock * (dashSpeed * 5));
		} else if (Input.GetButtonDown ("Fire1") && !isYanking) {
			Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickedPosition.z = 0;
			LayerMask layermask = ~(1 << LayerMask.NameToLayer("Player"));
			RaycastHit hit;
			Physics.Raycast(transform.position, clickedPosition - transform.position, out hit, 1000, layermask);
			if (hit.collider) {
				startGrappleTime = Time.time;
				hitPos = new Vector3(hit.point.x,hit.point.y,0);
				grapplePivot.transform.position = hitPos;
				isYanking = true;
				startPos = transform.position;
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				ropeLen = Vector3.Distance (startPos, grapplePivot.transform.position);
			}
		} else if (Input.GetButtonDown ("Fire1") && isSwinging) {
		} else if (!Input.GetButton ("Fire1") && isSwinging) {
			isSwinging = false;
			Destroy (hinge);
		}

		if (isYanking || isSwinging) {
			lr.SetPosition(0,transform.position);
			lr.SetPosition(1,hitPos);
			lr.enabled = true;
		} else {
			lr.enabled = false;
		}
	}

	private void Awake() 
	{
		anim = transform.root.gameObject.GetComponentInChildren<Animator>();
		if (anim == null)
		{
			Debug.LogError("The Player's Animator is NULL!");
		}
		ropePos = transform.Find("RopePos");
		if (ropePos == null) {
			Debug.LogError("There is no RopePos transform on the Player");
		}
	}

}

